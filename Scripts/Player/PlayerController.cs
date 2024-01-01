using System.Collections;
using UnityEngine;
using Input;
using UnityEngine.Android;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Player Movement")]
    [SerializeField] private float _playerWalkSpeed = 20f;
    [SerializeField] private float _crouchSpeed = 8f;
    [SerializeField] private float turnSpeed = 12f;

    [SerializeField] public GameObject _grippingObject;

    private float _currentPlayerSpeed = 20f;
    private float _playerHeight = 2f;
    private float _crouchHeight = 0.5f;
    [SerializeField] private float _playerMovementMultiplier = 15f;
    private float _playerMovementMultiplierMinValue = 6f;
    private float _playerMovementMultiplierMaxValue = 15f;

    private Vector3 moveDir;

    [Header("Drag")]
    [SerializeField] private float _playerGroundDrag = 15f;
    [SerializeField] private float _playerAirDrag = 2f;
    [SerializeField] private float _airAcceleration = 0.12f;
    private float _playerAirMultiplier = 1f;

    [Header("Slopes and Stairs")]
    private RaycastHit slopeHit;

    [Header("Jumping")]
    [SerializeField] private float _playerJumpHeight = 2f;
    [SerializeField] private float _playerFallSpeed = 1f;

    private Vector3 _jumpDirection = new Vector3(0, 5, 0);
    private Vector3 _fallDirection = new Vector3(0, -1, 0);

    [Header("Conditions")]

    [SerializeField] private float FarGrabThreshold = 30;

    public bool OnGround = false;
    public bool Paralyzed = false;
    public bool BeingThrown = false;
    public bool BeingHeld = false;
    public bool Gripping = false;
    public bool CanGrip = false;

    public bool CloseToCreature = false;

    private bool _canJump = true;
    private bool _soundPlaying = false;
    private bool _playerFalling = false;

    [Header("Audio")]

    private AudioSource _playerAudioSource;

    [SerializeField] private ParticleSystem _particleSystem;

    private LayerMask _layerMask => LayerMask.GetMask("Environment");

    public Animator CultistAnimator;
    public CapsuleCollider PlayerCollider;
    private Transform _groundingTransform;
    private Rigidbody _playerRigidbody;
    private InputManager _inputManager;
    private CameraManager _cameraManager;
    private bool _physicsDisabled = false;
    private const byte PlayerIndex = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        PlayerCollider = GetComponent<CapsuleCollider>();
        CultistAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _currentPlayerSpeed = _playerWalkSpeed;
        _playerAudioSource = gameObject.GetComponent<AudioSource>();
        _playerRigidbody = GetComponentInChildren<Rigidbody>();
        _inputManager = InputManager.Instance;
        _cameraManager = CameraManager.Instance;
        
        _groundingTransform = GetComponentInChildren<GroundCheck>().transform;
    }

    private void Update()
    {
        if (BeingHeld || Gripping)
        {
            Paralyzed = true;
            AnimationController.Instance.SetAnimatorBool(PlayerController.Instance.CultistAnimator, "Hanging", true);
            Physics.IgnoreLayerCollision(gameObject.layer, BeastPlayerController.Instance.gameObject.layer, true);
        }

        Conditions();
        Grip();

        if (Paralyzed || BeingHeld || _physicsDisabled) return;
        Jump();
    }

    private void FixedUpdate()
    {
        if (Paralyzed || BeingHeld || _physicsDisabled) return;
        Movement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (BeingThrown)
        {
            _inputManager.StartRumble(PlayerIndex, .5f, .5f, .2f);
            EffectsManager.Instance.SpawnParticleEffect("CultistImpact", transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySoundWorld("CultistImpact", transform.position, 30f);
            CameraManager.Instance.SwitchToGroupCam(_cameraManager.GetThrowCam());
            Physics.IgnoreLayerCollision(gameObject.layer, BeastPlayerController.Instance.gameObject.layer, false);
        }

        if (collision.collider.CompareTag("Grippable"))
        {
            CanGrip = true;

            _grippingObject = collision.gameObject;

            _inputManager.StartRumble(PlayerIndex, .2f, .2f, .5f);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
        {
            _playerRigidbody.AddForce(collision.contacts[0].normal * 100, ForceMode.Impulse);
            DialogueManagerScript.Instance.Event14();
            Paralyzed = true;
            BeingThrown = false;
            return;
        }

        BeingThrown = false;
        Paralyzed = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_grippingObject == collision.gameObject)
        {
            CanGrip = false;
            _grippingObject = null;
        }
    }

    private void Grip()
    {
        if (_inputManager.GetInputData(PlayerIndex).GetButtonState(ButtonMap.Interact) == ButtonState.Hold && CanGrip)
        {
            _playerRigidbody.velocity = new Vector3(0, 0, 0);
            Gripping = true;
            EnablePhysics(false);
        }
        else
        {
            AnimationController.Instance.SetAnimatorBool(PlayerController.Instance.CultistAnimator, "Hanging", false);
            Gripping = false;
            EnablePhysics(true);
        }
    }

    private void Jump()
    {
        var input = _inputManager.GetInputData(PlayerIndex);
        var camRot = _cameraManager.GetCameraRotation();
        
        if (input.CheckButtonPress(ButtonMap.JumpOrAim) && OnGround && _canJump)
        {
            AudioManager.Instance.PlaySoundWorld(MathHelpers.RandomString("CultistJump1", "CultistJump2", "CultistJump3", "CultistJump4", "CultistJump5"),
                transform.position, 15f);
            EffectsManager.Instance.SpawnParticleEffect("CultistJump", transform.position, Quaternion.identity);
            
            StartCoroutine(JumpCooldown());

            // Nullifies the player's currect velocity as to allow the player to navigate their jumps better.
            // Cancels the current gravitational pull towards the ground, as well as any momentum gained from other means.

            var moveVector = input.GetPrimaryVectorInput();

            Vector3 moveDir = (moveVector.x * (camRot * Vector3.right) + moveVector.y * (camRot * Vector3.forward));

            _playerRigidbody.velocity = new Vector3(0, 0, 0);

            _playerRigidbody.AddForce((moveDir + Vector3.up * 20f) * _playerJumpHeight, ForceMode.Impulse);
            _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed / 15 * moveDir, ForceMode.Impulse);
        }
    }

    private IEnumerator JumpCooldown()
    {
        _canJump = false;
        yield return new WaitForSeconds(0.5f);
        _canJump = true;
    }

    private void Conditions()
    {
        if (OnGround)
        {
            _playerRigidbody.drag = _playerGroundDrag;
        }
        else
        {
            _playerRigidbody.drag = _playerAirDrag;
        }

        if (BeingThrown)
        {
            _playerRigidbody.drag = 0;
            Paralyzed = true;
        }
        else
        {
            Paralyzed = false;
        }

        float distance = Vector3.Distance(BeastPlayerController.Instance.gameObject.transform.position, transform.position);

        if (distance > FarGrabThreshold)
        {
            CloseToCreature = false;
        }
        else
        {
            CloseToCreature = true;
        }
    }

    private void Movement()
    {
        // Returns true/false depending on whether or not a raycast collides with a surface underneath the player.
        // The permitted surfaces are hand-picked through a layermask as to prevent the ray from hitting objects like projectiles.
        
        var input = _inputManager.GetInputData(PlayerIndex);
        var moveVector = _inputManager.GetInputData(PlayerIndex).GetPrimaryVectorInput();
        var camRot = _cameraManager.GetCameraRotation();

        moveDir = camRot * Vector3.forward * moveVector.y + camRot * Vector3.right * moveVector.x;

        // Sets the rotation for the player based on the current angle from the input system, then lerps between the start and the end position.
        if (moveVector != Vector2.zero)
        {
            Vector3 turnDirection = moveDir;
            transform.forward = Vector3.Lerp(transform.forward, turnDirection, Time.deltaTime * turnSpeed);

            if (OnGround)
            {
                AnimationController.Instance.SetAnimatorBool(CultistAnimator, "Running", true);
                _particleSystem.Play();
            }
        }
        else
        {
            AnimationController.Instance.SetAnimatorBool(CultistAnimator, "Running", false);
            _particleSystem.Stop();
        }

        if (OnGround)
        {
            AnimationController.Instance.SetAnimatorBool(CultistAnimator, "Jumping", false);
            // Propels the player in the direction the player is facing; determined by the "direction" transform attached to the player as to prevent jittery camera movement.
            if (moveVector != Vector2.zero)
            {
                if (!OnSlope())
                {
                    _playerRigidbody.AddForce(_playerMovementMultiplier * _currentPlayerSpeed * moveDir, ForceMode.Acceleration);
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

            AnimationController.Instance.SetAnimatorBool(CultistAnimator, "Running", false);

            if (!Gripping)
            {
                _playerRigidbody.AddForce((_currentPlayerSpeed * _airAcceleration) * _playerAirMultiplier * _playerMovementMultiplier * moveDir, ForceMode.Acceleration);
            }

            // As long as the player is not being constricted, they will fall towards the ground at a constant rate.
            if (!Paralyzed && !OnGround)
            {
                if (!BeingHeld && !Gripping)
                {
                    AnimationController.Instance.SetAnimatorBool(CultistAnimator, "Jumping", true);
                    _playerRigidbody.AddForce(_fallDirection * _playerFallSpeed, ForceMode.Impulse);
                }
            }
        }
    }

    private bool OnSlope()
    {
        //todo remove player height variable make it automatic instead to reduce issues
        if (Physics.Raycast(_groundingTransform.position + Vector3.up*.1f, Vector3.down, out slopeHit, 1f, _layerMask))
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

    private Vector3 SlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    public void SlowDown()
    {
        if (_playerMovementMultiplier >= _playerMovementMultiplierMinValue)
        {
            _playerMovementMultiplier--;
            _inputManager.StartRumble(PlayerIndex, .3f, .3f, .1f);
        }
        else
        {
            if (!DialogueManagerScript.Instance._slowDownEventActivated)
            {
                DialogueManagerScript.Instance.Event11();
                DialogueManagerScript.Instance._slowDownEventActivated = true;
            }
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

    public void EnablePhysics(bool state)
    {
        _playerRigidbody.useGravity = state;
        _physicsDisabled = !state;
    }
}
