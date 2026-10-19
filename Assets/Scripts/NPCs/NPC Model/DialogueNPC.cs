using System.Collections;
using UnityEngine;

[System.Serializable]
public struct Dialogue
{
    public string name;
    [TextArea(3,10)]
    public string[] sentences;
}

[System.Serializable]
public class DialogueNPC : IInteractable
{
    private NeutralNPC _parent;
    private NPCRotation _rotation;
    public InteractableOutline interactable => null;
    private Transform _playerPosition;


    public DialogueNPC(NeutralNPC parent, NPCRotation rotation)
    {
        _parent = parent;
        _rotation = rotation;
        _playerPosition = GameManager.Instance.player.transform;
    }

    public void ArtificialUpdate()
    {
        if(DialogueManager.Instance == null || !DialogueManager.Instance.HasActiveDialogue())
            return;

        float distance = Vector3.Distance(_parent.transform.position, _playerPosition.position);
    
        if(DialogueManager.Instance.HasActiveDialogue() && distance > 8f)
        {
            if(DialogueManager.Instance.IsCurrentNPC(_parent))
            {
                if(_parent.isDialoguer)
                    DialogueManager.Instance.EndDialogue();

                else if(_parent.isComentarist)
                    DialogueManager.Instance.EndComment();
            }
        }
    }

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(_parent);
        
        if(_rotation != null) _rotation.GetTarget(GameManager.Instance.player.transform.position);
    }

    public void SetInteractable(bool value)
    {
        //Not needed for DialogueNPC
    }
}
