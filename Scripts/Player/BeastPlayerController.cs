using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Input;

public class BeastPlayerController : MonoBehaviour
{

    public static BeastPlayerController Instance;

    [Header("Player Movement")]
    [SerializeField] private float _playerWalkSpeed = 20f;
    [SerializeField] private float _crouchSpeed = 8f;
    [SerializeField] private float turnSpeed = 12f;
    [SerializeField] private float aimTurnSpeed = 12f;

    public float _currentPlayerSpeed = 10f;
    private float _playerHeight = 2f;
    private float _crouchHeight = 0.5f;
    private float _playerMovementMultiplier = 15f;
    private float _playerMovementMultiplierMinValue = 6f;
    private float _playerMovementMultiplierMaxValue = 15f;

    private Vector3 _moveDir;

    [Header("Player Cord")]
    [SerializeField] public LineRenderer _playerCord;
    [SerializeField] public bool _isAttached = true;
    [SerializeField] private float _maxDistanceBetweenPlayers;

    [Header("Drag")]
    [SerializeField] private float _playerGroundDrag = 15f;
    [SerializeField] private float _playerAirDrag = 2f;
    private float _airAcceleration = 0.12f;
    private float _playerAirMultiplier = 1f;

    [Header("Slopes and Stairs")]
    private RaycastHit slopeHit;

    [Header("Push Ability")]
    public GameObject PushCosmetic;
    [SerializeField] private float _pushCooldown = 2f;
    [SerializeField] private float _shoutRadius = 15f;
    [SerializeField] private bool _canPush = true;
    private float _pushForce;
    private Vector3 _pushForceDirection = new Vector3(0, 0, -5);
    [SerializeField] private float _pushForceMultiplier = 1;

    [Header("Jumping")]
    [SerializeField] private float _playerJumpHeight = 2f;
    [SerializeField] private float _playerFallSpeed = 1f;

    private Vector3 _jumpDirection = new Vector3(0, 5, 0);
    private Vector3 _fallDirection = new Vector3(0, -1, 0);

    [Header("Conditions")]

    public bool OnGround = false;
    public bool Paralyzed = false;
    public bool BeingThrown = false;
    public bool BeingHeld = false;
    public bool IsAiming = false;

    public bool UnlockedPush = false;
    private bool _canJump = true;
    private bool _soundPlaying = false;
    private bool _playerFalling = false;

    [Header("Audio")]

    private AudioSource _playerAudioSource;
    private LayerMask _layerMask => LayerMask.GetMask("Environment");
    private LayerMask _pushLayerMask => LayerMask.GetMask("Enemy");

    private Rigidbody _playerRigidbody;
    public Animator CreatureAnimator;
    private PlayerController _playerController;
    private InputManager _inputManager;
    private CameraManager _cameraManager;
    private bool _physicsDisabled = false;
    private const byte PlayerIndex = 0;

    private AnimationController _animationController;

    private Transform _groundingTransform;

    private float _gurgleTimer;
    private float _gurgleTimestamp;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        CreatureAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        _currentPlayerSpeed = _playerWalkSpeed;
        _playerAudioSource = gameObject.GetComponent<AudioSource>();
        _playerRigidbody = GetComponentInChildren<Rigidbody>();
        _animationController = AnimationController.Instance;
        _playerController = PlayerController.Instance;
        _inputManager = InputManager.Instance;
        _cameraManager = CameraManager.Instance;
        CreatureAnimator = GetComponent<Animator>();
        _groundingTransform = GetComponentInChildren<GroundCheck>().transform;

