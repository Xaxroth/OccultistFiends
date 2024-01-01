using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ManagerInstance<UIManager>
{
    [SerializeField] private Transform _timingPointerParent;
    [SerializeField] private Image _rangeImage;
    [SerializeField] private Transform _arrowParent;

    [SerializeField] private Transform _timingUI;

    [SerializeField] private GameObject _tutorial1, _tutorial2, _tutorial3, _tutorial4;

    [SerializeField] private TutorialPopup _throwTutorialUI, _grabTutorialUI, _attackTutorialUI, _lockTutorialUI;
    public enum TutorialType{ThrowTutorial, GrabTutorial, AttackTutorial, LockTutorial}

    public void ToggleTimingUI(bool value)
    {
        _timingUI.gameObject.SetActive(value);
    }

    public void InitTimingUI(float clockTimingWindow, float targetPoint)
    {
        _rangeImage.fillAmount = clockTimingWindow;
        var timingPointerAngle = targetPoint*360f;

        _timingPointerParent.localEulerAngles = new Vector3(0, 0, -timingPointerAngle);
        _rangeImage.transform.localEulerAngles = new Vector3(0, 0, -timingPointerAngle+clockTimingWindow*180f);
    }

    public void UpdateTimingUI(float timingClock)
    {
        var arrowAngle = timingClock*360f;

        _arrowParent.localEulerAngles = new Vector3(0, 0, -arrowAngle);
    }

    public void DisplayTutorial1()
    {
        _tutorial1.SetActive(true);
        _tutorial2.SetActive(false);
        _tutorial3.SetActive(false);
        _tutorial4.SetActive(false);
    }
    
    public void DisplayTutorial2()
    {
        _tutorial1.SetActive(false);
        _tutorial2.SetActive(true);
        _tutorial3.SetActive(false);
        _tutorial4.SetActive(false);
    }
    
    public void DisplayTutorial3()
    {
        _tutorial1.SetActive(false);
        _tutorial2.SetActive(false);
        _tutorial3.SetActive(true);
        _tutorial4.SetActive(false);
    }
    
    public void DisplayTutorial4()
    {
        _tutorial1.SetActive(false);
        _tutorial2.SetActive(false);
        _tutorial3.SetActive(false);
        _tutorial4.SetActive(true);
    }

    public void HideTutorials()
    {
        _tutorial1.SetActive(false);
        _tutorial2.SetActive(false);
        _tutorial3.SetActive(false);
        _tutorial4.SetActive(false);
    }

    private sbyte _lastPriority = -1;
    public void ShowTutorial(TutorialType type, float showTime = 10f, sbyte priority = 0)
    {
        if(_lastPriority > priority) return;

        switch (type)
        {
            case TutorialType.AttackTutorial:
                _attackTutorialUI.ActivateTimed(showTime);
                _throwTutorialUI.Deactivate(false);
                _grabTutorialUI.Deactivate(false);
                _lockTutorialUI.Deactivate(false);
                break;
            case TutorialType.ThrowTutorial:
                _throwTutorialUI.ActivateTimed(showTime);
                _attackTutorialUI.Deactivate(false);
                _grabTutorialUI.Deactivate(false);
                _lockTutorialUI.Deactivate(false);
                break;
            case TutorialType.GrabTutorial:
                _grabTutorialUI.ActivateTimed(showTime);
                _attackTutorialUI.Deactivate(false);
                _throwTutorialUI.Deactivate(false);
                _lockTutorialUI.Deactivate(false);
                break;
            case TutorialType.LockTutorial:
                _lockTutorialUI.ActivateTimed(showTime);
                _grabTutorialUI.Deactivate(false);
                _attackTutorialUI.Deactivate(false);
                _throwTutorialUI.Deactivate(false);
                break;
        }
    }

    public void ResetTutorialPriority()
    {
        _lastPriority = -1;
    }
}
