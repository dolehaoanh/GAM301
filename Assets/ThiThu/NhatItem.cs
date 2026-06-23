using System.Collections;
using UnityEngine;

public class NhatItem : MonoBehaviour
{
    public CameraQuayCutscene cameraQuayCutscene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (cameraQuayCutscene != null)
            {
                cameraQuayCutscene.ChayQuayCutscene(transform);
            }
            gameObject.SetActive(false);
        }
    }
}
