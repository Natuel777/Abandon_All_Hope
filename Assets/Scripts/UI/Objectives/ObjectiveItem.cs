using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectiveItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _icon;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Objective _objective;

    public void Initialize(Objective obj)
    {
        _objective = obj;
        _text.text = obj.Text;
        _icon.enabled = obj.IconEnabled;
    }

    public void MarkAsCompleted()
    {
        UIManager.Instance.FadeCanvasGroup(_canvasGroup, false, false, 3f, true);
    }
}