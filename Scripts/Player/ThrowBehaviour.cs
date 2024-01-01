using System.Linq;
using UnityEngine;
using Input;
using System.Collections;
using UnityEngine.Serialization;

public class ThrowBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 10f);
    [SerializeField] private int positionCount;
    [SerializeField] private float pitchChangeSpeed = 100.0f;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float defaultSpeed = 30.0f;
    [SerializeField] private float minSpeed = 10f;
    [SerializeField] private float maxSpeed = 60f;
    [SerializeField] private float yAxisSpeed = 0;
    [SerializeField] private float minPitch = -70, maxPitch = 70;
    [SerializeField] private float speedAddAmount = 0.2f;
    [SerializeField] private TrajectoryLine trajectory;
    [SerializeField] private Transform heldObjectSlot;

    private Vector3 _throwForce;
    private Vector3[] _positions;
    private bool _displayTrajectory = false;
    private bool _reverseSpeed;
    private bool _hitSomething = false;
    private float _aimPitch;
    private Vector3 _aimForward;
    private PlayerController _smallPlayer;
    private Vector3[] _startingLineRendererPoints;
    private Vector3[] _newPointsInLine;
    public Vector3 hitPoint;
    private LayerMask EnvironmentLayerMask => LayerMask.GetMask("Environment","Grippable","Barrier");
    private GameObject _heldObject;

    [SerializeField] private ParticleSystem _spitParticles;

    private const byte PlayerIndex = 0;

    private bool displayTrajectory
    {
        get => _displayTrajectory;
        set
        {
            if (!value)
            {
                trajectory.PositionCount = 0;
                trajectory.DisplayHit(false);
            }
            _displayTrajectory = value;
        }
    }
    
    private void Start()
    {
        _startingLineRendererPoints = new Vector3[positionCount];
        // Set the number of points in the LineRenderer
        trajectory.PositionCount = positionCount;

        // Allocate an array to store the trajectory positions
        _positions = new Vector3[positionCount];
    }

    void Update()
    {
        var input = InputManager.Instance.GetInputData(PlayerIndex);
        var cam = CameraManager.Instance;
        
        if (_heldObject)
        {
            if (input.CheckButtonPress(ButtonMap.Spit))
            {
                ThrowPlayer();
            }
            
            if (input.CheckButtonPress(ButtonMap.JumpOrAim))
            {
                speed = defaultSpeed;
                cam.SwitchToThrowCam();
                GetComponent<BeastPlayerController>().IsAiming = true;

                displayTrajectory = true;
            }

            if (input.CheckButtonHold(ButtonMap.JumpOrAim))
            {
                MovePitch();

                // Right mouse button pressed, start displaying the trajectory
                
                speed += speedAddAmount * Time.deltaTime;
                speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

                /*if (!_reverseSpeed)
                {
                    speed += speedAddAmount * Time.deltaTime;

                    if (speed >= maxSpeed) _reverseSpeed = true;
                }
                else
                {
                    speed -= speedAddAmount * Time.deltaTime;
                    
                    if (speed <= minSpeed) _reverseSpeed = false;

                }*/
                
                _aimForward = Quaternion.Euler(-_aimPitch, transform.eulerAngles.y, 0) * Vector3.forward;
                _aimForward.Normalize();
            }
        }
        
        if (input.GetButtonState(ButtonMap.JumpOrAim) == ButtonState.None && displayTrajectory)
        {
            displayTrajectory = false;
            GetComponent<BeastPlayerController>().IsAiming = false;
            if (_heldObject)
            {
                cam.SwitchToGroupCam(CameraManager.Instance.GetAirCam());   
            }
        }
        
        //Debug.Log(PlayerController.Instance.PlayerCollider.radius);
    }
    
    private void FixedUpdate()
    {
        if (!displayTrajectory) return;

        // Display the trajectory

        _throwForce = _aimForward * speed + (Vector3.up * yAxisSpeed);

        DisplayTrajectory();
        CalculateTrajectoryHit();
    }

    private void MovePitch()
    {
        var moveDir = InputManager.Instance.GetInputData(PlayerIndex).GetPrimaryVectorInput();
        var aimDir = InputManager.Instance.GetInputData(0).GetAimVectorInput();
        
        _aimPitch += aimDir.y * pitchChangeSpeed * Time.deltaTime;
        _aimPitch = Mathf.Clamp(_aimPitch, minPitch, maxPitch);
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, Mathf.Clamp(transform.localEulerAngles.y, minPitch, maxPitch), transform.localEulerAngles.z);
    }

    private void PickUp(GameObject target)
    {
        _heldObject = target;
        _heldObject.transform.position = heldObjectSlot.transform.position;
        _heldObject.transform.SetParent(heldObjectSlot);
        _heldObject.GetComponent<Rigidbody>().useGravity = false;
        _heldObject.GetComponent<Rigidbody>().isKinematic = true;
        _smallPlayer.BeingHeld = true;
    }
    void CalculateTrajectoryHit()
    {
        if (!trajectory) return;

        Vector3 hitNormal = Vector3.up;
        
        for (int i = 0; i < _startingLineRendererPoints.Length - 1; i++)
        {
            //if (Physics.SphereCast(_startingLineRendererPoints[i], 1, Vector3.zero, out RaycastHit hitInfo, EnvironmentLayerMask))
            //{
            //    //initialize the new array to the furthest point + 1 since the array is 0-based
            //    _newPointsInLine = new Vector3[(i + 1) + 1];

            //    //transfer the points we need to the new array
            //    for (int i2 = 0; i2 < _newPointsInLine.Length; i2++)
            //    {
            //        _newPointsInLine[i2] = _startingLineRendererPoints[i2];
            //    }

            //    //set the current point to the raycast hit point (the end of the line renderer)
            //    hitPoint = hitInfo.point;
            //    _newPointsInLine[i + 1] = hitInfo.point;

            //    hitNormal = hitInfo.normal;

            //    //flag that we hit something
            //    _hitSomething = true;
                    
            //    trajectory.DisplayHit(true);

            //    break;
            //}

            Vector3 startPoint = _startingLineRendererPoints[i];
            Vector3 endPoint = _startingLineRendererPoints[i + 1];
            Vector3 direction = endPoint - startPoint;
            float distance = direction.magnitude;
            direction.Normalize();

            RaycastHit hit;

            if (Physics.SphereCast(startPoint, PlayerController.Instance.PlayerCollider.bounds.extents.y * 2, direction, out hit, distance, EnvironmentLayerMask))
            {
                _newPointsInLine = new Vector3[(i + 1) + 1];

                //transfer the points we need to the new array
                for (int i2 = 0; i2 < _newPointsInLine.Length; i2++)
                {
                    _newPointsInLine[i2] = _startingLineRendererPoints[i2];
                }

                //set the current point to the raycast hit point (the end of the line renderer)
                hitPoint = hit.point;
                _newPointsInLine[i + 1] = hit.point;

                hitNormal = hit.normal;

                //flag that we hit something
                _hitSomething = true;

                trajectory.DisplayHit(true);

                break;
            }

            _hitSomething = false;
            trajectory.DisplayHit(false);
        }

        if (!_hitSomething) return;
        trajectory.SetHitPosition(_newPointsInLine[^1]);
        //trajectory.SetHitUp(hitNormal);
    }

    void DisplayTrajectory()
    {
        trajectory.PositionCount = positionCount;
        _positions = new Vector3[trajectory.PositionCount];
        
        // Calculate the trajectory
        Vector3 velocity = _throwForce;
        Vector3 position = heldObjectSlot.transform.position + offset;
        for (int i = 0; i < trajectory.PositionCount; i++)
        {
            _positions[i] = position;
            _startingLineRendererPoints[i] = position;
            
            velocity += Physics.gravity * Time.fixedDeltaTime;
            position += velocity * Time.fixedDeltaTime;
        }

        if (_hitSomething)
        {
            //use new points for the line renderer
            trajectory.PositionCount = _newPointsInLine.Length;

            trajectory.SetPositions(_newPointsInLine);
        }
        else
        {
            //use old points for the line renderer
            trajectory.PositionCount = _startingLineRendererPoints.Length;

            trajectory.SetPositions(_positions);
        }
        
        //trajectory.PositionCount = _startingLineRendererPoints.Length;

        //trajectory.SetPositions(_positions);

        // Update the LineRenderer with the new positions
        trajectory.SetPositions(_positions);
    }

    public void OnDrawGizmos()
    {
        if(_startingLineRendererPoints == null || _startingLineRendererPoints.Length == 0) return;
        
        for (int i = 0; i < _startingLineRendererPoints.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_startingLineRendererPoints[i], PlayerController.Instance.PlayerCollider.bounds.extents.y * 2);

        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint, 1);
    }

    void ThrowPlayer()
    {
        StartCoroutine(ThrowCosmetics());

        displayTrajectory = false;

        //CameraManager.Instance.SwitchToAirCam();
        CameraManager.Instance.SwitchToGroupCam(CameraManager.Instance.GetCurrentCamera());

        Rigidbody rb = _heldObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        _heldObject.GetComponent<Rigidbody>().isKinematic = false;
        _heldObject.transform.SetParent(null);
        _heldObject = null;

        _smallPlayer.Paralyzed = true;
        _smallPlayer.BeingThrown = true;
        _smallPlayer.BeingHeld = false;
        
        rb.velocity = _throwForce;
        GetComponent<BeastPlayerController>().IsAiming = false;
    }

    private IEnumerator ThrowCosmetics()
    {
        BeastPlayerController.Instance.Paralyzed = true;
        AnimationController.Instance.SetAnimatorInt(BeastPlayerController.Instance.CreatureAnimator, "MovementMagnitude", 0);
        AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "Spit", true);
        AudioManager.Instance.PlaySoundWorld("Spit", _heldObject.transform.position, 30f);
        EffectsManager.Instance.SpawnParticleEffect("CreatureSpit", _heldObject.transform.position, _heldObject.transform.rotation);
        yield return new WaitForSeconds(0.5f);
        BeastPlayerController.Instance.Paralyzed = false;
        AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "Spit", false);

    }

    public Vector3 HoldPosition => heldObjectSlot.position;

    public void PickUpPlayer(PlayerController smallPlayer)
    {
        _smallPlayer = smallPlayer;
        PickUp(_smallPlayer.gameObject);
    }
}
