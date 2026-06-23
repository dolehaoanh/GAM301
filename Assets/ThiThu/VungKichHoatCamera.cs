using UnityEngine;

public class VungKichHoatCamera : MonoBehaviour
{
    public CameraTheoDoi cameraTheoDoi;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.transform.root.CompareTag("Player")) && cameraTheoDoi != null)
        {
            cameraTheoDoi.QuanSatKeThuTamThoi();
        }
    }
}
