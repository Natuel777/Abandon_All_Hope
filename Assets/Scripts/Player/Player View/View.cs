using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class View
{
    private Health _health;
    [SerializeField] private SceneDataSO _sceneData;
    [SerializeField] private FullScreenPassRendererFeature _postProcessingFeature;

    [Header("Damage PP")]
    [SerializeField] private Material _screenDamageMaterial;
    private Coroutine _screenDamageTask;
    private Func<IEnumerator, Coroutine> _startCoroutine;
    private Action<IEnumerator> _stopCoroutine;

    [Header("Fade PP")]
    [SerializeField] private Material _fadeMaterial;
    private float _fadeSpeed = 0.2f;
    private float FadeTime {set{_fadeMaterial.SetFloat("_FadeTime", value);}}

    [Header("Intro Sequence")]
    [Tooltip("Desactivar para saltarse el fade negro y la secuencia de introducción (escenas de prueba).")]
    [SerializeField] private bool _playIntroSequence = false;

    public View Initialize(Func<IEnumerator, Coroutine> coroutineTask, Action<IEnumerator> stopCoroutine, Health h)
    {
        _startCoroutine = coroutineTask;
        _stopCoroutine = stopCoroutine;
        _health = h;

        _screenDamageMaterial.SetFloat("_VignetteRadius", 3f);

        EventManager.Subscribe("OnPlayerDamaged", ScreenDamage);

        // [DESACTIVADO] Inicialización del material de FullScreen Pass (fade/intro).
        // Causaba pantalla negra al iniciar. Comentado a pedido del usuario.
        // Para reactivar la secuencia de intro / el fade, descomentar este bloque.
        /*
        bool firstLoad = _playIntroSequence
                         && MazeManager.Instance != null
                         && _sceneData != null
                         && !_sceneData.firtsLoadDone;

        if(firstLoad)
        {
            // Secuencia de intro: fade largo desde negro + espera a "OnIntroEnded"
            Fade(true, 35f);
            ChangeFullScreenPassMaterial(_fadeMaterial);
            EventManager.Subscribe("OnIntroEnded", x => ChangeFullScreenPassMaterial(_screenDamageMaterial));
        }

        else
        {
            // Sin intro: limpiar el fade YA (el material puede quedar persistido en negro)
            // y usar el material de daño directamente, sin esperas.
            FadeTime = 0f;
            ChangeFullScreenPassMaterial(_screenDamageMaterial);
        }
        */

        return this;
    }

    private void ScreenDamage(params object[] parameters)
    {
        Debug.Log("PP");
        foreach(var item in parameters)
        {
            if(item is float intensity)
            {
                if(_startCoroutine != null && _stopCoroutine != null) 
                {
                    _stopCoroutine(StartDamagedScreen(intensity));
                    _startCoroutine(StartDamagedScreen(intensity));
                } 

                break;
            }
        }
    }

    private IEnumerator StartDamagedScreen(float intensity)
    {
        float targetRadius = Remap(intensity, 0, 1, 0.1f, 1f);

        for(float t = 0; t <= 1; t += Time.deltaTime * 3f)
        {
            float cur = Mathf.Lerp(1f, targetRadius, t);
            _screenDamageMaterial.SetFloat("_VignetteRadius", cur);
            yield return null;
        }

        for(float t = 0; t <= 1; t += Time.deltaTime * 2f)
        {
            float lifePercent = _health.Life / _health.MaxLife;
            float dynamicRestValue = Mathf.Lerp(0.01f, 2.5f, lifePercent);
            float cur = Mathf.Lerp(targetRadius, dynamicRestValue , t);
            _screenDamageMaterial.SetFloat("_VignetteRadius", cur);
            yield return null;
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }

    private IEnumerator Interpolate(float from, float to, float waitAfter = 0)
    {
        float current = from;
        FadeTime = current;

        if(waitAfter != 0) yield return new WaitForSeconds(waitAfter);

        for(float t = 0f; current != to; t += Time.deltaTime * _fadeSpeed)
        {
            current = Mathf.Clamp01(Mathf.SmoothStep(from, to, t));
            FadeTime = current;
            yield return null;
        }
    }

    public void Fade(bool value, float waitAfter = 0)
    {
        if(value) _startCoroutine(Interpolate(1, 0, waitAfter));

        else _startCoroutine(Interpolate(0, 1, waitAfter));
    }

    private void ChangeFullScreenPassMaterial(Material material)
    {
        _postProcessingFeature.passMaterial = material;
    }
}
