using System.Collections;
using UnityEngine;

public class NhatItem : MonoBehaviour
{
    public QuanLyCutscene quanLyCutscene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (quanLyCutscene != null)
            {
                quanLyCutscene.ChayQuayCutscene(transform);
            }
            gameObject.SetActive(false);
        }
    }
}
