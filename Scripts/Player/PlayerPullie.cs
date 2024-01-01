using System;
using Input;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerPullie : MonoBehaviour
{
    private InputManager _inputManager;

    [SerializeField] private float PullSpeed = 5f;
    [SerializeField] private float RotationSpeed = 4f;

    private ThrowBehaviour _bigPlayer;
    private PlayerController _playerController;

    public bool DisablePull = false;
    
    public bool Turning = false;

    private bool _eventPlayed;

    private void Awake()
    {
        _inputManager = InputManager.Instance;
        _bigPlayer = FindObjectOfType<ThrowBehaviour>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Turning)
        {
            var lookPos = transform.position - BeastPlayerController.Instance.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            BeastPlayerController.Instance.transform.rotation = Quaternion.Slerp(BeastPlayerController.Instance.transform.rotation, rotation, Time.deltaTime * RotationSpeed);
        }

        if (DisablePull) return;
        
        var input = _inputManager.GetInputData(1);

        if (input.CheckButtonPress(ButtonMap.PullBack) && !_beingYanked && !(_playerController.BeingHeld || _playerController.BeingThrown))
        {
            if (!_eventPlayed && !DialogueManagerScript.Instance.InProgress)
            {
                DialogueManagerScript.Instance.Event4();
            }
            InputManager.Instance.StartRumble(1, .2f, .2f, .5f);
            AudioManager.Instance.PlaySoundWorld("Pullback", BeastPlayerController.Instance.transform.position, 15f);
            _playerController.EnablePhysics(false);

            _yankSpeedMultiplier = 1f;
            _yankTimer = 0;
            _beingYanked = true;

            if (_playerController.CloseToCreature)
            {
                AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "GrabClose", true);
                //BeastPlayerController.Instance.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - BeastPlayerController.Instance.transform.position);
            }
            else
            {
                AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "GrabFar", true);
            }

        }

        if (_beingYanked)
        {
            StartCoroutine(DevourPlayer());
        }


    }

    private IEnumerator DevourPlayer()
    {
        Turning = true;
        BeastPlayerController.Instance.Paralyzed = true;
        AnimationController.Instance.SetAnimatorInt(BeastPlayerController.Instance.CreatureAnimator, "MovementMagnitude", 0); 
        yield return new WaitForSeconds(0.5f);
        BeastPlayerController.Instance.Paralyzed = false;
        YankPlayer();
    }

    private bool _beingYanked;

    private float _yankTimer = 0f;
    private float _yankSpeedMultiplier = 1f;
    void YankPlayer()
    {
        _yankTimer += Time.deltaTime;
        
        transform.position = Vector3.MoveTowards(transform.position, _bigPlayer.HoldPosition, PullSpeed*_yankSpeedMultiplier*Time.deltaTime);

        _yankSpeedMultiplier = 1+Mathf.Pow(_yankTimer, .4f)+_yankTimer*.2f;

        if (Vector3.Distance(transform.position, _bigPlayer.HoldPosition) < .1f)
        {
            _playerController.EnablePhysics(true);
            _beingYanked = false;
            _bigPlayer.PickUpPlayer(_playerController);
            Turning = false;

            AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "GrabFar", false);
            AnimationController.Instance.SetAnimatorBool(BeastPlayerController.Instance.CreatureAnimator, "GrabClose", false);
        }
    }
}
