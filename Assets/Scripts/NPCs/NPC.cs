using UnityEngine;

public abstract class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline outline;
    public InteractableOutline interactable { get; protected set; }
    [SerializeField] protected bool isInteractable, canMove, canRotate;
    [SerializeField] protected EntityDataSO data;
    [SerializeField] protected int startAnimIndex;
    [SerializeField] protected Animator animator;

    protected virtual void Start() 
    {
        animator.SetInteger("StartAnimIndex", startAnimIndex);
    }

    public abstract void Interact();
    public abstract void SetInteractable(bool value);
}
