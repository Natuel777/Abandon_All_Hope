using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : Entity
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private FPCamera _playerCamera;
    public CameraShake cameraShake;
    [SerializeField] private LayerMask _groundMask, _detectionLayer;
    [SerializeField] private Transform _mesh;
    [SerializeField] private View _view;
    private Rigidbody _rb;
    private Controller _controller;
    private Movement _movement;
    private GroundCheck _groundCheck;
    private FallDamage _fallDamage;
    public IInteractable currentItem;

    [Header("Objets")]
    public Health health;
    public Interaction interaction;

    //Provisional, para la demo
    public CharonLantern lantern;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _movement = new Movement(speed, maxSpeed, _rb, _mesh, _jumpForce);
        health = new Health(data.maxLife);
        _view.Initialize(StartCoroutine, StopCoroutine, health);
        _groundCheck = new GroundCheck(transform, _groundMask, cameraShake);
        _movement.GetGroundCheck(_groundCheck);
        _fallDamage = new FallDamage(_rb, health, _groundCheck, transform, _groundMask);

        if(GameManager.Instance != null)
        {
            GameManager.Instance.player = gameObject;
            GameManager.Instance.playerClass = this;
        }

        LoadCheckpoint();
    }

    private void Start()
    {
        interaction = new Interaction(_detectionLayer, lantern);
        EventManager.Subscribe("PoolWeight", lantern.SetTimerOrb);
        health.RestoreHealth();
        _controller = new Controller(_movement, _playerCamera, transform)
                    .GetInteractionClass(interaction)
                    .GetCurrentInteract(currentItem);
    }

    private void Update() 
    {        
        _controller.ArtificialUpdate();
        _groundCheck.ArtificialUpdate();
        interaction.ArtificialUpdate();
    }

    private void FixedUpdate() 
    {
        _controller.ArtificialFixedUpdate();
        _movement.Move();
        _fallDamage.ArtificialFixedUpdate();
    }

    public void SaveCheckpoint()
    {
        PlayerState state = new PlayerState(health.Life, transform.position);
        CheckpointManager.Save(state);
        Debug.Log("Checkpoint guardado");
    }

    public void LoadCheckpoint()
    {
        PlayerState state = CheckpointManager.Load();
        
        if(state == null) return;

        health.SetHealth(state.health);
        _rb.position = state.position;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        Debug.Log("Respawneado en checkpoint");
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.CompareTag("BGGround"))
        {
            health.ReceiveDamageValue(health.MaxLife);
        }

        if(other.gameObject.name == "MoveablePart" && MazeManager.Instance != null && MazeManager.Instance.data.levelFinished)
        {
            _view.Fade(false, 3f);
            Debug.Log("sorete");
        }
    }

    private void OnDestroy() 
    {
        health.Dispose();
    }
}
