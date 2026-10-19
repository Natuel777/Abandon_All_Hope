using UnityEngine;

public class InteractableOutline
{
    private Outline _outline;
    //La existencia de este bool privado es redunddate, verdad?
    private bool _interactPressed = false;

    public InteractableOutline(Outline outline)
    {
        _outline = outline;
        DisableOutLine();
    }

    public void DisableOutLine()
    {
        _outline.enabled = false;
    }

    public void EnableOutLine()
    {
        if(!_interactPressed) _outline.enabled = true;
    }

    public InteractableOutline GetInteractionBool(bool x)
    {
        _interactPressed = x;
        return this;
    }
}
