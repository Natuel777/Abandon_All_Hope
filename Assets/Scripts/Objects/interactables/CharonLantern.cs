using UnityEngine;
using UnityEngine.VFX;

public class CharonLantern : InteractableObject
{
    [SerializeField] private GameObject _lanternLight;
    [SerializeField] private VisualEffect _orb;
    private VisualEffect _timerOrb;
    [SerializeField] private VisualEffect[] _timerOrbs;
    private bool _orbSelected, _orbActive;
    private float _initialCoolDown, _initialSize = 1f;
    private int _currentOrbIndex = 0;
    private string _currentType = "";

    private void Start()
    {
        Player p = GetComponentInParent<Player>();
        if(p != null) p.currentItem = this;
        else if(GameManager.Instance != null) GameManager.Instance.playerClass.currentItem = this;
        if(AudioManager.Instance != null) EventManager.Subscribe("PoolWeight", AudioManager.Instance.PlaySFX);
        EventManager.Subscribe("PoolWeight", SetTimerOrb);
        EventManager.Subscribe("PoolWeight", x => _orbActive = false);
        _timerOrb = _timerOrbs[0];
        SetOrb(false);

        foreach(VisualEffect orb in _timerOrbs){orb.Stop();}
    }

    public override void Interact()
    {
        if(_orbActive)
        {
            switch(_currentOrbIndex)
            {
                case 0:
                    _currentType = "Medium";
                    break;
                case 1:
                    _currentType = "Light";
                    break;
                case 2:
                    _currentType = "Heavy";
                    break;
                default:
                    _currentType = "Light";
                    break;
            }

            MazeManager.Instance.PoolWeight(_currentType);
            return;
        } 

        _lanternLight.SetActive(!_lanternLight.activeInHierarchy);
    }

    private void FixedUpdate()
    {
        OrbTimerUpdater();

        if(!_orbActive) return;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll > 0f)
            ChangeTimerOrb(+1);

        else if(scroll < 0f)
            ChangeTimerOrb(-1);
    }

    private void ChangeTimerOrb(int direction)
    {
        _timerOrb.Stop();
        _currentOrbIndex += direction;

        if(_currentOrbIndex >= _timerOrbs.Length)
            _currentOrbIndex = 0;

        if(_currentOrbIndex < 0)
            _currentOrbIndex = _timerOrbs.Length - 1;

        _timerOrb = _timerOrbs[_currentOrbIndex];
        AudioManager.Instance.PlaySFX(new AudioClipData("WheelSound"));
        _timerOrb.Play();
    }

    private void OrbTimerUpdater()
    {
        if(!_orbSelected || _initialCoolDown <= 0f) return;
        if(GameManager.Instance == null) return;

        float progress = Mathf.Clamp01(1f - (GameManager.Instance.coolDownTimer / _initialCoolDown));
        float newSize = Mathf.Lerp(_initialSize, 0f, progress);
        _timerOrb.SetFloat("Size", newSize);

        if(GameManager.Instance.coolDownTimer <= 0) SetTimerOrb(false);
    }

    public void SetOrb(bool value)
    {
        if(value) _orb.Play();

        else _orb.Stop();
    }

    public void ActivateOrbSelection() 
    {
        _orbActive = true;
        _lanternLight.SetActive(false);
        
        if(_timerOrb != null) _timerOrb.Play();
    }

    public void SetTimerOrb(params object[] parameters)
    {
        foreach(var item in parameters)
        {
            if(item is bool value)
            {
                if(value)
                {
                    _timerOrb.Stop();
                    _orbSelected = true;
                    _timerOrb.Play();
                    _initialCoolDown = GameManager.Instance != null ? Mathf.Max(1f, GameManager.Instance.coolDownTimer) : 1f;
                    _initialSize = _timerOrb.GetFloat("Size");
                }

                else
                {
                    _orbSelected = false;
                    _orbActive = false;
                    _timerOrb.SetFloat("Size", _initialSize);
                    _timerOrb.Stop();
                }

                break;
            }
        }  
    }

    public override void SetInteractable(bool value)
    {
        //Not YET needed for CharonLantern
    }
}
