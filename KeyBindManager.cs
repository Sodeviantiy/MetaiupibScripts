using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyBindManager : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public TMP_Text up, down, left, right, jump;
    private GameObject currentKey;
    private Color32 normal = new Color32(255, 255, 255, 104);
    private Color32 selected = new Color32(255, 255, 255, 145);


    void Start()
    {
        keys.Add("Up", KeyCode.W);
        keys.Add("Down", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);

        UpdateKeyText();
    }

    void UpdateKeyText()
    {
        up.text = keys["Up"].ToString();
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
    }

    public KeyCode GetKey(string action)
    {
        return keys[action];
    }

   
    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = e.keyCode.ToString();
                currentKey.GetComponent<Image>().color = normal;
                currentKey = null;
                UpdateKeyText();
            }
        }
    }
    

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
            currentKey.GetComponent<Image>().color = normal;

        currentKey = clicked;
        currentKey.GetComponent<Image>().color = selected;
    }
}
