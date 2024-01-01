using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField]private GameObject _hitDisplay;

    public LineRenderer LineRenderer => _lineRenderer;

    public void SetPositions(Vector3[] pos) => _lineRenderer.SetPositions(pos);
    
    public void SetHitPosition(Vector3 pos) => _hitDisplay.transform.position = pos;
    public void SetHitUp(Vector3 up) => _hitDisplay.transform.up = up;
    public void DisplayHit(bool state) => _hitDisplay.SetActive(state);

    public int PositionCount
    {
        get => _lineRenderer.positionCount;
        set => _lineRenderer.positionCount = value;
    }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
}
