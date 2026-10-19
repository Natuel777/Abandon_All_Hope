using UnityEngine;
using UnityEngine.UI;

public class Note : InteractableObject
{
    [SerializeField] private Sprite noteImage;
    [SerializeField] [TextArea] private string noteText;

    private void Start() 
    {
        SetInteractable(true);
    }

    public override void Interact()
    {
        GameManager.Instance.playerClass.interaction.Disableinteraction();
        UIManager.Instance.OpenNote(noteImage, noteText);
        AudioManager.Instance.PlaySFX(new AudioClipData("Papper"));
    }
    
    public override void SetInteractable(bool value)
    {
        if(value)
        {
            interactable = new InteractableOutline(outline);
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        } 

        else
        {
            interactable = null;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
