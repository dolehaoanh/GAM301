using UnityEngine;

public class DieuKhienNhanVat : MonoBehaviour
{
    public float tocDo = 10f;
    private Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float diChuyenNgang = Input.GetAxis("Horizontal");
        float diChuyenDoc = Input.GetAxis("Vertical");

        Vector3 huongDiChuyen = new Vector3(diChuyenNgang, 0f, diChuyenDoc).normalized;
        Vector3 viTriMoi = transform.position + huongDiChuyen * tocDo * Time.deltaTime;

        if (rb != null)
        {
            rb.MovePosition(viTriMoi);
        }
        else
        {
            transform.Translate(huongDiChuyen * tocDo * Time.deltaTime, Space.World);
        }

        if (huongDiChuyen != Vector3.zero)
        {
            transform.forward = huongDiChuyen;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", huongDiChuyen.magnitude * tocDo);
        }
    }
}
