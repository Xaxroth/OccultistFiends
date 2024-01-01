using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;

public class PillarBehaviour : MonoBehaviour, ButtonInterface
{
    public bool Interactable = true;
    public Transform TopTarget;
    public Transform BottomTarget;
    public Vector3 StartPosition;
    public bool PlayerInRange;
    public float MoveSpeed;
    public bool MovingUp;
    public bool MovingDown;
    public bool Locked;
    public BoxCollider _boundary;

    [SerializeField] private BoxCollider _puzzleBoundary;

    [SerializeField] private ParticleSystem PillarParticles;

    [SerializeField] private ParticleSystem _leftPillarRing;
    [SerializeField] private ParticleSystem _rightPillarRing;

    [SerializeField] private ParticleSystem _corruptRingLeft;
    [SerializeField] private ParticleSystem _corruptRingRight;

    [SerializeField] private ParticleSystem _handTrail;

    [SerializeField] private GameObject ShieldBarrier;



    public AudioSource _creatureInteractPillarPuzzle;
    
    // Start is called before the first frame update
    void Start()
    {
        StartPosition = transform.position;
    }

    private void OnEnable()
    {
        Locked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_leftPillarRing.gameObject.activeInHierarchy)
        {
            _corruptRingLeft.gameObject.SetActive(true);
        }
        if (!_rightPillarRing.gameObject.activeInHierarchy)
        {
            _corruptRingRight.gameObject.SetActive(true);
        }

        if (Locked == true)
        {
            _handTrail.Stop();
        }

        if (PlayerInRange && Interactable)
        {
            if (InputManager.Instance.GetInputData(0).CheckButtonHold(ButtonMap.Interact))
            {
                MovingUp = true;
                MovingDown = false;
                _handTrail.Play();
                BeastPlayerController.Instance.Paralyzed = true;
                AnimationController.Instance.SetAnimatorBool(AnimationController.Instance.CreatureAnimator, "Interact", true);

                PillarParticles.Play();
                _corruptRingLeft.Play();
                _corruptRingRight.Play();



                if (!_creatureInteractPillarPuzzle.isPlaying) 
                {
					_creatureInteractPillarPuzzle.Play();
				}
       
            }
            else
            {
                _handTrail.Stop();
                MovingUp = false;
                MovingDown = true;
                BeastPlayerController.Instance.Paralyzed = false;
                AnimationController.Instance.SetAnimatorBool(AnimationController.Instance.CreatureAnimator, "Interact", false);
                PillarParticles.Stop();
                _corruptRingLeft.Stop();
                _corruptRingRight.Stop();
                _creatureInteractPillarPuzzle.Stop();
			}
        }

        if (MovingUp)
        {
            StartLift();
        }
        else if (MovingDown)
        {
            StartDescent();
        }

        if (transform.position == TopTarget.position && Locked)
        {
            MoveSpeed = 0;
            Interactable = false;
            MovingDown = false;
            MovingUp = false;
            BeastPlayerController.Instance.Paralyzed = false;
        }
    }

    public void StartLift()
    {
        if (MovingUp)
        {
            //fix jank
            transform.position = Vector3.MoveTowards(transform.position, TopTarget.position, MoveSpeed * Time.deltaTime);
        }

        if (transform.position == TopTarget.position)
        {
            MovingUp = false;
        }
    } public void StartDescent()
    {
        if (MovingDown)
        {
            //fix jank
            transform.position = Vector3.MoveTowards(transform.position, BottomTarget.position, (MoveSpeed * Time.deltaTime) * 6);   
        }

        if (transform.position == BottomTarget.position)
        {
			MovingDown = false;
        }
    }

    public void Activate()
    {
        Locked = true;
        _handTrail.Stop();
        DialogueManagerScript.Instance.Event5();
        PillarParticles.Play();
        ShieldBarrier.SetActive(false);
    }
}
