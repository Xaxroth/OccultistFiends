using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterByLetter : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;
    public string currentText = "";

    public bool _dialogueFinished;

    TextMeshProUGUI TMP;
    float currentTime;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        _dialogueFinished = false;

        StartCoroutine(TextSoundTest());

        if (!_dialogueFinished)
        {
            for (int i = 0; i < (fullText.Length + 1); i++)
            {
                if (_dialogueFinished)
                {
                    break;
                }
                AudioManager.Instance.PlaySound("DialogueText", 0, 1, 0);
                currentText = fullText.Substring(0, i);
                TMP.text = currentText;
                yield return new WaitForSeconds(delay);

                if (i == fullText.Length)
                {
                    _dialogueFinished = true;
                    currentText = fullText;

                    for (int j = 0; j < DialogueManagerScript.Instance.choicesToBeDisplayed; j++)
                    {
                        DialogueManagerScript.Instance.choiceBoxes[j].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private IEnumerator TextSoundTest()
    {
        yield return new WaitForSeconds(3);
        if (!_dialogueFinished)
        {
            StartCoroutine(TextSoundTest());
        }
    }

    private void Update()
    {
        bool p1Hold = InputManager.Instance.GetInputData(0).GetButtonState(ButtonMap.Interact) == ButtonState.Hold;
        bool p2Hold = InputManager.Instance.GetInputData(1).GetButtonState(ButtonMap.Interact) == ButtonState.Hold;
        if ((p1Hold || p2Hold) && !_dialogueFinished)
        {
            _dialogueFinished = true;
            currentText = fullText;
            TMP.text = fullText;

            for (int j = 0; j < DialogueManagerScript.Instance.choicesToBeDisplayed; j++)
            {
                DialogueManagerScript.Instance.choiceBoxes[j].gameObject.SetActive(true);
            }
        }
    }
}
