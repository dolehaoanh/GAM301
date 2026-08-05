using UnityEngine;

public class NguoiChoi : MonoBehaviour
{
    public float vanToc = 6f;
    public KeyCode len = KeyCode.W;
    public KeyCode duoi = KeyCode.S;
    public KeyCode trai = KeyCode.A;
    public KeyCode phai = KeyCode.D;
    public Vector3 viTriXuatPhat;
    public bool daDenDich = false;
    public QuanLyTroChoi quanLyTroChoi;
    public string tagVungDich;

    private Rigidbody rb;

    private void Start()
    {
        viTriXuatPhat = transform.position;
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    private void Update()
    {
        Vector3 huongDi = Vector3.zero;
        if (Input.GetKey(len)) huongDi.z += 1f;
        if (Input.GetKey(duoi)) huongDi.z -= 1f;
        if (Input.GetKey(trai)) huongDi.x -= 1f;
        if (Input.GetKey(phai)) huongDi.x += 1f;

        huongDi = huongDi.normalized;
        
        if (rb != null)
        {
            rb.linearVelocity = huongDi * vanToc;
        }
        else
        {
            transform.Translate(huongDi * vanToc * Time.deltaTime, Space.World);
        }
    }

    public void DatLaiViTri()
    {
        transform.position = viTriXuatPhat;
        daDenDich = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BiXanh"))
        {
            if (quanLyTroChoi != null)
            {
                quanLyTroChoi.TangSoLanThua();
            }
        }
        else if (other.CompareTag(tagVungDich))
        {
            daDenDich = true;
            if (quanLyTroChoi != null)
            {
                quanLyTroChoi.KiemTraChienThang();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagVungDich))
        {
            daDenDich = false;
        }
    }
}
