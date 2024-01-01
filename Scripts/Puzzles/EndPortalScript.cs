using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPortalScript : MonoBehaviour
{
    [SerializeField] private Image _blackScreen;
    [SerializeField] private GameObject _credits;

    private bool _enteredPortal;

    private bool _fadeIn;

    [SerializeField] private float _fadeSpeed = 2;

    void Awake()
    {
        DialogueManagerScript.Instance.Event13();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_enteredPortal && other.gameObject.GetComponent<BeastPlayerController>())
        {
            StartCoroutine(FadeAlphaToZero(_blackScreen, 4));
            _enteredPortal = true;
        }
    }
        IEnumerator FadeAlphaToZero(Image image, float duration)
        {
            Color32 startColor = image.color;
            Color32 endColor = new Color32(0, 0, 0, 255);
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                image.color = Color.Lerp(startColor, endColor, time / duration);
                yield return null;
            }

            StartCoroutine(FadeBack(image, 2));

        }

        IEnumerator FadeBack(Image image, float duration)
        {
            yield return new WaitForSeconds(2);
            _credits.SetActive(true);
            Cursor.visible = true;
        }
}
