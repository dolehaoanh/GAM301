using UnityEngine;

public class VienDan : MonoBehaviour
{
    public float tocDo = 10f;
    private float thoiGianKichHoat;

    private void OnEnable()
    {
        thoiGianKichHoat = 0f;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * tocDo * Time.deltaTime);
        thoiGianKichHoat += Time.deltaTime;
        if (thoiGianKichHoat >= 3f)
        {
            ThuHoiDan();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GaySatThuong(collision.gameObject);
        ThuHoiDan();
    }

    private void OnTriggerEnter(Collider other)
    {
        GaySatThuong(other.gameObject);
        ThuHoiDan();
    }

    private void GaySatThuong(GameObject doiTuong)
    {
        HPNhanVat sucKhoe = doiTuong.GetComponent<HPNhanVat>();
        if (sucKhoe == null)
        {
            sucKhoe = doiTuong.GetComponentInParent<HPNhanVat>();
        }

        if (sucKhoe != null)
        {
            sucKhoe.NhanSatThuong(10f);
        }
    }

    private void ThuHoiDan()
    {
        gameObject.SetActive(false);
    }
}
