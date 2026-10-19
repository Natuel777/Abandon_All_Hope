using System.Collections.Generic;
using UnityEngine;

public class NeutralNPC : NPC
{
    private List<IInteractable> _interactableAssets = new();
    public bool isDialoguer, isMazeMiser, isComentarist;
    public MazeMiser mazeMiser;
    public DialogueNPC dialoguer;
    public NPCRotation idleRotation;
    public NPCMovement movement;
    public Dialogue dialogue;

    [Header("Maze Miser Settings")]
    //Despues lo corrigjo
    [SerializeField] private Transform _trailSpawnPoint;

    protected override void Start() 
    {
        base.Start();

        if(isInteractable) SetInteractable(isInteractable);

        if(canRotate) idleRotation = new NPCRotation(transform, data.NPCRotationSpeed);

        if(canMove) movement = new NPCMovement(transform, data.NPCRotationSpeed);

        if(isMazeMiser)
        {
            SetMazeMiser(true);

            if(this.gameObject.name != "Tutorial Miser")
                gameObject.SetActive(false);
            
            else
            {
                isMazeMiser = false;
                mazeMiser = null;
                _interactableAssets.Remove(mazeMiser);
            }
        } 

        if(isDialoguer || isComentarist)
        {
            SetDialoguer(true);
        } 
    }

    private void Update()
    {
        if(idleRotation != null && idleRotation.target != Vector3.zero) idleRotation.ArtificialUpdate();

        if(movement != null) movement.ArtificialUpdate();

        if(dialoguer != null) dialoguer.ArtificialUpdate();
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

    public override void Interact()
    {
        foreach(IInteractable item in _interactableAssets)
        {
            item.Interact();
        }

        if(this.gameObject.name == "Tutorial Starter") 
        {
            Objective current = ObjectiveManager.Instance.GetActiveObjective();
            
            if(current != null && current.Text == "Vuelve a hablar con el alma en el umbral de la escalinata")
                ObjectiveManager.Instance.CompleteObjective();

            if(GameManager.Instance.mainCamera.TryGetComponent<FPCamera>(out FPCamera fpCamera))
                fpCamera.waypointMarker.SetTarget(null);
        }
    }

    public void SetDialoguer(bool value)
    {
        if(value)
        {
            dialoguer = new DialogueNPC(this, idleRotation);
            _interactableAssets.Add(dialoguer);
        }

        else 
        {
            dialoguer = null;
            _interactableAssets.Remove(dialoguer);
        }
    }

    public void SetMazeMiser(bool value)
    {
        if(value)
        {
            mazeMiser = new MazeMiser(idleRotation, transform, _trailSpawnPoint);
            _interactableAssets.Add(mazeMiser);
            MazeManager.Instance.mazeMisers.Add(this);
        }

        else
        {
            mazeMiser = null;
            _interactableAssets.Remove(mazeMiser);
            MazeManager.Instance.mazeMisers.Remove(this);
        }
    }
}