        _gurgleTimestamp = Random.Range(1f, 5f);
    }

    private void Update()
    {
        _gurgleTimer += Time.deltaTime;

        if (_gurgleTimer >= _gurgleTimestamp)
        {
            AudioManager.Instance.PlayAmbientWorld(MathHelpers.RandomString("Gurgle1", "Gurgle2", "Gurgle3", "Gurgle4"), transform.position, 15f);
            _gurgleTimestamp = Random.Range(1f, 5f);
            _gurgleTimer = 0;
        }

        if (Paralyzed)
        {
            _playerCord.enabled = false;
        }
        else
        {
            Conditions();   
        }

        if (Paralyzed || BeingHeld || _physicsDisabled) return;
        Push();
    }

    private void FixedUpdate()
    {
        if (Paralyzed || BeingHeld || _physicsDisabled) return;
        Movement();
    }

    private IEnumerator JumpCooldown()
    {
        _canJump = false;
        yield return new WaitForSeconds(0.5f);
        _canJump = true;
    }

    private bool _attachedOnce = true;
    private void Conditions()
    {
        if (_isAttached)
        {
            if (_attachedOnce)
            {
                foreach (var umbilicalCord in FindObjectsOfType<UmbilicalCord>())
                {
                    umbilicalCord.Play();
                }

                _attachedOnce = false;
            }

            _playerCord.enabled = true;
            _playerCord.SetPosition(0, transform.position);
            _playerCord.SetPosition(1, _playerController.transform.position);

            float distance = (transform.position - _playerController.transform.position).magnitude;

            if (distance >= _maxDistanceBetweenPlayers)
            {
                _playerController.Paralyzed = true;
                _playerController.transform.position = Vector3.MoveTowards(_playerController.transform.position, transform.position, 15f * Time.deltaTime);
            }
            else if (distance <= _maxDistanceBetweenPlayers && !_playerController.BeingThrown && !_playerController.BeingHeld)
            {
                _playerController.Paralyzed = false;
            }

            if (OnGround)
            {
                _playerRigidbody.drag = _playerGroundDrag;
            }
            else
            {
                _playerRigidbody.drag = _playerAirDrag;
            }
        }
        else if(!_attachedOnce)
        {
            foreach (var umbilicalCord in FindObjectsOfType<UmbilicalCord>())
            {
                umbilicalCord.Pause();
            }

            _attachedOnce = true;
        }
    }

    private void Movement()
    {
        // Returns true/false depending on whether or not a raycast collides with a surface underneath the player.
        // The permitted surfaces are hand-picked through a layermask as to prevent the ray from hitting objects like projectiles.
        
        var input = _inputManager.GetInputData(PlayerIndex);
        var moveVector = _inputManager.GetInputData(PlayerIndex).GetPrimaryVectorInput();
        var camRot = _cameraManager.GetCameraRotation();

        _moveDir = camRot * Vector3.forward * moveVector.y + camRot * Vector3.right * moveVector.x;
        var _moveDirF = camRot * Vector3.forward * moveVector.y;

        // Sets the rotation for the player based on the current angle from the input system, then lerps between the start and the end position.
        if (moveVector != Vector2.zero && !Paralyzed) //todo turning breaks when in aim mode rework turning function
        {
            RotatePlayer(_moveDir);
            AnimationController.Instance.SetAnimatorBool(AnimationController.Instance.CreatureAnimator, "Interact", false);
            _animationController.SetAnimatorInt(CreatureAnimator, "MovementMagnitude", (int)moveVector.magnitude);
        }
        else
        {
            _animationController.SetAnimatorInt(CreatureAnimator, "MovementMagnitude", (int)moveVector.magnitude);
        }

        if (OnGround)
        {
            // Propels the player in the direction the player is facing; determined by the "direction" transform attached to the player as to prevent jittery camera movement.
            if (moveVector == Vector2.zero) return;
            
            var slopeMoveDirY = _moveDirF + SlopeMovementDirection();
                
            if (!OnSlope())
            {
                if (IsAiming)
                {
                    _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed * _moveDirF, ForceMode.Acceleration);
                }
                else
                {
                    _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed * _moveDir, ForceMode.Acceleration);
                }
            }
            else
            {
                if (IsAiming)
                {
                    _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed * slopeMoveDirY, ForceMode.Acceleration);
                }
                else
                {
                    _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed * SlopeMovementDirection(), ForceMode.Acceleration);
                }
            }
        }
        else
        {
            // Since the player is airborne, their base movement values must be reduced as to compensate for the lower Rigidbody drag values to create consistent speed.

            _playerRigidbody.AddForce((_currentPlayerSpeed * _airAcceleration) * _playerAirMultiplier * _playerMovementMultiplier * _moveDir, ForceMode.Acceleration);

            // As long as the player is not being constricted, they will fall towards the ground at a constant rate.
            if (!Paralyzed && !OnGround)
            {
                if (!BeingHeld)
                {
                    _playerRigidbody.AddForce(_fallDirection * _playerJumpHeight / 2f, ForceMode.Impulse);
                }
            }
        }
    }

    private void RotatePlayer(Vector3 moveVector)
    {
        moveVector.Normalize();
        float dot = Vector3.Dot(transform.forward, moveVector);
        
        if (IsAiming && dot < -.8f)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(moveVector.x, moveVector.z) * Mathf.Rad2Deg;

        float newAngle;

        newAngle = IsAiming ? Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * aimTurnSpeed) 
            : Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSpeed);

        transform.eulerAngles = new Vector3(0, newAngle, 0);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(_groundingTransform.position + Vector3.up*.5f, Vector3.down, out slopeHit, 10f, _layerMask))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void SlowDown()
    {
        if (_playerMovementMultiplier >= _playerMovementMultiplierMinValue)
        {
            _playerMovementMultiplier--;
            _inputManager.StartRumble(PlayerIndex, .3f, .3f, .1f);
        }
    }

    public void RestoreSpeed()
    {
        if (_playerMovementMultiplier <= _playerMovementMultiplierMaxValue)
        {
            _playerMovementMultiplier++;
        }

        if (_playerMovementMultiplier >= _playerMovementMultiplierMaxValue)
        {
            _playerMovementMultiplier = _playerMovementMultiplierMaxValue;
        }
    }

    private void Push()
    {
        var input = _inputManager.GetInputData(PlayerIndex);

        if (input.CheckButtonPress(ButtonMap.Shout) && _canPush && UnlockedPush)
        {
            AnimationController.Instance.SetAnimatorBool(CreatureAnimator, "MagicAoE", true);

            _inputManager.StartRumble(PlayerIndex, .3f, .3f, .3f);


            StartCoroutine(PushCosmeticCoroutine());
            StartCoroutine(PushCooldown());
        }
    }

    private IEnumerator PushCosmeticCoroutine()
    {
        Paralyzed = true;
        EffectsManager.Instance.SpawnParticleEffect("CreatureShout", transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySoundWorld("Gurgle4", transform.position, 60f, 0, .8f);
        yield return new WaitForSeconds(1.25f);
        Paralyzed = false;
        AnimationController.Instance.SetAnimatorBool(CreatureAnimator, "MagicAoE", false);

            Collider[] targetsHit = Physics.OverlapSphere(transform.position, _shoutRadius, _pushLayerMask);

            foreach (Collider enemyCollider in targetsHit)
            {
                    float distanceToTarget = Vector3.Distance(transform.position, enemyCollider.transform.position);
                    distanceToTarget /= _shoutRadius;
                    float pushForce = _pushForceMultiplier / distanceToTarget;

                   _pushForceDirection = (enemyCollider.transform.position - transform.position).normalized * pushForce;

                    enemyCollider.GetComponent<ShadowEnemyBehaviour>().StunEnemy(_pushForceDirection);
            }
    }

    private IEnumerator PushCooldown()
    {
        _canPush = false;
        yield return new WaitForSeconds(_pushCooldown);
        _canPush = true;
    }

    private Vector3 SlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(_moveDir, slopeHit.normal).normalized;
    }

    public void EnablePhysics(bool state)
    {
        _playerRigidbody.useGravity = state;
        _physicsDisabled = !state;
    }
}
