using UnityEngine;

// Lop quan ly thong tin va luong mau cua doi tuong ke thu
public class KeThu : MonoBehaviour
{
    // Bien public luong mau khoi tao cua ke thu
    public int luongMau = 100;

    // Phuong thuc nhan sat thuong va tru vao luong mau hien tai
    public void nhanSatThuong(int satThuong)
    {
        luongMau -= satThuong;
    }
}
