using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject _deathPanel, _pausePanel, _notePanel;
    public CanvasGroup contextGroup, controlsGroup;
    public TextMeshProUGUI context, controls;

    [Header("Note UI Elements")]
    [SerializeField] private Image _noteImage;
    [SerializeField] private TextMeshProUGUI _noteText;
    public bool noteActive => _notePanel != null && _notePanel.activeSelf;
    public bool  isFading = false;
    private Coroutine _fadeRoutine = null;

    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EventManager.Subscribe("OnPlayerDeath", x => ActivateDeathPanel());
        //EventManager.Subscribe("OnTutorialEnded", x => ObjectiveManager.Instance.CompleteObjective());
        EventManager.Subscribe("OnTutorialEnded", x => ObjectiveManager.Instance.SetObjective("Vuelve a hablar con el alma en el umbral de la escalinata"));
        EventManager.Subscribe("OnTutorialEnded", x => 
                                UIManager.Instance.controls.text = "Presiona [F] para invocar un peso.\n\nUsa la rueda del ratón para seleccionar el tipo de peso que deseas invocar.\n\nPresiona [Click Izquierdo] para lanzar el peso en la dirección que estés señalando.");
        EventManager.Subscribe("OnTutorialEnded", x => FadeCanvasGroup(controlsGroup, true, true, 10f));
    }

    public void FadeCanvasGroup(CanvasGroup group, bool fade, bool cycle = false, float duration = 1f, bool disableAtEnd = false)
    {
        if(_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeRoutine(group, fade, cycle, duration, disableAtEnd));
    }

    private void ActivateDeathPanel() => _deathPanel.SetActive(true);

    public void SetPauseMenu()
    {
        if(GameManager.Instance.IsPaused)
        {
            GameManager.Instance.ActiveOrDesactive(_pausePanel, false);
            GameManager.Instance.SetPauseState(false);
        }

        else
        {
            GameManager.Instance.ActiveOrDesactive(_pausePanel, true);
            GameManager.Instance.SetPauseState(true);
        }
    }

    public void OpenNote(Sprite noteImage, string noteText)
    {
        _noteImage.sprite = noteImage;
        _noteText.text = noteText;
        GameManager.Instance.ActiveOrDesactive(_notePanel, true);
        GameManager.Instance.SetPauseState(true);
    }

    public void CloseNote()
    {
        GameManager.Instance.ActiveOrDesactive(_notePanel, false);
        GameManager.Instance.SetPauseState(false);
    }

    private IEnumerator FadeRoutine(CanvasGroup group, bool fadeIn, bool cycle, float duration, bool disableAtEnd)
    {
        isFading = true;
        float elapsed = 0f;
        float startAlpha = group.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
    
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }
    
        group.alpha = endAlpha;
    
        if(cycle)
        {
            yield return new WaitForSeconds(0.5f);
    
            elapsed = 0f;
            startAlpha = group.alpha;
            endAlpha = fadeIn ? 0f : 1f;
    
            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }
    
            group.alpha = endAlpha;
        }
    
        isFading = false;
        _fadeRoutine = null;

        if(disableAtEnd && group.alpha == 0f)
            GameManager.Instance.ActiveOrDesactive(group.gameObject, false);
    }
}
