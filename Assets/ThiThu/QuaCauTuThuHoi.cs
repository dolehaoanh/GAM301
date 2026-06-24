using UnityEngine;

public class QuaCauTuThuHoi : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }
}
