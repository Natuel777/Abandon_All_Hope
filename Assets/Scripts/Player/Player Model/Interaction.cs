using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction 
{
    //Implementar EventManger

    private float _reach;
    private IInteractable _currentinteractable;
    public PointDirection pointDirection;
    public bool hasInteracted;
    private Camera _camera;
    private LayerMask _detectionLayer;
    private CharonLantern _lantern;

    public Interaction(LayerMask layer, CharonLantern lantern)
    {
        _reach = 3f;
        _camera = Camera.main;
        _detectionLayer = layer;
        _lantern = lantern;
    }

    public void ArtificialUpdate()
    {
        CheckInteraction();
    }

    public void StartInteraction()
    {
        if (hasInteracted) return;

        if(_currentinteractable != null)
        {
            hasInteracted = true;
            _currentinteractable.Interact();

            if(_currentinteractable is NeutralNPC neutral)
            {
                _currentinteractable.interactable?.GetInteractionBool(hasInteracted);
                _lantern.SetOrb(true);

                if(neutral.isMazeMiser)
                {
                    pointDirection = new PointDirection(neutral, this, Camera.main);
                    return;
                }

                if(neutral.gameObject.name == "Tutorial Starter")
                {
                    MazeManager.Instance.EnableTutorial();
                    return;
                }

                return;
            }
        }
    }

    private void CheckInteraction()
    {
        if(!hasInteracted)
        {
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, _reach, _detectionLayer))
            {
                if(hit.collider.TryGetComponent<IInteractable>(out IInteractable obj))
                {
                    _currentinteractable = obj;

                    if(obj is MonoBehaviour mb && mb.enabled && obj.interactable != null)
                    {
                        obj.interactable.EnableOutLine();  

                        if(obj is NPC npc && npc.interactable != null)
                        {
                            if(npc is NeutralNPC neutral && neutral.isMazeMiser)
                            {
                                if(!UIManager.Instance.isFading)
                                {
                                    if(!UIManager.Instance.controls.text.Contains("Presiona [E]"))
                                        UIManager.Instance.controls.text = "Presiona [E] para interactuar.\n\n Mantén [Click Derecho] para dictar una dirección de empuje.";
                                    
                                    UIManager.Instance.FadeCanvasGroup(UIManager.Instance.controlsGroup, true, false, 1f);
                                }
                            }
                        }
                    }
                
                    else Disableinteraction();
                }

                else Disableinteraction();
            }

            else Disableinteraction();
        }
    }

    public void Disableinteraction()
    {
        if(_currentinteractable == null) return;
        
            hasInteracted = false;
            _lantern.SetOrb(false);

            if(_currentinteractable is NPC npc && npc.interactable != null)
            {
                npc.interactable.GetInteractionBool(hasInteracted);

                if(npc is NeutralNPC neutral && neutral.isMazeMiser)
                {
                    UIManager.Instance.FadeCanvasGroup(UIManager.Instance.controlsGroup, false, false, 1f);
                }
            }

            _currentinteractable.interactable?.DisableOutLine();
            _currentinteractable = null;

        if(pointDirection != null)
        {
            pointDirection = null;
        }
    }
}
