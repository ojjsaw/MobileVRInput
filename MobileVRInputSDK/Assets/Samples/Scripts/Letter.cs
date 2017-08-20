using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Letter : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        KeyboardInput.instance.curr_key = transform.GetChild(0).GetComponent<Text>().text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        KeyboardInput.instance.curr_key = transform.GetChild(0).GetComponent<Text>().text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (KeyboardInput.instance.curr_key == transform.GetChild(0).GetComponent<Text>().text)
            KeyboardInput.instance.curr_key = "";
    }
}
