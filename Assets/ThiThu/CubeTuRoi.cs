using UnityEngine;

public class CubeTuRoi : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }
}
