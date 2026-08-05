using UnityEngine;

public class ThoiGianTuHuy : MonoBehaviour
{
    public float thoiGianTonTai = 1.0f;
    private float đếmThoiGian = 0f;

    private void OnEnable()
    {
        đếmThoiGian = thoiGianTonTai;
    }

    private void Update()
    {
        đếmThoiGian -= Time.deltaTime;
        if (đếmThoiGian <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
