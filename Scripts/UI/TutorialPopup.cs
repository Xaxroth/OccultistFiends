using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialPopup : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private bool _active = false;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Deactivate(true);
    }

    public void Activate(bool instant)
    {
        if (instant)
        {
            _canvasGroup.alpha = 1;
            _active = true;
            return;
        }
        
        if(_active) return;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void Deactivate(bool instant)
    {
        if (instant)
        {
            _canvasGroup.alpha = 0;
            _active = false;
            return;
        }
        
        if(!_active) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    public void ActivateTimed(float time)
    {
        if(_active) return;
        StopAllCoroutines();
        StartCoroutine(FadeInOut(time));
    }

    IEnumerator FadeInOut(float time)
    {
        _active = true;
        float timer = 0f;

        do
        {
            timer += Time.deltaTime * 3f;
            _canvasGroup.alpha = timer;
            yield return null;
        } while (timer < 1);

        _canvasGroup.alpha = 1;
        timer = 1;

        yield return new WaitForSeconds(time);

        _active = false;
        
        do
        {
            timer -= Time.deltaTime * 3f;
            _canvasGroup.alpha = timer;
            yield return null;
        } while (timer > 0);

        _canvasGroup.alpha = 0;
        UIManager.Instance.ResetTutorialPriority();
    }

    IEnumerator FadeIn()
    {
        _active = true;
        float timer = 0f;

        do
        {
            timer += Time.deltaTime * 3f;
            _canvasGroup.alpha = timer;
            yield return null;
        } while (timer < 1);

        _canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        _active = false;
        float timer = 1f;
        
        do
        {
            timer -= Time.deltaTime * 3f;
            _canvasGroup.alpha = timer;
            yield return null;
        } while (timer > 0);

        _canvasGroup.alpha = 0;
    }
}
