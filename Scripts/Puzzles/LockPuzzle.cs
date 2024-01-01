using Input;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Cinemachine;


public class LockPuzzle : MonoBehaviour, ButtonInterface
{
	[SerializeField]
	private Transform puzzleFrame;

	[SerializeField]
	private PlayerController _player;

	private int lockState;

	public Transform activeSpawnPoint;

	[SerializeField]
	private Transform centerSpawnPoint;

	[SerializeField]
	private Transform leftSpawnPoint;

	[SerializeField]
	private Transform rightSpawnPoint;

	[SerializeField]
	private Transform _returnSpawnPoint;

	[SerializeField]
	private GameObject _lockWall;

	[SerializeField]
	private CinemachineVirtualCamera _puzzleCamera;

	[SerializeField]
	private float spawnSpeed = 10f;

	[SerializeField] private float _rotateSpeed = 2f;

	public bool CanRotateLeft;
	public bool CanRotateRight;

	[SerializeField] private GameObject PromptObject;

	[SerializeField] private GameObject _lockProp;

	public AudioSource _lockPuzzleDone;

	private Vector3 _frameTargetAngles;
	private Vector3 _playerTargetAngles;

	private bool _isRotating;

	[SerializeField] private Animator _animator;

	private void Awake()
    {
		_player = PlayerController.Instance;
	}

    private void OnEnable()
    {
		BeastPlayerController.Instance._isAttached = false;
		FindObjectOfType<PlayerPullie>().DisablePull = true;
		StartCoroutine(FadeCamera());
		_puzzleCamera.enabled = true;
		_player.transform.position = centerSpawnPoint.position;
		_puzzleCamera.Priority = 101;
		BeastPlayerController.Instance.Paralyzed = true;
		TogglePrompts(true);
    }

    private void TogglePrompts(bool value)
    {
	    PromptObject.SetActive(value);
    }

    private void OnDisable()
    {
	    
	}

    public void Activate()
    {
		BeastPlayerController.Instance.Paralyzed = false;
		AudioManager.Instance.PlaySound("PuzzleSolved", 0, 1, 0);
	    _puzzleCamera.enabled = false;
	    _player.transform.position = _returnSpawnPoint.position;
		_lockWall.SetActive(false);
	    _puzzleCamera.Priority = 0;
		BeastPlayerController.Instance.UnlockedPush = true;
		_lockProp.SetActive(false);

		if (!_lockPuzzleDone.isPlaying)
		{
			_lockPuzzleDone.Play();
		}
		else
		{
			_lockPuzzleDone.Stop();
		}

		FindObjectOfType<PlayerPullie>().DisablePull = false;
		BeastPlayerController.Instance._isAttached = true;
		TogglePrompts(false);
		gameObject.SetActive(false);
    }

	private IEnumerator FadeCamera()
    {
		yield return null;
    }


    private void Start()
	{
		lockState = 0;
	}

    void Update()
	{
		if (InputManager.Instance.GetInputData(0).CheckButtonPress(ButtonMap.RotateLeft) && CanRotateLeft && !_isRotating)
		{
			InputManager.Instance.StartRumble(0, .5f, 0, .1f);
			var lockAngles = puzzleFrame.transform.eulerAngles;
			_frameTargetAngles = new Vector3(lockAngles.x, lockAngles.y, lockAngles.z - 90);
				
			var playerAngles = _player.transform.eulerAngles;
			_playerTargetAngles = new Vector3(playerAngles.x, playerAngles.y - 90, playerAngles.z);
			
			lockState--;
			Lockstate();
			AnimationController.Instance.SetAnimatorBool(_animator, "TurnRight", true);
			StartCoroutine(RotateLoop());
			InputManager.Instance.StartRumble(1, 1, 1, 1);
			CanRotateLeft = false;
			CanRotateRight = true;
		}

		if (InputManager.Instance.GetInputData(0).CheckButtonPress(ButtonMap.RotateRight) && CanRotateRight && !_isRotating)
		{
			InputManager.Instance.StartRumble(0, 0, .5f, .1f);
			var lockAngles = puzzleFrame.transform.eulerAngles;
			_frameTargetAngles = new Vector3(lockAngles.x, lockAngles.y, lockAngles.z + 90);

			var playerAngles = _player.transform.eulerAngles;
			_playerTargetAngles = new Vector3(playerAngles.x, playerAngles.y + 90, playerAngles.z); 
			
			lockState++;
			Lockstate();
			AnimationController.Instance.SetAnimatorBool(_animator, "TurnLeft", true);
			StartCoroutine(RotateLoop());
			InputManager.Instance.StartRumble(1, 1, 1, 1);
			CanRotateLeft = true;
			CanRotateRight = false;
		}

		if (activeSpawnPoint == centerSpawnPoint)
		{
			CanRotateLeft = true;
			CanRotateRight = true;
		}
	}

	private void Lockstate()
	{
		Debug.Log(lockState);

		if (lockState == 0)
		{
			activeSpawnPoint = centerSpawnPoint;
		}
		if (lockState == 1)
		{
			activeSpawnPoint = rightSpawnPoint;
		}
		if (lockState == -1)
		{
			activeSpawnPoint = leftSpawnPoint;
		}
		
		AnimationController.Instance.SetAnimatorInt(_animator, "LockState", lockState);
	}

	IEnumerator RotateLoop()
	{
		_isRotating = true;
		float timer = 0;

		Vector3 frameStartAngles = puzzleFrame.eulerAngles;
		Vector3 playerStartAngles = _player.transform.eulerAngles;
		Vector3 playerStartPos = _player.transform.position;

		do
		{
			timer += Time.deltaTime*_rotateSpeed;
			puzzleFrame.eulerAngles = Vector3.Slerp(frameStartAngles, _frameTargetAngles, timer);
			_player.transform.eulerAngles = Vector3.Slerp(playerStartAngles, _playerTargetAngles, timer);
			_player.transform.position = Vector3.Lerp(playerStartPos, activeSpawnPoint.position, timer);
			yield return null;
		} while (timer < 1);
		
		puzzleFrame.eulerAngles = _frameTargetAngles;
		_player.transform.eulerAngles = _playerTargetAngles;
		_player.transform.position = activeSpawnPoint.position;

		_isRotating = false;
		AnimationController.Instance.SetAnimatorBool(_animator, "TurnRight", false);
		AnimationController.Instance.SetAnimatorBool(_animator, "TurnLeft", false);
	}

}


