using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using Input;

public class TrajectoryGenerator : MonoBehaviour
{
    //public bool Moving = false;
    public GameObject HitDisplayObject;
    public bool HitSomething = false;
    public Vector3 Offset = new Vector3(0, 0, 10f);
    public int PositionCount;
    public float rotationSpeed = 100.0f;
    [Range(10, 60f)] public float speed = 10.0f;
    [Range(10, 60f)] public float DefaultSpeed = 30.0f;
    [Range(10, 60f)] public float MinSpeed = 10f;
    [Range(10, 60f)] public float MaxSpeed = 60f;
    [SerializeField] [Range(1, 10f)] private float _yAxisSpeed = 0;
    public float SpeedAddAmount = 0.2f;
    public bool ReverseSpeed;
    public float gravity = 9.8f;
    public LineRenderer lineRenderer;
    public GameObject throwableObject;


    private Vector3 initialForce;
    private Vector3[] positions;
    private bool displayTrajectory = false;

    [SerializeField] private float minPitch = -70, maxPitch = 70;
    private float aimPitch;
    private Vector3 aimForward;
    
    Vector3[] startingLineRendererPoints = null;
    Vector3[] newPointsInLine = null;

    public Vector3 HitPointNormal;
    [SerializeField] private LayerMask EnvironmentLayerMask;
    [SerializeField] private LayerMask ThrowableLayerMask;
    [SerializeField] private CubeBehaviour SmallPlayer;
    [SerializeField] private GameObject HeldObject;
    [SerializeField] private Transform HeldObjectSlot;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVc;
    [SerializeField] private CinemachineVirtualCamera _throwCamera;
    [SerializeField] private CinemachineVirtualCamera _airCamera;
    [SerializeField] private Transform TargetGroup;
    [SerializeField] private Transform LookAtTarget;

    private void Start()
    {
        startingLineRendererPoints = new Vector3[PositionCount];
    }

    public void SwitchToGroupCam()
    {
        Debug.Log("group cam");
        _cinemachineVc.Priority = 100;
        _throwCamera.Priority = 0;
        _airCamera.Priority = 0;
    }

    void SwitchToThrowCam()
    {
        Debug.Log("throw cam");
        _cinemachineVc.Priority = 0;
        _throwCamera.Priority = 100;
        _airCamera.Priority = 0;
    }
    
    void SwitchToAirCam()
    {
        Debug.Log("air cam");
        _cinemachineVc.Priority = 0;
        _throwCamera.Priority = 0;
        _airCamera.Priority = 100;
    }

