using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool _mouseOver = false;
    
    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;
    void OnEnable()
    {
        if (!m_ScrollRect) return;
        m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
    }
    void Awake() => m_ScrollRect = GetComponent<ScrollRect>();

    void Start() 
    {
        if (m_ScrollRect) m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        ScrollToSelected(true);
    }

    void Update() 
    {
        Scroll();
        
        if (!_mouseOver) {
            // Lerp scrolling code.
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, 10f * Time.unscaledDeltaTime);
        }
        else {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }

    void Scroll()
    {
        if (m_Selectables.Count <= 0) return;
        ScrollToSelected(false);
    }

    void ScrollToSelected(bool quickScroll) {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement) {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1) {
            if (quickScroll) {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else {
                m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _mouseOver = true;

    public void OnPointerExit(PointerEventData eventData) 
    {
        _mouseOver = false;
        ScrollToSelected(false);
    }
}
