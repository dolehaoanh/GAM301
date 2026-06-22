using UnityEngine;

public class VungKichHoatCamera : MonoBehaviour
{
    public CameraTheoDoi cameraTheoDoi;
    public Transform keThu;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.transform.root.CompareTag("Player")) && cameraTheoDoi != null && keThu != null)
        {
            cameraTheoDoi.QuanSatKeThuTamThoi(keThu);
        }
    }
}
