using UnityEngine;
using UnityEngine.UI;

public class ColorButtons : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image bg;

    private Color32 NormalWhite = new Color32(245, 245, 245, 255);
    private Color32 NormalBlue = new Color32(9, 65, 142, 255);

    private Color32 AlphaBlue = new Color32(97, 144, 208, 255);

    public void StandartColor()
    {
        icon.GetComponent<Image>().color = NormalBlue;
        bg.GetComponent<Image>().color = NormalWhite;
    }

    public void HoverColor()
    {
        icon.GetComponent<Image>().color = NormalWhite;
        bg.GetComponent<Image>().color = AlphaBlue;
    }

    public void ClickColor()
    {
        icon.GetComponent<Image>().color = NormalWhite;
        bg.GetComponent<Image>().color = NormalBlue;
    }
}
