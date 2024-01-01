using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : ManagerInstance<CameraManager>
{
    private Quaternion _yawRotation;

    private CinemachineBrain _cinemachineBrain;
    [SerializeField] private CinemachineVirtualCamera groupCam;
    [SerializeField] private CinemachineVirtualCamera throwCam;
    [SerializeField] private CinemachineVirtualCamera airCam;

    private void Start()
    {
        _cinemachineBrain = GetComponent<CinemachineBrain>();
        SwitchToGroupCam(groupCam);
    }

    private void Update()
    {
        _yawRotation = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
    }

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return (CinemachineVirtualCamera)_cinemachineBrain.ActiveVirtualCamera;
    }
    
    public void SwitchToGroupCam(CinemachineVirtualCamera previousCam)
    {
        previousCam.Priority = 0;
        groupCam.Priority = 100;
        throwCam.Priority = 0;
        airCam.Priority = 0;
    }

    public void SwitchToThrowCam()
    {
        groupCam.Priority = 0;
        throwCam.Priority = 100;
        airCam.Priority = 0;
    }
    
    public void SwitchToAirCam()
    {
        groupCam.Priority = 0;
        throwCam.Priority = 0;
        airCam.Priority = 100;
    }
    
    public void SetGroupCam(CinemachineVirtualCamera newGroupCam)
    {
        groupCam = newGroupCam;
    }
    
    public CinemachineVirtualCamera GetGroupCam()
    {
        return groupCam;
    }
    
    public CinemachineVirtualCamera GetAirCam()
    {
        return airCam;
    }
    
    public CinemachineVirtualCamera GetThrowCam()
    {
        return throwCam;
    }

    public void SwitchActiveCam(CinemachineVirtualCamera newCamera)
    {
        newCamera.Priority = 100;
        groupCam.Priority = 0;
        throwCam.Priority = 0;
        airCam.Priority = 0;
    }

    public Quaternion GetCameraRotation() => _yawRotation;
}
