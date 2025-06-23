using UnityEngine;
using Michsky.MUIP;

public class SettingsButtons : MonoBehaviour
{
    [SerializeField] private WindowManager myWindowManager;
    [SerializeField] private string windowName = "SettingsWindow";

    public void OpenSettings()
    {
        // Открывает конкретное окно (по имени)
        myWindowManager.OpenWindow(windowName);
    }

    public void CloseSettings()
    {
        // Закрывает текущее активное окно
        myWindowManager.HideCurrentWindow();
    }
}
