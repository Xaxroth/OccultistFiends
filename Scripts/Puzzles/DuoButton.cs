using Input;
using UnityEngine;

public class DuoButton : InteractionObject
{
    public enum ButtonType
    {
        Solo,
        Duo
    };

    public ButtonType Type;

    public bool Timed;
    public bool UseParticleEffect;
    public float TimeOut;
    private float TimeOutTimer;

    [HideInInspector]public bool Interactable = true;
    [HideInInspector]public bool Activated;
    private bool Checked;
    public bool SkillCheck;

    [SerializeField] private ButtonManager _buttonManager;

    public Material ActiveMaterial;

    [SerializeField] private ParticleSystem _pillarSwirl;
    
    [SerializeField] private float ClockTimingWindow = .2f;

    private bool InLoop;
    [HideInInspector]public bool CountingDown;
    
    private float timingClock = 0f;
    [Range(0, 1)]
    public float targetPoint = .5f;

    public float clockSpeed = .5f;
    
    private float p1PressValue = 0;
    private float p2PressValue = 0;

    private bool p1Press;
    private bool p1PressPersistent;
    private bool p2Press;
    private bool p2PressPersistent;

    private UIManager _uiManager;

    private void Awake()
    {
        _buttonManager.Buttons.Add(this);

        _uiManager = UIManager.Instance;
    }

    private void Update()
    {
        if (!SkillCheck && playerInRange)
        {
            Activated = true;
            _buttonManager.CheckSolution();
        }

        if (InLoop)
        {
            TimingLoop();
        }

        if (Activated && ActiveMaterial != null)
        {
            GetComponent<MeshRenderer>().material = ActiveMaterial;
        }
        
        if (!playerInRange && InLoop) return;

        if (!playerInRange) return;

        if (InputManager.Instance.GetInputData(PlayerIndex).CheckButtonPress(ButtonMap.Interact) && Interactable)
        {
            InLoop = true;
            _uiManager.ToggleTimingUI(true);
            _uiManager.InitTimingUI(ClockTimingWindow, targetPoint);

            AnimationController.Instance.SetAnimatorBool(AnimationController.Instance.CultistAnimator, "Interact", true);
			AudioManager.Instance.PlaySoundWorld("Puzzle", transform.position, 15f);
		}

        if (CountingDown)
        {
            StartResetCountdown();
        }
        
        _uiManager.UpdateTimingUI(timingClock);
    }

    void TimingLoop()
    {
        if (InputManager.Instance.GetInputData(PlayerIndex).CheckButtonPress(ButtonMap.JumpOrAim))
        {
            InLoop = false;
            _uiManager.ToggleTimingUI(false);
        }
        
        timingClock -= Time.deltaTime * clockSpeed;
        timingClock = Mathf.Repeat(timingClock, 1f);
        
        var input1 = InputManager.Instance.GetInputData(0);
        var input2 = InputManager.Instance.GetInputData(1);

        p1Press = input1.CheckButtonPress(ButtonMap.Interact);
        p2Press = input2.CheckButtonPress(ButtonMap.Interact);

        if (p1Press)
        {
            p1PressValue = timingClock;
            p1PressPersistent = true;
        }

        if (p2Press)
        {
            p2PressValue = timingClock;
            p2PressPersistent = true;
            AudioManager.Instance.PlaySound("PuzzlePress");
        }

        switch (Type)
        {
            case ButtonType.Solo:
                
                if (!p2PressPersistent) return;
                
                if (IsPointInRange(p2PressValue))
                {
                    Debug.Log("Hit Timing: " + Mathf.Abs(targetPoint) + ", " + p1PressValue + ", " + p2PressValue);
                    p1PressPersistent = false;
                    p2PressPersistent = false;

                    Activated = true;
                    Interactable = false;
                    
                    if (Timed)
                    {
                        TimeOutTimer = TimeOut;
                        CountingDown = true;
                    }
                    
                    InLoop = false;
                    AnimationController.Instance.SetAnimatorBool(AnimationController.Instance.CultistAnimator, "Interact", false);

                    if (UseParticleEffect)
                    {
                        _pillarSwirl.gameObject.SetActive(false);
                    }
                    _uiManager.ToggleTimingUI(false);
                    _buttonManager.CheckSolution();
                }
                else
                {
                    Debug.Log("Missed Timing: " + Mathf.Abs(targetPoint) + ", " + p1PressValue + ", " + p2PressValue);
                    p1PressPersistent = false;
                    p2PressPersistent = false;
                }
                
                break;
            
            case ButtonType.Duo:
                if (!p1PressPersistent || !p2PressPersistent) return;
        
                Debug.Log(p1PressValue - targetPoint + " | " + (p1PressValue - targetPoint - 1));
                Debug.Log(p2PressValue - targetPoint + " | " + (p2PressValue - targetPoint - 1));
                
                if (IsPointInRange(p1PressValue) && IsPointInRange(p2PressValue))
                {
                    Debug.Log("Both Hit Timing: " + Mathf.Abs(targetPoint) + ", " + p1PressValue + ", " + p2PressValue);
                    p1PressPersistent = false;
                    p2PressPersistent = false;
                    Activated = true;
                    Interactable = false;
                    _uiManager.ToggleTimingUI(false);
                    _buttonManager.CheckSolution();
                }
                else
                {
                    Debug.Log("One or Both Missed Timing: " + Mathf.Abs(targetPoint) + ", " + p1PressValue + ", " + p2PressValue);
                    p1PressPersistent = false;
                    p2PressPersistent = false;
                }
                break;
        }
    }

    public void StartResetCountdown()
    {
        TimeOutTimer -= Time.deltaTime;

        if (TimeOutTimer <= 0)
        {
            TimeOutTimer = 0;
            Activated = false;
            Interactable = true;
        }
    }

    public bool IsPointInRange(float pressPoint)
    {
        float halfRange = ClockTimingWindow / 2;
        float startPoint = (targetPoint - halfRange + 1) % 1;
        float endPoint = (targetPoint + halfRange) % 1;

        if (endPoint <= startPoint)
        {
            return pressPoint >= startPoint || pressPoint <= endPoint;
        }
        else
        {
            return pressPoint >= startPoint && pressPoint <= endPoint;
        }
    }
}
