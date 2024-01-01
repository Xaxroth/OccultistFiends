using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbilicalCord : MonoBehaviour
{
    [SerializeField]private bool isCultist;

    private Transform _pivot;
    private Transform _target;

    [SerializeField]private float _forceSpeed = 5f;

    private ParticleSystem _particleSystem;

    private void Start()
    {
        if (isCultist) _target = BeastPlayerController.Instance.transform;
        else _target = PlayerController.Instance.transform;

        _particleSystem = GetComponent<ParticleSystem>();

        float size = transform.localScale.x;

        var parent = new GameObject();
        parent.transform.SetParent(transform.parent);
        parent.transform.localPosition = transform.localPosition;

        _pivot = parent.transform;
        
        transform.SetParent(null);
        transform.localScale = Vector3.one*size;
    }

    private void Update()
    {
        transform.position = _pivot.position;
        
        Vector3 force = (_target.position - transform.position).normalized*_forceSpeed;

        var velocity = _particleSystem.velocityOverLifetime;

        velocity.x = force.x;
        velocity.y = force.y;
        velocity.z = force.z;
    }

    public void Play()
    {
        _particleSystem.Play();
    }

    public void Pause()
    {
        _particleSystem.Pause();
    }
}
