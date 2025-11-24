using UnityEngine;
using UnityEngine.UI;

public class HotbarInventory : MonoBehaviour
{
    [Header("Slots da Hotbar (UI)")]
    public Image[] slots;

    [Header("Cores")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;

    private Sprite[] items;
    private int selectedIndex = 0;

    void Awake()
    {
        items = new Sprite[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = null;
            slots[i].color = normalColor;
        }

        UpdateSelection();
    }

    void Update()
    {
        HandleHotbarInput();
    }

    void HandleHotbarInput()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedIndex = i;
                UpdateSelection();
            }
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }

    public bool AddItem(Sprite itemIcon)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemIcon;
                slots[i].sprite = itemIcon;
                return true;
            }
        }
        return false; // hotbar cheia
    }

    public Sprite GetSelectedItem()
    {
        return items[selectedIndex];
    }

    public void RemoveSelectedItem()
    {
        items[selectedIndex] = null;
        slots[selectedIndex].sprite = null;
    }
}
