using UnityEngine;
using UnityEngine.Rendering;

public class VungDacBiet : MonoBehaviour
{
    public Volume boHieuUngDacBiet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (boHieuUngDacBiet != null) boHieuUngDacBiet.weight = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (boHieuUngDacBiet != null) boHieuUngDacBiet.weight = 0f;
        }
    }
}
