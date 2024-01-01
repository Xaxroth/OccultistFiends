using System.Collections.Generic;
using Input;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Tooltip("0 is opened by default")]
    [SerializeField] private SubMenu[] menus;

    [SerializeField] private bool closeOnStart = true;
    [SerializeField] private bool isMainMenu = false;

    private int _currentMenu = 0;
    private Stack<int> _menuHistory = new();
    private bool _menuOpen;

    private void OnEnable()
    {
        InputManager.OnUIBack += OnBack;
        InputManager.OnUIMenu += OnMenu;
    }

    private void OnDisable()
    {
        InputManager.OnUIBack -= OnBack;
        InputManager.OnUIMenu -= OnMenu;
    }

    private void Start()
    {
        foreach (var menu in menus)
        {
            menu.gameObject.SetActive(true);
        }
        
        FindObjectOfType<SettingsMenu>()?.Setup();
        
        SetMenu(0);

        if (closeOnStart) OpenMenu(false);
        else OpenMenu(true);
    }

    public void SetMenu(int index) => SetMenu(index, true);

    private void SetMenu(int index, bool saveLast)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].gameObject.SetActive(i == index);
            if(i == index) menus[i].SelectFirst(); 
        }

        index.ClampZero();

        if(_currentMenu != index && saveLast) 
            _menuHistory.Push(_currentMenu);
        
        _currentMenu = index;
    }

    public void OpenMenu(bool state)
    {
        if (!state)
            SetMenu(-1, false);
        else
            SetMenu(0);

        _menuOpen = state;

        Time.timeScale = state ? 0 : 1f;
    }

    public void OnBack()
    {
        if(!_menuOpen || !_menuHistory.TryPeek(out int result)) return;

        SetMenu(_menuHistory.Pop(), false);
    }

    void OnMenu()
    {
        if(isMainMenu) return;
        
        if (_menuOpen && _currentMenu == 0)
        {
            OpenMenu(false);
            return;
        }

        if (!_menuOpen)
        {
            OpenMenu(true);
            return;
        }
    }

    public void Restart()
    {
        GameSceneManager.Instance.ReloadScene();
    }

    public void LoadScene(string sceneName)
    {
        GameSceneManager.Instance.LoadScene(sceneName);
    }

    public void LoadSceneIndex(int sceneIndex)
    {
        GameSceneManager.Instance.LoadSceneIndex(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
