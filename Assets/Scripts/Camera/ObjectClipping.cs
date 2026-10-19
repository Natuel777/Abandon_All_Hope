using UnityEngine;

public class ObjectClipping : ProjectCamera
{
    public Transform weaponTransform;
    public float weaponLength = 0.5f;
    [SerializeField] private float _pushBackAmount = 0.3f;
    [SerializeField] private float _rotationAmount = 30f;
    [SerializeField] private float _smooth = 10f;
    private Vector3 _defaultLocalPos;
    private Quaternion _defaultLocalRot;
    public bool hasObjectEquiped = false;
    private bool _hasCollided = false;

    protected override void Start() 
    {
        base.Start();
        _defaultLocalPos = weaponTransform.localPosition;
        _defaultLocalRot = weaponTransform.localRotation;
    }

    private void Update() 
    {
        if(!hasObjectEquiped) return;

        //Este ray tiene que optimizarse
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward + new Vector3(0.08f,0,0));

        if (Physics.Raycast(ray, out RaycastHit hit, weaponLength, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            _hasCollided = true;
            float t = 1 - (hit.distance / weaponLength); // 0 lejos, 1 pegado
            Vector3 targetPos = _defaultLocalPos + Vector3.back * _pushBackAmount * t;
            Quaternion targetRot = _defaultLocalRot * Quaternion.Euler(-_rotationAmount * t, 0, 0);
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPos, Time.deltaTime * _smooth);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, targetRot, Time.deltaTime * _smooth);
        }

        else if(_hasCollided)
        {
            weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, _defaultLocalPos, Time.deltaTime * _smooth);
            weaponTransform.localRotation = Quaternion.Slerp(weaponTransform.localRotation, _defaultLocalRot, Time.deltaTime * _smooth);
        }
    }
}
