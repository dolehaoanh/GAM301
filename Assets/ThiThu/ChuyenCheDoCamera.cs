using UnityEngine;

public class ChuyenCheDoCamera : MonoBehaviour
{
    public GameObject cameraGocNhinThuBa;
    public GameObject cameraGocNhinTrenXuong;
    public KeyCode phimChuyenDoi = KeyCode.C;

    private bool dangNhinTrenXuong = false;

    private void Start()
    {
        if (cameraGocNhinThuBa != null) cameraGocNhinThuBa.SetActive(true);
        if (cameraGocNhinTrenXuong != null) cameraGocNhinTrenXuong.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(phimChuyenDoi))
        {
            dangNhinTrenXuong = !dangNhinTrenXuong;
            if (cameraGocNhinThuBa != null) cameraGocNhinThuBa.SetActive(!dangNhinTrenXuong);
            if (cameraGocNhinTrenXuong != null) cameraGocNhinTrenXuong.SetActive(dangNhinTrenXuong);
        }
    }
}
