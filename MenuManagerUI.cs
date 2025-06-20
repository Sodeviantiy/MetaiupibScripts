using UnityEngine;

public class MenuManagerUI : MonoBehaviour
{
    public GameObject menuPanel;
    private bool isMenuActive = false;
    private ColorButtons colorButtons;

    void Start()
    {
        colorButtons = FindFirstObjectByType<ColorButtons>();
        menuPanel.SetActive(false);
    }

    //void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Escape))        
    //        ToggleMenu();
    //}

    public void OpenMenu()
    {
        isMenuActive = true;
        menuPanel.SetActive(isMenuActive);
        Cursor.visible = true;
        colorButtons.ClickColor();
    }

    public void CloseMenu()
    {
        isMenuActive = false;
        menuPanel.SetActive(false);
        Cursor.visible = false;
        colorButtons.StandartColor();
    }

    public bool isMenuOpen()
    {
        return isMenuActive;
    }

    public void ToggleMenu()
    {
        if (isMenuActive == false)
            OpenMenu();
        else
            CloseMenu();
    }
}
