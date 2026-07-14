using UnityEngine;

public class ChuyenGocNhinCamera : MonoBehaviour
{
    public GameObject cameraGocNhinThuNhat;
    public GameObject cameraGocNhinThuBa;
    public KeyCode phimChuyenGocNhin = KeyCode.H;

    private bool dangOThuNhat = false;

    private void Start()
    {
        if (cameraGocNhinThuNhat != null)
        {
            cameraGocNhinThuNhat.SetActive(false);
        }
        if (cameraGocNhinThuBa != null)
        {
            cameraGocNhinThuBa.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(phimChuyenGocNhin))
        {
            dangOThuNhat = !dangOThuNhat;
            if (cameraGocNhinThuNhat != null)
            {
                cameraGocNhinThuNhat.SetActive(dangOThuNhat);
            }
            if (cameraGocNhinThuBa != null)
            {
                cameraGocNhinThuBa.SetActive(!dangOThuNhat);
            }
        }
    }
}
