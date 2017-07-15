using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.Vehicles.Car;

public class UserController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum CarButtonType
    {
        BREAK,
        ACCELERATION,
        NONE
    }

    public CarButtonType controlType = CarButtonType.NONE;
    public CarUserControl CarControl;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(controlType == CarButtonType.ACCELERATION) CarControl.ver = 1;
        else if (controlType == CarButtonType.BREAK) CarControl.ver = -1;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CarControl.ver = 0;

    }

}
