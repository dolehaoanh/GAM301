using NUnit.Framework;
using UnityEngine;

// Lop kiem thu Edit Mode cho he thong tinh sat thuong CombatSystem
public class KiemThuEditModeChienDau
{
    // Kiem thu truong hop 1: baseDamage = 10, multiplier = 1.5f -> ket qua mong doi = 15
    [Test]
    public void KiemTraSatThuongTruongHopMot()
    {
        // Khoi tao GameObject va gan component CombatSystem trong Edit Mode
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();

        int satThuongCoBan = 10;
        float heSoNhan = 1.5f;
        int ketQuaMongDoi = 15;

        // Tinh toan sat thuong thuc te
        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        // Ghi log chi tiet ket qua kiem thu Edit Mode truong hop 1
        Debug.Log($"[EDIT MODE TEST 1] Sat thuong co ban: {satThuongCoBan} | He so nhan: {heSoNhan} | Ket qua thuc te: {ketQuaThucTe} | Mong doi: {ketQuaMongDoi}");

        // So sanh ket qua thuc te voi ket qua mong doi
        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);

        // Huy GameObject sau khi kiem thu xong
        Object.DestroyImmediate(doiTuong);
    }

    // Kiem thu truong hop 2: baseDamage = 5, multiplier = 2.0f -> ket qua mong doi = 10
    [Test]
    public void KiemTraSatThuongTruongHopHai()
    {
        // Khoi tao GameObject va gan component CombatSystem trong Edit Mode
        GameObject doiTuong = new GameObject();
        CombatSystem heThongChienDau = doiTuong.AddComponent<CombatSystem>();

        int satThuongCoBan = 5;
        float heSoNhan = 2.0f;
        int ketQuaMongDoi = 10;

        // Tinh toan sat thuong thuc te
        int ketQuaThucTe = heThongChienDau.CalculateDamage(satThuongCoBan, heSoNhan);

        // Ghi log chi tiet ket qua kiem thu Edit Mode truong hop 2
        Debug.Log($"[EDIT MODE TEST 2] Sat thuong co ban: {satThuongCoBan} | He so nhan: {heSoNhan} | Ket qua thuc te: {ketQuaThucTe} | Mong doi: {ketQuaMongDoi}");

        // So sanh ket qua thuc te voi ket qua mong doi
        Assert.AreEqual(ketQuaMongDoi, ketQuaThucTe);

        // Huy GameObject sau khi kiem thu xong
        Object.DestroyImmediate(doiTuong);
    }
}
