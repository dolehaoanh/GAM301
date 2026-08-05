using UnityEngine;

public class BiXanh : MonoBehaviour
{
    public float vanToc = 6f;
    public float gioihanTrai = -3.4f;
    public float gioihanPhai = 3.4f;
    public bool diSangPhai = true;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        if (diSangPhai)
        {
            transform.Translate(Vector3.right * vanToc * Time.deltaTime, Space.World);
            if (transform.position.x >= gioihanPhai)
            {
                diSangPhai = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * vanToc * Time.deltaTime, Space.World);
            if (transform.position.x <= gioihanTrai)
            {
                diSangPhai = true;
            }
        }
    }
}
