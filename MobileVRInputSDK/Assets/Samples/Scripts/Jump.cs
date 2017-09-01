using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump : MonoBehaviour, IPointerDownHandler
{

    public Rigidbody sphere;

    public void OnPointerDown(PointerEventData eventData)
    {
        sphere.AddForce(Vector3.up * 250f);
    }
}
