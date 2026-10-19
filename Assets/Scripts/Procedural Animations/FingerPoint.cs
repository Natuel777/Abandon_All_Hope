using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FingerPoint : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint _leftArmIK;
    public static FingerPoint Instance {get; private set;}
    private bool _hasAlreadyPointed = false;
    [SerializeField] private Transform _pointDirection;
    [SerializeField] private GameObject _arms;

    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetIKTarget()
    {
        GameManager.Instance.ActiveOrDesactive(_arms, true);

        if(!_hasAlreadyPointed)
        {
            _leftArmIK.data.target = _pointDirection;
            _leftArmIK.weight = 1f;
            _leftArmIK.gameObject.GetComponentInParent<RigBuilder>().Build();
            _hasAlreadyPointed = true;
        }
    }

    public void DisableArms()
    {
        if(_arms.activeInHierarchy) GameManager.Instance.ActiveOrDesactive(_arms, false);
    }
}
