using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomPointerHandler : MonoBehaviour, IPointerDownHandler
{
    public event Action<PointerEventData> OnPointerDownEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(eventData);
    }

    
}
