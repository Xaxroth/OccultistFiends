using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerScript : MonoBehaviour
{
    public static DialogueManagerScript Instance;

    [SerializeField] private GameObject startDialogue;

    [SerializeField] private GameObject panel;

    [SerializeField] private PlayerController _player;

    [SerializeField] private TextMeshProUGUI DialogueBoxTMP;

    [SerializeField] private OptionsLetterByLetter _optionsLetterByLetterScript;

    [SerializeField] private Image _spriteImage;

    [SerializeField] private Sprite _cultist;
    [SerializeField] private Sprite _shadow;
    [SerializeField] private Sprite _creature;

    public LetterByLetter letterByLetterScript;

    public Color CreatureLetterColor;
    public Color CultistLetterColor;
    public Color ShadowLetterColor;

    public GameObject DialogueBox;
    public GameObject[] choiceBoxesButtons;
    public OptionsLetterByLetter[] choiceBoxes;

    public AudioClip dialogue1;
    public AudioClip dialogue2;

    public float dialogueDuration = 21;
    public float optionSpawn = 21;
    public float dialogueReopen = 25;

    public bool isInDialogue;
    private bool setEventFunction = false;
    private bool _inProgress;

    public int currentDialogueNode = 1;
    public int selectedOption;
    public int choicesToBeDisplayed;

    private bool _pullEventActivated;
    public bool _slowDownEventActivated;
    private bool _barrierEventActivated;
    public bool InProgress;

    private int badKarma = 0;
    private int goodKarma = 0;

    [SerializeField] private bool isClosingDialogue;

    private void Awake()
    {
        Instance = this;

        _player = PlayerController.Instance;

        DialogueBoxTMP = DialogueBox.GetComponent<TextMeshProUGUI>();
        letterByLetterScript = DialogueBox.GetComponent<LetterByLetter>();

        Invoke("Event1", 3.0f);
    }

    public void SetColor(Color color)
    {
        DialogueBoxTMP.color = color;
    }

    public void PlayDialogue(string textToBeDisplayed, int numberOfChoices, string choice1, string choice2, string choice3, string choice4)
    {
        panel.SetActive(true);

        choicesToBeDisplayed = numberOfChoices;

        DialogueBox.SetActive(false);

        DialogueBoxTMP.text = textToBeDisplayed;
        letterByLetterScript.fullText = textToBeDisplayed;

        DialogueBox.SetActive(true);
    }


    private void Update()
    {
        if (panel.activeInHierarchy == true)
        {
            isInDialogue = true;
        }
    }

    public void StartOfDialogue()
    {
        InProgress = true;
    }

    public void SetSprite(Sprite image)
    {
        _spriteImage.sprite = image;
    }

    public void EndOfDialogue()
    {
        InProgress = false;
    }

    void CloseDialogue()
    {
        currentDialogueNode = 0;
        panel.SetActive(false);
        DialogueBox.SetActive(false);

        isInDialogue = false;
        isClosingDialogue = false;
    }

    #region Events

    // Regrettably, we are not using ScriptableObjects to handle this data. The events themselves should be handled elsewhere.
    public void Event1()
    {
        if (!_pullEventActivated)
        {
            StartOfDialogue();

            if (!setEventFunction)
            {
               for (int i = 0; i < choiceBoxes.Length; i++)
               {
                   choiceBoxes[i].ButtonFunction.onClick.AddListener(Event1);
               }

               setEventFunction = true;
            }

            switch (currentDialogueNode)
            {
                // ENTRY POINT FOR DIALOGUE
                case 0:
                    Invoke("Event1", 5.0f);
                    SetColor(CultistLetterColor);
                    SetSprite(_cultist);
                    PlayDialogue("Great Old One... I have found you.", 2, null, null, null, null);
                    break;
                    // PLAYERS FIRST CHOICE
                case 1:
                    // WHAT OPTION DID THE PLAYER SELECT?
                    Invoke("Event1", 5.0f);
                    SetColor(CreatureLetterColor);
                    SetSprite(_creature);
                    PlayDialogue("Ehyeeog syha'hnahh mgep Y' mgepfhtagn... Hh... Hhh... H-Help...", 2, null, null, null, null);
                    break;
                // PLAYERS SECOND CHOICE
                case 2:
                    Invoke("Event1", 5.0f);
                    SetColor(CultistLetterColor);
                    SetSprite(_cultist);
                    PlayDialogue("How may I serve?", 1, null, null, null, null);
                    break;
                // PLAYERS THIRD CHOICE
                case 3:
                    Invoke("Event1", 5.0f);
                    SetColor(CreatureLetterColor);
                    SetSprite(_creature);
                    PlayDialogue("...Y' ephaiah mgepna'ah'ehye... Co... Cord...", 1, null, null, null, null);
                    break;
                // FINAL CASE - DIALOGUE IS OVER
                case 4:
                    Invoke("Event2", 5.0f);
                    EndOfDialogue();
                    CloseDialogue();
                    break;
            }


            if (panel.activeInHierarchy)
            {
                currentDialogueNode++;
            }
        }

    }

    public void Event2()
    {
        if (!_pullEventActivated)
        {

            StartOfDialogue();

            if (!setEventFunction)
            {
                for (int i = 0; i < choiceBoxes.Length; i++)
                {
                    choiceBoxes[i].ButtonFunction.onClick.AddListener(Event2);
                }

                setEventFunction = true;
            }

            switch (currentDialogueNode)
            {
                case 0:
                    Invoke("Event2", 5.0f);
                    SetColor(CultistLetterColor);
                    SetSprite(_cultist);
                    PlayDialogue("Why do you not speak, Great One?", 2, null, null, null, null);
                    break;
                case 1:
                    Invoke("Event2", 7.0f);
                    SetColor(CreatureLetterColor);
                    SetSprite(_creature);
                    PlayDialogue("...My... Voo... vooii... voice... St.. stl... stolen.", 2, null, null, null, null);
                    break;
                case 2:
                    Invoke("Event2", 8.0f);
                    PlayDialogue("Aimgr'luh.. Cha..n.. Chanting... Bbb... bre- break... P-... pill... pillars...", 1, null, null, null, null);
                    break;
                case 3:
                    Invoke("Event3", 5.0f);
                    EndOfDialogue();
                    CloseDialogue();
                    break;
            }

            if (panel.activeInHierarchy)
            {
                currentDialogueNode++;
            }
        }
    }

    public void Event3()
    {
        if (!_pullEventActivated)
        {

            StartOfDialogue();

            if (!setEventFunction)
            {
                for (int i = 0; i < choiceBoxes.Length; i++)
                {
                    choiceBoxes[i].ButtonFunction.onClick.AddListener(Event3);
                }

                setEventFunction = true;
            }

            switch (currentDialogueNode)
            {
                case 0:
                    Invoke("Event3", 6.0f);
                    SetColor(CultistLetterColor);
                    SetSprite(_cultist);
                    PlayDialogue("I see them, Great One... But they are out of my reach.", 2, null, null, null, null);
                    break;
                case 1:
                    Invoke("Event3", 3.0f);
                    SetColor(CreatureLetterColor);
                    SetSprite(_creature);
                    PlayDialogue("...C-cord...!", 2, null, null, null, null);
                    break;
                case 2:
                    EndOfDialogue();
                    CloseDialogue();
                    break;
            }

            if (panel.activeInHierarchy)
            {
                currentDialogueNode++;
            }
        }
    }

    public void Event4()
    {
        if (!_pullEventActivated)
        {
            StartOfDialogue();

            if (!setEventFunction)
            {
                for (int i = 0; i < choiceBoxes.Length; i++)
                {
                    choiceBoxes[i].ButtonFunction.onClick.AddListener(Event4);
                }

                setEventFunction = true;
            }

            switch (currentDialogueNode)
            {
                case 0:
                    Invoke("Event4", 2.0f);
                    SetColor(CultistLetterColor);
                    SetSprite(_cultist);
                    PlayDialogue("...", 2, null, null, null, null);
                    break;
                case 1:
                    Invoke("Event4", 3.0f);
                    PlayDialogue("...Wet...", 2, null, null, null, null);
                    break;
                case 2:
                    _pullEventActivated = true;
                    EndOfDialogue();
                    CloseDialogue();
                    break;
            }

            if (panel.activeInHierarchy)
            {
                currentDialogueNode++;
            }
        }
    }

    public void Event5()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event5);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event5", 5.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("...Hmm... Th... Thank you.", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event5", 5.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("This way, great one. Let us proceed.", 2, null, null, null, null);
                break;
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event6()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event6);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event6", 3.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("These bones...", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event6", 7.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("Them... the or-ORDER... Wh-what re-remains of t-them...", 2, null, null, null, null);
                break;
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event7()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event7);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event7", 7.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("These skulls... My strength alone is not enough to disturb them - they will not move!", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event7", 5.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("...I will aid you... Through the cord...", 2, null, null, null, null);
                break;
            case 2:
                Invoke("Event7", 5.0f);
                PlayDialogue("...Quickly... Before they reassemble...", 2, null, null, null, null);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event8()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event8);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event8", 3.0f);
                SetColor(ShadowLetterColor);
                SetSprite(_shadow);
                PlayDialogue("...WHY...", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event8", 2.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("...", 2, null, null, null, null);
                break;
            case 2:
                Invoke("Event8", 5.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("They... awaken...", 2, null, null, null, null);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event9()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event9);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event9", 3.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Another mechanism.", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event9", 2.0f);
                SetColor(CultistLetterColor);
                PlayDialogue("The lock...", 2, null, null, null, null);
                break;
            case 2:
                Invoke("Event9", 2.0f);
                SetColor(CultistLetterColor);
                PlayDialogue("That keyhole looks vaguely...", 2, null, null, null, null);
                break;
            case 3:
                Invoke("Event9", 3.0f);
                SetColor(CultistLetterColor);
                PlayDialogue("'Me' sized...", 2, null, null, null, null);
                break;
            case 4:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event10()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event10);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event10", 3.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("...", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event10", 5.0f);
                PlayDialogue("Steel yourself... They come...", 2, null, null, null, null);
                break;
            case 2:
                Invoke("Event10", 5.0f);
                PlayDialogue("We must make haste!", 2, null, null, null, null);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event11()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event11);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event11", 5.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Great One! These shadows... I'm... stuck!", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event11", 7.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("I feel my strength returning... Stay close, my devoted! I will keep you safe.", 2, null, null, null, null);
                break;
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event12()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event12);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event12", 5.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Fresh air... We are almost there!", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event12", 4.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("It has been long.", 2, null, null, null, null);
                break;
            case 2:
                Invoke("Event12", 7.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("We are so close! Soon, you will be free, Great One!", 2, null, null, null, null);
                break;
            case 3:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event13()
    {
        StartOfDialogue();

        if (!setEventFunction)
        {
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event13);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event13", 4.0f);
                SetColor(CreatureLetterColor);
                SetSprite(_creature);
                PlayDialogue("At long last - the way is opened!", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event13", 5.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Let us enter the surface world!", 2, null, null, null, null);
                break;
            case 2:
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    public void Event14()
    {
        if (!setEventFunction && !_barrierEventActivated)
        {
            StartOfDialogue();
            for (int i = 0; i < choiceBoxes.Length; i++)
            {
                choiceBoxes[i].ButtonFunction.onClick.AddListener(Event14);
            }

            setEventFunction = true;
        }

        switch (currentDialogueNode)
        {
            case 0:
                Invoke("Event14", 2.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Ouch! ...That hurt.", 2, null, null, null, null);
                break;
            case 1:
                Invoke("Event14", 5.0f);
                SetColor(CultistLetterColor);
                SetSprite(_cultist);
                PlayDialogue("Is this barrier reacting to violent movements?", 2, null, null, null, null);
                break;
            case 2:
                _barrierEventActivated = true;
                EndOfDialogue();
                CloseDialogue();
                break;
        }

        if (panel.activeInHierarchy)
        {
            currentDialogueNode++;
        }
    }

    #endregion
}
