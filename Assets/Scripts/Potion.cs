using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Potion : MonoBehaviour, IBeginDragHandler, IEndDragHandler,IDragHandler
{
    public Color Color;
    public PotionSlot PotionSlot;

    private UnityEvent _disableEvent;
    private UnityEvent _enableEvent;
    private UnityEvent _decreaseEvent;

    private int _count = 0;

    [SerializeField] private TextMeshProUGUI _counter;

    public int GetCount()
    {
        return _count;
    }

    public void SetCount(int count)
    {
        _count = count;
        _counter.text = $"{_count}";
    }

    public void IncreaseCount()
    {
        _count++;
        _counter.text = $"{_count}";
    }

    public void DestroyPotion()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _enableEvent.Invoke();
        _decreaseEvent.Invoke();
    }

    public void AddEvents(UnityEvent disableEvent, UnityEvent enableEvent, UnityEvent decreaseEvent)
    {
        _disableEvent = disableEvent;
        _enableEvent = enableEvent;
        _decreaseEvent = decreaseEvent;

        _disableEvent.AddListener(DisableRaycastTarget);
        _enableEvent.AddListener(EnableRaycastTarget);
    }

    public void DisableRaycastTarget()
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void EnableRaycastTarget()
    {
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _disableEvent.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _enableEvent.Invoke();   
    }
}
