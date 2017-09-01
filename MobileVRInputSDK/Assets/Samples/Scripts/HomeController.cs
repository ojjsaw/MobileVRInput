using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour, IPointerDownHandler
{
    public enum Panels
    {
        PANEL1, PANEL2, PANEL3, PANEL4, NONE
    }

    private Panels currPanel = Panels.NONE;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch(currPanel)
        {
            case Panels.PANEL1:
                MVRInputManager.instance.DisableAllconfiguration();
                SceneManager.LoadScene("CarRacing");
                break;
            case Panels.PANEL2:
                MVRInputManager.instance.DisableAllconfiguration();
                SceneManager.LoadScene("Candy");
                break;
            case Panels.PANEL3:
                MVRInputManager.instance.DisableAllconfiguration();
                SceneManager.LoadScene("VirtualKeyboard");
                break;
            case Panels.PANEL4:
                MVRInputManager.instance.DisableAllconfiguration();
                SceneManager.LoadScene("VirtualJoystick");
                break;
            default:
                break;
        }
    }

    public void Panel1()
    {
        currPanel = Panels.PANEL1;
    }

    public void Panel2()
    {
        currPanel = Panels.PANEL2;
    }

    public void Panel3()
    {
        currPanel = Panels.PANEL3;
    }

    public void Panel4()
    {
        currPanel = Panels.PANEL4;
    }

    public void PanelExit()
    {
        currPanel = Panels.NONE;
    }
}
