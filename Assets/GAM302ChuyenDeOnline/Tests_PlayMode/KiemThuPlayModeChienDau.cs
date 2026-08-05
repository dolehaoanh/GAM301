using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KiemThuPlayModeChienDau
{
    [UnityTest]
    public IEnumerator KiemTraTruLuongMauKeThu()
    {
        GameObject doiTuongKeThu = new GameObject("KeThu");
        KeThu keThu = doiTuongKeThu.AddComponent<KeThu>();
        keThu.luongMau = 100;

        GameObject doiTuongChienDau = new GameObject("HeThongChienDau");
        CombatSystem heThongChienDau = doiTuongChienDau.AddComponent<CombatSystem>();

        yield return null;

        int satThuongCoBan = 20;
        float heSoNhan = 1.5f;
        int satThuongTinhDuoc = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);
        keThu.nhanSatThuong(satThuongTinhDuoc);

        int luongMauMongDoi = 70;
        Assert.AreEqual(luongMauMongDoi, keThu.luongMau);

        Object.Destroy(doiTuongKeThu);
        Object.Destroy(doiTuongChienDau);
    }
}
