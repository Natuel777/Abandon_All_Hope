using UnityEngine;

public class PointDirection
{
    private NeutralNPC _NPCParent;
    private Ray _ray;
    private RaycastHit _hit;
    private float _reach;
    private Interaction _parent;
    private Camera _camera;
    public bool IsPointing {get; private set;}

    public PointDirection(NeutralNPC miser, Interaction interaction, Camera camera)
    {
        _NPCParent = miser;
        _parent = interaction;
        _camera = camera;
        _reach = 150f;
    }

    public void StartRaycasting()
    {
        IsPointing = true;
        _ray = new Ray(_camera.transform.position, _camera.transform.forward);
        
        if(Physics.Raycast(_ray, out RaycastHit hit, _reach))
        {
            _hit = hit;
            _NPCParent.mazeMiser.SetLookAt(_hit.point);
        }
    }

    public void SetHitPoint()
    {
        _NPCParent.mazeMiser.GetDirection(_hit.point);
        _NPCParent.mazeMiser.ConfirmDirection();

        if (_NPCParent.gameObject.name == "Tutorial Miser" && _NPCParent.mazeMiser.currentPushDirection == Vector3.right)
            MazeManager.Instance.tutorial.SetTutorialWeight();

        FingerPoint.Instance.DisableArms();
        _parent.Disableinteraction();
        IsPointing = false;
    }
}
