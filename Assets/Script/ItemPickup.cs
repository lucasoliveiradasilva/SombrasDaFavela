using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public GameObject iconi;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            iconi.SetActive(true);
            Destroy(gameObject);
        }
    }
}
