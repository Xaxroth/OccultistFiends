using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsLetterByLetter : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;
    private string currentText = "";

    public Button ButtonFunction;
    private TextMeshProUGUI TMP;
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
        if (fullText != null)
        {
            for (int i = 0; i < (fullText.Length + 1); i++)
            {
                currentText = fullText.Substring(0, i);
                TMP.text = currentText;
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
