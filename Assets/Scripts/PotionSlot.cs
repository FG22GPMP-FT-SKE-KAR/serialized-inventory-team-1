using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PotionSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private PotionCollection _potionCollection;

    private Potion _currentPotion;

    public void OnDrop(PointerEventData eventData)
    {
        Potion potion = eventData.pointerDrag.GetComponent<Potion>();

        // Lets the user switch the potions between slots
        if(_currentPotion != null)
        {
            _currentPotion.transform.position = potion.PotionSlot.transform.position;
            _currentPotion.PotionSlot = potion.PotionSlot;
            potion.PotionSlot.SetPotion(_currentPotion);
        }

        else
        {
            potion.PotionSlot.DetachPotion();
        }

        potion.transform.position = transform.position;
        potion.PotionSlot = this;
        SetPotion(potion);
    }

    public void DetachPotion()
    {
        _currentPotion = null;
    }

    public void DestroyPotion()
    {
        _currentPotion.DestroyPotion();
        _currentPotion = null;
    }

    public void SetPotion(Potion potion)
    {
        _currentPotion = potion;
    }

    public Potion GetPotion()
    {
        return _currentPotion;
    }

    public bool HasPotion()
    {
        if(_currentPotion != null)
        {
            return true;
        }

        return false;
    }
}
