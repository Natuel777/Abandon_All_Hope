using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    private Movement _movement;
    private FPCamera _camera;
    private Transform _playerTransform;
    private Interaction _interaction;
    private IInteractable _currentInteract;
    private bool _isDead = false, _isCrouching = false;

    public Controller(Movement movement, FPCamera camera, Transform playerTransform)
    {
        _movement = movement;
        _camera = camera;
        _playerTransform = playerTransform;
        SetDeathInput(false);
        EventManager.Subscribe("OnPlayerDeath", SetDeathInput);
    }

    public void ArtificialUpdate()
    {
        bool noteOpen = UIManager.Instance != null && UIManager.Instance.noteActive;

        if(!_isDead && !noteOpen)
        {
            SetCameraInputs();
            GetDialogueInput();
            SetPointing();

            if(Input.GetKeyDown(KeyCode.E))
                GetInteractInput();

            if(Input.GetKeyDown(KeyCode.F) && MazeManager.Instance != null && MazeManager.Instance.hasMazeStarted)
                GetPoolInput();

            if(Input.GetKeyDown(KeyCode.Escape))
                GetPauseInput();

            if(Input.GetKeyDown(KeyCode.Mouse0) && _currentInteract != null)
                GetActionInput();
        }

        else if(noteOpen) GetNoteInput();

        else GetDeathInput();
    }

    public void ArtificialFixedUpdate()
    {
        if(_isDead) return;

        GetMovementInputs();

        if(Input.GetKey(KeyCode.Space) && !_isCrouching && !_interaction.hasInteracted) 
            GetJumpInput();
    }

    //WASD
    private void GetMovementInputs()
    {
        float _inputX = Input.GetAxisRaw("Horizontal");
        float _inputY = Input.GetAxisRaw("Vertical");
        float inputMagnitude = new Vector3(_inputX, 0 , _inputY).magnitude;
        _camera.StartHeadBob(0f);

        if(Input.GetKey(KeyCode.LeftShift) && !_isCrouching && _inputY > 0)
        {
            _movement.SetMovementInputs(_inputX * 0.5f, _inputY, _playerTransform, true, _isCrouching);

            if(_movement.readyToJump)
                _camera.StartHeadBob(inputMagnitude, true);

            else _camera.StartHeadBob(0f);
        }

        else if(_isCrouching)
        {
            _movement.SetMovementInputs(_inputX, _inputY, _playerTransform, false, _isCrouching);

            if(_movement.readyToJump)
                _camera.StartHeadBob(inputMagnitude);

            else _camera.StartHeadBob(0f);
        }
                    
        else
        {
            _movement.SetMovementInputs(_inputX, _inputY, _playerTransform);

            if(_movement.readyToJump)
                _camera.StartHeadBob(inputMagnitude);

            else _camera.StartHeadBob(0f);
        }
    }

    //Mouse
    private void SetCameraInputs()
    {
        float _mouseX = Input.GetAxisRaw("Mouse X");
        float _mouseY = Input.GetAxisRaw("Mouse Y");
        _camera.GetInputs(_mouseX, _mouseY);
    }

    //Space (Dialogue)
    private void GetDialogueInput()
    {
        if(_interaction.hasInteracted)
        {
            if(Input.GetKeyDown(KeyCode.Space) && DialogueManager.Instance.HasActiveDialogue()) 
                DialogueManager.Instance.DisplayNextSentence();
        }
    }

    //Space
    private void GetJumpInput()
    {
        _movement.Jump(_playerTransform);
    }

    //E
    private void GetInteractInput()
    {
        _interaction.StartInteraction();
    }

    //Right Mouse Button
    private void SetPointing()
    {
        if(_interaction.pointDirection != null)
        {
            if(Input.GetKeyDown(KeyCode.Mouse1)) FingerPoint.Instance.SetIKTarget();
                
            if(Input.GetKey(KeyCode.Mouse1))
            {
                _interaction.pointDirection.StartRaycasting();
            }

            if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                _interaction.pointDirection.SetHitPoint();
            }
        }
    }

    //R
    private void GetDeathInput()
    {
        if(Input.GetKeyDown(KeyCode.R) && GameManager.Instance != null)
            GameManager.Instance.RestartScene();
    }

    //Revisar Eventmanager
    private void SetDeathInput(params object[] parameters)
    {
        foreach(var item in parameters)
        {
            if(item is bool value) _isDead = value;
        }
    }

    //F
    private void GetPoolInput()
    {
        if(GameManager.Instance != null) GameManager.Instance.playerClass.lantern.ActivateOrbSelection();
        if(AudioManager.Instance != null) AudioManager.Instance.PlaySFX(new AudioClipData("FSound"));
    }

    //ESC
    private void GetPauseInput()
    {
        UIManager.Instance.SetPauseMenu();
    }

    //LEFT MOUSE BUTTON
    private void GetActionInput()
    {
        _currentInteract.Interact();
    }

    //Any Key
    private void GetNoteInput()
    {
        if(Input.anyKeyDown)
            UIManager.Instance.CloseNote();
    }

    public Controller GetInteractionClass(Interaction x)
    {
        _interaction = x;
        return this;
    }

    public Controller GetCurrentInteract(IInteractable interactable)
    {
        _currentInteract = interactable;
        return this;
    }
}