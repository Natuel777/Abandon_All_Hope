using UnityEngine;

public abstract class InteractableObject : MonoBehaviour, IInteractable
{
    public InteractableOutline interactable { get; protected set; }
    [SerializeField] protected Outline outline;
    public abstract void Interact();
    public abstract void SetInteractable(bool value);
}
