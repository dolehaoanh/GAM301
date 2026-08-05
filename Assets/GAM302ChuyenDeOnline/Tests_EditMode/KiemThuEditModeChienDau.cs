using NUnit.Framework;
using UnityEngine;

public class KiemThuEditModeChienDau
{
    [Test]
    public void KiemTraSatThuongTruongHopMot()
    {
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();
        int satThuongCoBan = 10;
        float heSoNhan = 1.5f;
        int ketQuaMongDoi = 15;

        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);
        Object.DestroyImmediate(doiTuong);
    }

    [Test]
    public void KiemTraSatThuongTruongHopHai()
    {
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();
        int satThuongCoBan = 5;
        float heSoNhan = 2.0f;
        int ketQuaMongDoi = 10;

        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);
        Object.DestroyImmediate(doiTuong);
    }
}
