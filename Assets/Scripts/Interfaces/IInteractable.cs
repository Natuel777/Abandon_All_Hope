public interface IInteractable
{
    public void Interact();
    public void SetInteractable(bool value);
    public InteractableOutline interactable { get; }
}
