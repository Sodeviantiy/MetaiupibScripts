using System;
using UnityEngine;
public class UILoadingView : MonoBehaviour
{
    private void Awake()
    {
        HideLoadingScreen();
    }

    public void ShowLoadingScreen()
    {
        gameObject.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        gameObject.SetActive(false);
    }
}
