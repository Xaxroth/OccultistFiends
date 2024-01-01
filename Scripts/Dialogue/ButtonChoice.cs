using Input;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChoice : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private int choiceNumber;

    void Awake()
    {
        button1 = GetComponent<Button>();
    }

    private void Update()
    {
        //bool p1 = InputManager.Instance.GetInputData(0).CheckButtonPress(ButtonMap.Interact);
        //bool p2 = InputManager.Instance.GetInputData(1).CheckButtonPress(ButtonMap.Interact);
        
        //if ((p1 || p2) && DialogueManagerScript.Instance.letterByLetterScript._dialogueFinished)
        //{
        //    DialogueManagerScript.Instance.selectedOption = choiceNumber;
        //    button1.onClick.Invoke();
        //}
    }
}
