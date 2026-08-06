using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Lop kiem thu Play Mode kiem tra viec ap dung sat thuong len luong mau ke thu trong Scene
public class KiemThuPlayModeChienDau
{
    // Test case chay trong Play Mode de kiem tra luong mau ke thu sau khi nhan sat thuong
    [UnityTest]
    public IEnumerator KiemTraTruLuongMauKeThu()
    {
        // Khoi tao GameObject ke thu va gan script KeThu voi luong mau ban dau = 100
        GameObject doiTuongKeThu = new GameObject("KeThu");
        KeThu keThu = doiTuongKeThu.AddComponent<KeThu>();
        keThu.luongMau = 100;

        // Khoi tao GameObject he thong chien dau va gan script CombatSystem
        GameObject doiTuongChienDau = new GameObject("HeThongChienDau");
        CombatSystem heThongChienDau = doiTuongChienDau.AddComponent<CombatSystem>();

        // Cho 1 frame de Scene va cac component duoc khoi tao hoan toan
        yield return null;

        int satThuongCoBan = 20;
        float heSoNhan = 1.5f;

        // Tinh toan luong sat thuong gay ra (20 * 1.5f = 30)
        int satThuongTinhDuoc = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        // Ap dung sat thuong len ke thu
        keThu.nhanSatThuong(satThuongTinhDuoc);

        int luongMauMongDoi = 70;

        // Ghi log chi tiet qua trinh va ket qua kiem thu Play Mode
        Debug.Log($"[PLAY MODE TEST] Mau ban dau: 100 | Sat thuong nhan: {satThuongTinhDuoc} | Mau con lai: {keThu.luongMau} | Mong doi: {luongMauMongDoi}");

        // Kiem tra luong mau con lai cua ke thu co bang 70 nhu mong doi hay khong
        Assert.AreEqual(luongMauMongDoi, keThu.luongMau);

        // Duyen huy cac GameObject thu nghiem sau khi hoan thanh test
        Object.Destroy(doiTuongKeThu);
        Object.Destroy(doiTuongChienDau);
    }
}
