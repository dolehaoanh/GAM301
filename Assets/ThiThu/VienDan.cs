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
        ThuHoiDan();
    }

    private void OnTriggerEnter(Collider other)
    {
        ThuHoiDan();
    }

    private void ThuHoiDan()
    {
        gameObject.SetActive(false);
    }
}
