using System.Collections;
using UnityEngine;

public class CameraShake : ProjectCamera
{
    private FPCamera _fpCamera;

    private void Awake() 
    {
        _fpCamera = GetComponent<FPCamera>();
    }

    public void ShakeCamera(float duration, float amount = 0f)
    {
        if(duration <= 0f) return;
        StartCoroutine(Shake(duration, amount));
    }

    private IEnumerator Shake(float duration, float amount)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _fpCamera.SetShakeOffset(Random.insideUnitSphere * amount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _fpCamera.SetShakeOffset(Vector3.zero);
    }
}
