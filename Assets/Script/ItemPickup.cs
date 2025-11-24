using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item")]
    public Sprite itemIcon;            // imagem do item
    public float distanceToPick = 3f;  // distância máxima para coletar

    [Header("Mensagem UI")]
    public GameObject messageUI;       // texto "Pressione E"

    private Transform player;
    private HotbarInventory hotbar;

    void Start()
    {
        // procura player e hotbar automaticamente
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        GameObject hotbarObj = GameObject.Find("Hotbar");
        if (hotbarObj != null)
            hotbar = hotbarObj.GetComponent<HotbarInventory>();

        if (messageUI != null)
            messageUI.SetActive(false);
    }

    void Update()
    {
        if (player == null || hotbar == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool near = dist <= distanceToPick;

        // mostra mensagem
        if (messageUI != null)
            messageUI.SetActive(near);

        // coleta item
        if (near && Input.GetKeyDown(KeyCode.E))
        {
            hotbar.AddItem(itemIcon);

            if (messageUI != null)
                messageUI.SetActive(false);

            Destroy(gameObject);
        }
    }
}
