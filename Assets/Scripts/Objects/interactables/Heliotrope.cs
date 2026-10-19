using UnityEngine;

public class Heliotrope : InteractableObject
{
    private void Start() 
    {
        SetInteractable(true);
    }

    public override void Interact()
    {
        GameManager.Instance.playerClass.interaction.Disableinteraction();
        GameManager.Instance.ActiveOrDesactive(this.gameObject, false);
        AudioManager.Instance.PlaySFX(new AudioClipData("item-select"));
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
