using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class WaypointMarker
{
    [SerializeField] private Image _img;
    [SerializeField] private Transform _target;

    public void SetTarget(params object[] parameters)
    {
        Transform previous = _target;

        if(parameters != null)
        {
            foreach(var item in parameters)
            {
                if(item is Transform t)
                {
                    if(!_img.gameObject.activeInHierarchy)
                        GameManager.Instance.ActiveOrDesactive(_img.gameObject, true);
                    _target = t;
                    break;
                } 
            }
        }

        if(_target == previous)
        {
            GameManager.Instance.ActiveOrDesactive(_img.gameObject, false);
            _target = null;
        } 
    }

    public void ArtificialUpdate()
    {
        if(_target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);

        if(screenPos.z < 0)
        {
            GameManager.Instance.ActiveOrDesactive(_img.gameObject, false);
            return;
        }

        else
        {
            if(!_img.gameObject.activeSelf)
                GameManager.Instance.ActiveOrDesactive(_img.gameObject, true);
        }

        _img.transform.position = screenPos;
    }
}
