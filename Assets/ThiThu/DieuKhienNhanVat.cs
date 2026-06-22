using UnityEngine;

public class DieuKhienNhanVat : MonoBehaviour
{
    public float tocDo = 5f;

    private void Update()
    {
        float diChuyenNgang = Input.GetAxis("Horizontal");
        float diChuyenDoc = Input.GetAxis("Vertical");

        Vector3 huongDiChuyen = new Vector3(diChuyenNgang, 0f, diChuyenDoc).normalized;
        transform.Translate(huongDiChuyen * tocDo * Time.deltaTime, Space.World);

        if (huongDiChuyen != Vector3.zero)
        {
            transform.forward = huongDiChuyen;
        }
    }
}
