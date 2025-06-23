using UnityEngine;
using Michsky.MUIP;

public class SettingsButtons : MonoBehaviour
{
    [SerializeField] private WindowManager myWindowManager;
    [SerializeField] private string windowName = "SettingsWindow";

    public void OpenSettings()
    {
        // ��������� ���������� ���� (�� �����)
        myWindowManager.OpenWindow(windowName);
    }

    public void CloseSettings()
    {
        // ��������� ������� �������� ����
        myWindowManager.HideCurrentWindow();
    }
}
