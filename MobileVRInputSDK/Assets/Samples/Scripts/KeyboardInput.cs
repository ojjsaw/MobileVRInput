using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInput : MonoBehaviour, IPointerDownHandler
{
    public static KeyboardInput instance = null;

    public string curr_key = "";
    public TextMesh mytext = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (curr_key == "del")
        {
            if (mytext.text.Length > 0 && mytext.text != "Enter Text...")
                mytext.text = mytext.text.Remove(mytext.text.Length - 1);

            if (mytext.text.Length == 0)
                mytext.text = "Enter Text...";

        }
        else
        {
            if(mytext.text == "Enter Text...") mytext.text = curr_key;
            else mytext.text += curr_key;
        }

        curr_key = "";
    }
}
