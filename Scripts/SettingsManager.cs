using UnityEngine;
using Michsky.UI.Heat;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private ButtonManager myButton;
    public MenuManagerUI menuManager;
    public ColorButtons colorButtons;

    void Start()
    {
        menuManager = FindFirstObjectByType<MenuManagerUI>();
        colorButtons = FindFirstObjectByType<ColorButtons>();

        // Add button events
        myButton.onClick.AddListener(ClickTest);
        myButton.onHover.AddListener(HoverTest);
        myButton.onLeave.AddListener(LeaveTest);

        // Apply the changes and update the UI
        myButton.UpdateUI();
    }

    void ClickTest()
    {
        if (menuManager != null)
        {
            if (menuManager.isMenuOpen() == false)
                menuManager.ToggleMenu();
            else
                menuManager.ToggleMenu();
        }
    }

    void HoverTest()
    {
        colorButtons.HoverColor();
    }

    void LeaveTest()
    {
        if (!menuManager.isMenuOpen()) // Проверяем, открыто ли меню
            colorButtons.StandartColor();
    }
}
