using UnityEngine;
using UnityEngine.UI;

public class MenuBackButton : MonoBehaviour
{
    private void Start()
    {
        MenuManager menu = gameObject.GetComponentInParent<MenuManager>();
        
        gameObject.GetComponent<Button>()?.onClick.AddListener(menu.OnBack);
    }
}
