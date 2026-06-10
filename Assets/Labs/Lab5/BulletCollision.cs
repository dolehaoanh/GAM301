using UnityEngine;
using UnityEngine.VFX; // PHẢI CÓ ĐỂ UNITY NHẬN DIỆN ĐC KIỂU DỮ LIỆU Visual Effect !!!

public class BulletCollision : MonoBehaviour
{
    // biến chứa hiệu ứng
    // public VisualEffect explosionEffect; - code mau ko hoat dong dung
    public GameObject explosionEffect;

    // xử lý va chạm
    // void OnCollisionEnter(Collision collision) - code mau ko hoat dong dung
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            if (explosionEffect != null)
            {
                // luu effect de xoa sau khi dung xong
                // & sinh hiệu ứng nổ tại vị trí va chạm
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);

                Destroy(effect, 0.111f);
            }

            // Hủy viên đạn
            Destroy(gameObject);
        }
    }
}