using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public GameObject player, mainCamera;
    public Player playerClass;
    public float coolDownTimer = 0;
    public bool IsPaused = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if(player == null) player = FindFirstObjectByType<Player>()?.gameObject;
        if(player != null) playerClass = player.GetComponent<Player>();
        Time.timeScale = 1f;
    }

    private void FixedUpdate() 
    {
        if(coolDownTimer > 0) coolDownTimer -= Time.deltaTime;
    }

    public void SetCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetPauseState(bool paused)
    {
        IsPaused = paused;
        SetCursor(paused);
        Time.timeScale = paused ? 0 : 1f;
    }

    public void DestroyObject(GameObject item)
    {
        Destroy(item);
    }
    
    public void DestroyComponent(Object component)
    {
        Destroy(component);
    }

    public void ActiveOrDesactive(GameObject obj, bool value)
    {
        obj.SetActive(value);
    }

    public void RestartScene()
    {
        EventManager.Clear();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
