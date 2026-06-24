using UnityEngine;
using UnityEngine.Rendering;

public class DieuKhienPostProcessingH : MonoBehaviour
{
    public Volume boHieuUng;
    public KeyCode phimKichHoat = KeyCode.H;
    public float tocDoChuyenDoi = 2f;

    private float weightMucTieu = 0f;

    private void Start()
    {
        if (boHieuUng == null)
        {
            boHieuUng = GetComponent<Volume>();
        }
        if (boHieuUng != null)
        {
            boHieuUng.weight = 0f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(phimKichHoat))
        {
            weightMucTieu = (weightMucTieu == 0f) ? 1f : 0f;
        }

        if (boHieuUng != null)
        {
            boHieuUng.weight = Mathf.MoveTowards(boHieuUng.weight, weightMucTieu, tocDoChuyenDoi * Time.deltaTime);
        }
    }
}
