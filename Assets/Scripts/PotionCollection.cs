using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PotionCollection : MonoBehaviour, IDropHandler
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private int _waitTime;

    private UnityEvent _disableEvent;
    private UnityEvent _enableEvent;
    private UnityEvent _decreaseEvent;

    private DateTime _nextDateTime;

    private string filePath;

    [System.Serializable]
    public class PotionSave
    {
        public bool[] HasPotion;
        public string[] Color;
        public int[] Count;
        public string NextDateTime;

        public PotionSave(bool[] hasPotion, string[] color, int[] count, string nextDateTime)
        {
            HasPotion = hasPotion;
            Color = color;
            Count = count;
            NextDateTime = nextDateTime;
        }
    }

    [SerializeField] private GameObject _potionObject;
    [SerializeField] private List<PotionSlot> _potionSlots = new List<PotionSlot>();

    private int _varietyAmount;

    private void Awake()
    {
        _disableEvent = new UnityEvent();
        _enableEvent = new UnityEvent();
        _decreaseEvent = new UnityEvent();

        _decreaseEvent.AddListener(DecreaseVarietyAmount);
        LoadData();
    }

    private void SaveData()
    {
        bool[] hasPotion = new bool[10];
        string[] color = new string[10];
        int[] count = new int[10];

        for(int i = 0; i < _potionSlots.Count; i++)
        {
            Potion potion = _potionSlots[i].GetPotion();

            if (potion == null)
            {
                hasPotion[i] = false;
                color[i] = "";
                count[i] = 0;
            }
            else
            {
                hasPotion[i] = true;
                color[i] = "#" + ColorUtility.ToHtmlStringRGBA(potion.Color);
                count[i] = potion.GetCount();
            }
        }

        string nextDateTime = _nextDateTime.ToString();

        PotionSave potionSave = new PotionSave(hasPotion, color, count, nextDateTime);

        string contents = JsonUtility.ToJson(potionSave);

        File.WriteAllText(filePath, contents);
    }

    private void LoadData()
    {
        filePath = Application.persistentDataPath + "/gamedata.json";

        if (File.Exists(filePath))
        {
            Debug.Log("Exists...");
            string contents = File.ReadAllText(filePath);

            PotionSave potionSave = JsonUtility.FromJson<PotionSave>(contents);
            _nextDateTime = DateTime.Parse(potionSave.NextDateTime);

            for (int i = 0; i < _potionSlots.Count; i++)
            {
                if (potionSave.HasPotion[i])
                {
                    Color loadedCol;

                    GameObject loadedPotionObject = Instantiate(_potionObject);

                    if (ColorUtility.TryParseHtmlString(potionSave.Color[i], out loadedCol))
                    {
                        loadedPotionObject.GetComponent<Image>().color = loadedCol;
                    }

                    Potion loadedPotion = loadedPotionObject.GetComponent<Potion>();
                    loadedPotion.transform.position = _potionSlots[i].transform.position;
                    loadedPotion.transform.SetParent(_canvas.transform, true);
                    _potionSlots[i].SetPotion(loadedPotion);

                    loadedPotion.Color = loadedCol;
                    loadedPotion.SetCount(potionSave.Count[i]);
                    loadedPotion.PotionSlot = _potionSlots[i];

                    loadedPotion.AddEvents(_disableEvent, _enableEvent, _decreaseEvent);

                    _varietyAmount++;
                }
            }

            Debug.Log(contents);
        }
    }

    public void Generate()
    {
        if(DateTime.Now < _nextDateTime)
        {
            Debug.Log("Please wait a little longer...");
            return;
        }

        if (_varietyAmount < _potionSlots.Count)
        {
            Color newColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);

            if (!CheckIfColorUsed(newColor))
            {
                SpawnNewPotion(newColor);
            }
        }

        else
        {
            int index = UnityEngine.Random.Range(0, _potionSlots.Count);
            _potionSlots[index].GetPotion().IncreaseCount();
        }

        _nextDateTime = DateTime.Now.AddSeconds(_waitTime);
    }

    private void SpawnNewPotion(Color newColor)
    {
        GameObject newPotionObject = Instantiate(_potionObject);
        int index = CheckAvailableSlot();

        newPotionObject.GetComponent<Image>().color = newColor;

        Potion newPotion = newPotionObject.GetComponent<Potion>();
        newPotion.PotionSlot = _potionSlots[index];
        newPotion.Color = newColor;
        newPotion.transform.position = _potionSlots[index].transform.position;
        newPotion.transform.SetParent(_canvas.transform, true);

        _potionSlots[index].SetPotion(newPotion);
        newPotion.IncreaseCount();
        _varietyAmount++;

        newPotion.AddEvents(_disableEvent, _enableEvent, _decreaseEvent);
    }

    private bool CheckIfColorUsed(Color newColor)
    {
        foreach (PotionSlot slot in _potionSlots)
        {
            Potion potion = slot.GetPotion();

            if(potion == null)
            {
                continue;
            }

            if (potion.Color == newColor)
            {
                potion.IncreaseCount();
                return true;
            }
        }

        return false;
    }

    private int CheckAvailableSlot()
    {
        int slotIndex = 0;

        foreach (PotionSlot slot in _potionSlots)
        {
            Potion potion = slot.GetPotion();

            if (potion == null)
            {
                return slotIndex;
            }

            slotIndex++;
        }

        return slotIndex;
    }

    private void DecreaseVarietyAmount()
    {
        _varietyAmount--;
    }

    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<Potion>().DestroyPotion();
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationFocus(bool focus)
    {
        SaveData();
    }
}