    void Update()
    {
        float rotation = 0.0f;
        float rotationY = 0.0f;

        var input = InputManager.Instance.GetInputData(0);


        //NonJankyRotation(ref rotation, ref rotationY);
        
        // Apply the rotation to the player
        /*transform.Rotate(new Vector3(0, rotation, 0));
        
        GetComponentInChildren<Camera>().transform.Rotate(new Vector3(rotationY, 0, 0));*/
        
        if (!HeldObject)
        {
            displayTrajectory = false;
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))// && !SmallPlayer.BeingThrown)
            {
                PickUp(SmallPlayer.gameObject);
            }
        }
        else
        {
            if (input.CheckButtonPress(ButtonMap.Interact))
            {
                // Left mouse button pressed, throw the object
                ThrowObject();
            
                /*if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(UnityEngine.Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, ThrowableLayerMask))
                    {
                        HeldObject = hit.transform.gameObject;
                        HeldObject.transform.position = HeldObjectSlot.transform.position;
                        HeldObject.transform.SetParent(HeldObjectSlot);
                        HeldObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }*/
            }
            
            if (input.CheckButtonPress(ButtonMap.JumpOrAim))
            {
                speed = DefaultSpeed;
                SwitchToThrowCam();
                GetComponent<BeastPlayerController>().IsAiming = true;
            }
            
            if (input.CheckButtonHold(ButtonMap.JumpOrAim))
            {
                NonJankyRotation();

                aimForward = Quaternion.Euler(aimPitch, transform.eulerAngles.y, 0) * Vector3.forward;
                
                // Right mouse button pressed, start displaying the trajectory
                displayTrajectory = true;

                if (!ReverseSpeed)
                {
                    speed += SpeedAddAmount * Time.deltaTime;

                    if (speed >= MaxSpeed)
                    {
                        ReverseSpeed = true;
                    }
                }
                else
                {
                    speed -= SpeedAddAmount * Time.deltaTime;
                    
                    if (speed <= MinSpeed)
                    {
                        ReverseSpeed = false;
                    }
                    
                }
                
                InitializeTrajectory();
            }
        }
        
        if (input.GetButtonState(ButtonMap.JumpOrAim) == ButtonState.None && displayTrajectory)
        {
            // Right mouse button released, stop displaying the trajectory
            displayTrajectory = false;
            GetComponent<BeastPlayerController>().IsAiming = false;
            lineRenderer.positionCount = 0;
            if (HeldObject)
            {
                SwitchToGroupCam();   
            }
        }

        if (input.CheckButtonPress(ButtonMap.JumpOrAim))
        {
            HitSomething = false;
        }
        
        if (displayTrajectory)
        {
            // Display the trajectory
            DisplayTrajectory();
        }
        else
        {
            lineRenderer.positionCount = 0;
            HitDisplayObject.SetActive(false);
        }
        
        //Debug.Log("RIGIDBODY VELOCITY" + SmallPlayer.gameObject.GetComponent<Rigidbody>().velocity);
    }

    private void NonJankyRotation()
    {
        var moveDir = InputManager.Instance.GetInputData(0).GetPrimaryVectorInput();

        aimPitch -= moveDir.y * rotationSpeed * Time.deltaTime;
        aimPitch = Mathf.Clamp(aimPitch, minPitch, maxPitch);
    }

    private void PickUp(GameObject target)
    {
        Debug.Log(" ACK PICKUP");
        HeldObject = target;
        HeldObject.transform.position = HeldObjectSlot.transform.position;
        HeldObject.transform.SetParent(HeldObjectSlot);
        HeldObject.GetComponent<Rigidbody>().useGravity = false;
        HeldObject.GetComponent<Rigidbody>().isKinematic = true;
        SmallPlayer.BeingHeld = true;
    }

    private void FixedUpdate()
    {
        if (lineRenderer)
        {
            RaycastHit HitInfo;

            for (int i = 0; i < startingLineRendererPoints.Length - 1; i++)
            {
                if (Physics.Linecast(startingLineRendererPoints[i], startingLineRendererPoints[i + 1], out HitInfo, EnvironmentLayerMask))
                {

                    /*Debug.Log("Line cast between " + i + " " + startingLineRendererPoints[i] + " and " + i + 1 + " " +
                              startingLineRendererPoints[i + 1]);*/

                    //initialize the new array to the furthest point + 1 since the array is 0-based
                    newPointsInLine = new Vector3[(i + 1) + 1];

                    //transfer the points we need to the new array
                    for (int i2 = 0; i2 < newPointsInLine.Length; i2++)
                    {
                        newPointsInLine[i2] = startingLineRendererPoints[i2];
                    }

                    //set the current point to the raycast hit point (the end of the line renderer)
                    newPointsInLine[i + 1] = HitInfo.point;

                    HitPointNormal = HitInfo.normal;

                    //flag that we hit something
                    HitSomething = true;
                    
                    HitDisplayObject.SetActive(true);

                    break;
                }

                HitSomething = false;
                HitDisplayObject.SetActive(false);
            }

            if (newPointsInLine != null)
            {
                HitDisplayObject.transform.position = newPointsInLine[newPointsInLine.Length-1];
                HitDisplayObject.transform.up = HitPointNormal;   
            }
        }
    }

    void InitializeTrajectory()
    {
        // Calculate the initial force based on the player's forward direction
        initialForce = aimForward * speed + (Vector3.up * _yAxisSpeed);

        // Set the number of points in the LineRenderer
        lineRenderer.positionCount = PositionCount;

        // Allocate an array to store the trajectory positions
        positions = new Vector3[lineRenderer.positionCount];
    }

    void DisplayTrajectory()
    {
        // Calculate the trajectory
        Vector3 velocity = initialForce;
        Vector3 position = HeldObjectSlot.transform.position + Offset;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = position;
            startingLineRendererPoints[i] = position;
            velocity += new Vector3(0, -gravity * Time.fixedDeltaTime, 0);
            position += velocity * Time.fixedDeltaTime;
        }

        if (HitSomething)
        {
            //use new points for the line renderer
            lineRenderer.positionCount = newPointsInLine.Length;

            lineRenderer.SetPositions(newPointsInLine);
        }
        else
        {
            //use old points for the line renderer
            lineRenderer.positionCount = startingLineRendererPoints.Length;

            lineRenderer.SetPositions(positions);
        }

        // Update the LineRenderer with the new positions
        lineRenderer.SetPositions(positions);
    }

    void ThrowObject()
    {
        displayTrajectory = false;
        HitDisplayObject.SetActive(false);
        
        SwitchToAirCam();
        
        // Instantiate the throwable object and apply the initial force
        //GameObject go = Instantiate(throwableObject, HeldObjectSlot.transform.position + Offset, transform.rotation);
        Rigidbody rb = HeldObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        HeldObject.GetComponent<Rigidbody>().isKinematic = false;
        //rb.AddForce(transform.forward * speed);
        HeldObject.transform.SetParent(null);
        HeldObject = null;
        rb.velocity = initialForce;
        SmallPlayer.BeingThrown = true;
        SmallPlayer.BeingHeld = false;
    }

    public Vector3 holdPosition => HeldObjectSlot.position;

    public void PickUpPlayer()
    {
        PickUp(SmallPlayer.gameObject);
    }
}
