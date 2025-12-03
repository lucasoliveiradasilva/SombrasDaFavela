using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public Image[] slots; // arraste as imagens dos slots aqui

    public void AddItem(Sprite icon)
    {
        // procura o primeiro slot vazio
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].enabled) // slot vazio = desativado
            {
                slots[i].sprite = icon;
                slots[i].enabled = true;
                return;
            }
        }

        Debug.Log("Hotbar cheia!");
    }
}
