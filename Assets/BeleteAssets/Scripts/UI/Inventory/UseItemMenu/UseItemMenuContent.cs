using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UseItemMenuContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item item { get; set; }
    public delegate void PointerHandlerDel(PointerEventData eventData);
    public PointerHandlerDel WhenPointerEnter;
    public PointerHandlerDel WhenPointerExit;
    public PointerHandlerDel WhenPointerClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        WhenPointerClick?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WhenPointerEnter?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WhenPointerExit?.Invoke(eventData);
    }
}
