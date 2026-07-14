using UnityEngine;
using UnityEngine.Rendering;

public class HieuUngNhanVatChet : MonoBehaviour
{
    public Volume theTichHieuUng;
    public HPNhanVat hpNhanVat;
    public float tocDoChuyenDoi = 2f;

    private float weightMucTieu = 0f;

    private void Start()
    {
        if (hpNhanVat == null)
        {
            hpNhanVat = GetComponent<HPNhanVat>();
        }
        if (theTichHieuUng != null)
        {
            theTichHieuUng.weight = 0f;
        }
    }

    private void Update()
    {
        if (hpNhanVat == null || theTichHieuUng == null) return;

        if (hpNhanVat.mauHienTai <= 0f)
        {
            weightMucTieu = 1f;
        }
        else
        {
            weightMucTieu = 0f;
        }

        theTichHieuUng.weight = Mathf.MoveTowards(
            theTichHieuUng.weight,
            weightMucTieu,
            tocDoChuyenDoi * Time.deltaTime
        );
    }
}
