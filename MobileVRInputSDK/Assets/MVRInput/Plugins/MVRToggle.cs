using UnityEngine;
using UnityEngine.EventSystems;

public class MVRToggle : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (transform.parent.parent.GetComponent<CanvasGroup>().alpha == 0)
            transform.parent.parent.GetComponent<CanvasGroup>().alpha = 1;
        else
            transform.parent.parent.GetComponent<CanvasGroup>().alpha = 0;
    }

}
