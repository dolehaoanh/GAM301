using UnityEngine;

public class VungKichHoatCameraT2 : MonoBehaviour
{
    public GiamSatCameraT2 giamSatCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (giamSatCamera != null)
            {
                giamSatCamera.BatDauCutscene();
            }
            gameObject.SetActive(false);
        }
    }
}
