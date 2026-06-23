using UnityEngine;
using UnityEngine.Rendering;

public class HieuUngMauThap : MonoBehaviour
{
    public Volume boHieuUngMauThap;
    private HPNhanVat HPNhanVat;
    public float nguongMauThap = 0.3f;
    public float tocDoChuyenDoi = 2f; // Cần để chuyển từ từ trạng thái màu màn hình (chứ không bị giật cục)

    private void Start()
    {
        if (HPNhanVat == null)
        {
            HPNhanVat = GetComponent<HPNhanVat>();
        }
    }

    private void Update()
    {
        if (boHieuUngMauThap == null || HPNhanVat == null) return;

        float tiLeMau = HPNhanVat.mauHienTai / HPNhanVat.mauToiDa;
        float weightMucTieu = (tiLeMau <= nguongMauThap) ? 1f : 0f;

        boHieuUngMauThap.weight = Mathf.MoveTowards(
            boHieuUngMauThap.weight,
            weightMucTieu,
            tocDoChuyenDoi * Time.deltaTime
        );
    }
}
