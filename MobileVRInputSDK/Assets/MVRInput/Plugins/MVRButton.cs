using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MVRInput;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MVRButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private MVRInputManager m_inputmanager = null;
    private MVRController m_controllermanager = null;
    private MVRButtonData m_data = null;
    private int id = -1;
    private GameObject ButtonPrefab = null;
    private Button bttn = null;
    private PointerEventData pointer = null;
    private ConnectionType applicationType = ConnectionType.GAMEPAD;
    private MVRInputConnection m_connection = null;

    public void Initialize(ConnectionType applicationType, MVRInputConnection connection, int elementId = -1)
    {
        this.applicationType = applicationType;
        m_connection = connection;
        id = elementId;

        if (this.applicationType == ConnectionType.APP)
        {
            m_inputmanager = MVRInputManager.instance;
            m_connection.SendToOther(m_connection.ObjectToByteArray(SaveButtonInfo(this.transform, id)));
            pointer = new PointerEventData(EventSystem.current);
        }
        else
        {
            m_controllermanager = MVRController.instance;
        }
    }

    public int GetID()
    {
        return id;
    }

    public void OnReceiveEvent(MVRButtonData data)
    {
        if (applicationType != ConnectionType.APP) return;

        if (data.pressed == 0) ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        else if (data.pressed == 1) ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.pointerDownHandler);
        else if (data.pressed == 2) ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.pointerUpHandler);
        else if (data.pressed == 3) ExecuteEvents.Execute(this.gameObject, pointer, ExecuteEvents.pointerExitHandler);

    }

    public void OnReceiveEvent(MVRButtonInfo info)
    {
        LoadButtonInfo(info);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (applicationType != ConnectionType.GAMEPAD) return;

        m_data.pressed = 0;
        m_connection.SendToOther(m_connection.ObjectToByteArray(m_data));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (applicationType != ConnectionType.GAMEPAD) return;

        m_data.pressed = 1;
        m_connection.SendToOther(m_connection.ObjectToByteArray(m_data));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (applicationType != ConnectionType.GAMEPAD) return;

        m_data.pressed = 2;
        m_connection.SendToOther(m_connection.ObjectToByteArray(m_data));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (applicationType != ConnectionType.GAMEPAD) return;

        m_data.pressed = 3;
        m_connection.SendToOther(m_connection.ObjectToByteArray(m_data));
    }

    private MVRButtonInfo SaveButtonInfo(Transform child, int id)
    {
        MVRButtonInfo mvrButton = new MVRButtonInfo();

        Image img = child.GetComponent<Image>();
        mvrButton.bR = img.color.r;
        mvrButton.bG = img.color.g;
        mvrButton.bB = img.color.b;
        mvrButton.bA = img.color.a;

        Button bttn = child.GetComponent<Button>();
        ColorBlock cb = bttn.colors;
        mvrButton.nR = cb.normalColor.r;
        mvrButton.nG = cb.normalColor.g;
        mvrButton.nB = cb.normalColor.b;
        mvrButton.nA = cb.normalColor.a;

        mvrButton.pR = cb.pressedColor.r;
        mvrButton.pG = cb.pressedColor.g;
        mvrButton.pB = cb.pressedColor.b;
        mvrButton.pA = cb.pressedColor.a;

        mvrButton.dR = cb.disabledColor.r;
        mvrButton.dG = cb.disabledColor.g;
        mvrButton.dB = cb.disabledColor.b;
        mvrButton.dA = cb.disabledColor.a;

        mvrButton.hR = cb.highlightedColor.r;
        mvrButton.hG = cb.highlightedColor.g;
        mvrButton.hB = cb.highlightedColor.b;
        mvrButton.hA = cb.highlightedColor.a;

        mvrButton.cm = cb.colorMultiplier;

        Navigation navigation = child.GetComponent<Button>().navigation;
        navigation.mode = Navigation.Mode.None;
        child.GetComponent<Button>().navigation = navigation;

        //child.GetComponent<Button>().interactable = false;
        //child.GetComponent<Image>().raycastTarget = false;

        RectTransform rt = child.GetComponent<RectTransform>();
        mvrButton.x = rt.localPosition.x;
        mvrButton.y = rt.localPosition.y;
        mvrButton.z = rt.localPosition.z;

        mvrButton.sx = rt.localScale.x;
        mvrButton.sy = rt.localScale.y;
        mvrButton.sz = rt.localScale.z;

        mvrButton.w = rt.sizeDelta.x;
        mvrButton.h = rt.sizeDelta.y;

        mvrButton.text = child.GetComponentInChildren<Text>().text;
        mvrButton.id = id;

        return mvrButton;
    }

    private void LoadButtonInfo(MVRButtonInfo mvrButton)
    {
        GameObject child = this.gameObject;
        child.transform.parent = m_controllermanager.transform;

        child.GetComponent<Image>().color = new Color(mvrButton.bR, mvrButton.bG, mvrButton.bB, mvrButton.bA);

        ColorBlock cb = new ColorBlock();
        cb.normalColor = new Color(mvrButton.nR, mvrButton.nG, mvrButton.nB, mvrButton.nA);
        cb.pressedColor = new Color(mvrButton.pR, mvrButton.pG, mvrButton.pB, mvrButton.pA);
        cb.disabledColor = new Color(mvrButton.dR, mvrButton.dG, mvrButton.dB, mvrButton.dA);
        cb.highlightedColor = new Color(mvrButton.hR, mvrButton.hG, mvrButton.hB, mvrButton.hA);
        cb.colorMultiplier = mvrButton.cm;
        child.GetComponent<Button>().colors = cb;

        Navigation navigation = child.GetComponent<Button>().navigation;
        navigation.mode = Navigation.Mode.None;
        child.GetComponent<Button>().navigation = navigation;

        child.GetComponent<RectTransform>().localPosition = new Vector3(mvrButton.x, mvrButton.y, mvrButton.z);
        child.GetComponent<RectTransform>().localScale = new Vector3(mvrButton.sx, mvrButton.sy, mvrButton.sz);
        child.GetComponent<RectTransform>().sizeDelta = new Vector2(mvrButton.w, mvrButton.h);

        child.GetComponentInChildren<Text>().text = mvrButton.text;
        child.name = mvrButton.id.ToString();
        bttn = child.GetComponent<Button>();
        id = mvrButton.id;

        m_data = new MVRButtonData();
        m_data.id = id;
    }

}
