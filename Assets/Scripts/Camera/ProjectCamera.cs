using UnityEngine;

public abstract class ProjectCamera : MonoBehaviour
{
    [SerializeField] protected Transform cameraPosition;
    protected Camera mainCamera;

    protected virtual void Start()
    {
        if(GameManager.Instance != null) GameManager.Instance.SetCursor(false);
        else { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }
        mainCamera = Camera.main;
    }
}
