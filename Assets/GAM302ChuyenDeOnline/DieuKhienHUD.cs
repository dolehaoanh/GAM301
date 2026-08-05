using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class DieuKhienHUD : MonoBehaviour
{
    public Slider thanhMau;
    public TextMeshProUGUI diemSoText;
    public TextMeshProUGUI thoiGianText;
    
    private ChiSoNhanVat chiSoNhanVatLocal;
    private QuanLyThoiGianTranDau quanLyThoiGian;

    private void Update()
    {
        if (chiSoNhanVatLocal == null)
        {
            ChiSoNhanVat[] cacNhanVat = Object.FindObjectsByType<ChiSoNhanVat>(FindObjectsInactive.Exclude);
            foreach (var nv in cacNhanVat)
            {
                if (nv.Object != null && nv.Object.HasStateAuthority)
                {
                    chiSoNhanVatLocal = nv;
                    break;
                }
            }
        }

        if (quanLyThoiGian == null)
        {
            quanLyThoiGian = Object.FindAnyObjectByType<QuanLyThoiGianTranDau>();
        }

        if (chiSoNhanVatLocal != null)
        {
            if (thanhMau != null)
            {
                if (thanhMau.maxValue < 100) thanhMau.maxValue = 100;
                thanhMau.value = chiSoNhanVatLocal.HP;
            }
                
            if (diemSoText != null)
                diemSoText.text = $"Diem So: {chiSoNhanVatLocal.DiemSo}";
        }

        if (quanLyThoiGian != null && quanLyThoiGian.Object != null && quanLyThoiGian.Object.IsValid && thoiGianText != null)
        {
            int phut = Mathf.FloorToInt(quanLyThoiGian.ThoiGianConLai / 60f);
            int giay = Mathf.FloorToInt(quanLyThoiGian.ThoiGianConLai % 60f);
            thoiGianText.text = string.Format("Thoi Gian: {0:00}:{1:00}", phut, giay);
        }
    }
}

