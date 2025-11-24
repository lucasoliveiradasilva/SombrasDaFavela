using UnityEngine;
using UnityEngine.UIElements;

public class itembar : MonoBehaviour
{
    public bool slot1;
    public bool slot2;
    public bool slot3;

    public GameObject chave;



    private void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Chave") && slot1 == false)
        {
            slot1 = true;
            chave.SetActive(true);
        }


    }

}
