using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler, IMoveHandler
{
    private bool isButton;

    private void Awake()
    {
        isButton = GetComponent<Button>();
    }

    public void OnMove(AxisEventData data)
    {
        if(isButton) return;
        if (data.moveDir is MoveDirection.Down or MoveDirection.Left) PlayDecrease();
        else PlayIncrease();
    }

    public void OnPointerClick(PointerEventData data) => PlayPress();

    public void OnPointerEnter(PointerEventData data) => PlaySelect();

    public void OnSelect(BaseEventData data) => PlaySelect();

    public void OnSubmit(BaseEventData data) => PlayPress();

    private void PlaySelect()
    {
        AudioManager.Instance.PlaySound("UISelect");
    }

    private void PlayPress()
    {
        AudioManager.Instance.PlaySound("UIPress");
    }

    private void PlayIncrease()
    {
        AudioManager.Instance.PlaySound("UIPress");
    }

    private void PlayDecrease()
    {
        AudioManager.Instance.PlaySound("UIPress");
    }
}
