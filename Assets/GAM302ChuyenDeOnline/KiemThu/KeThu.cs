using UnityEngine;

public class KeThu : MonoBehaviour
{
    public int luongMau = 100;

    public void nhanSatThuong(int satThuong)
    {
        luongMau -= satThuong;
    }
}
