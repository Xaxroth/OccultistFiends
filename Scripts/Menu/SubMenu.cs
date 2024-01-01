using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour
{
    [Tooltip("The button the eventsystem selects on menu change")]
    [SerializeField] private Selectable firstSelect;

    private void OnEnable()
    {
        SelectFirst();
    }

    public void SelectFirst()
    {
        EventSystem eventSystem = EventSystem.current;
        
        eventSystem.SetSelectedGameObject(firstSelect.gameObject);
        firstSelect.OnSelect(new BaseEventData(EventSystem.current));
    }
}