using UnityEngine;

public class bullet : MonoBehaviour
{
    Rigidbody rg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rg = GetComponent<Rigidbody>();

        // rg.linearVelocity = Vector3.up + new Vector3(Random.Range(-3f, 3f), Random.Range(10f, 15f), Random.Range(-3f, 3f));
        rg.linearVelocity = transform.forward*20f + new Vector3(Random.Range(-3f, 3f), Random.Range(10f, 15f), Random.Range(-1f, 1f));
        //đổi vector3.up thành transform.forward để đạn không bị cố định hướng bắn lên trời, * 20 để tăng tốc độ bắn
        //Random.Range tùy mọi người điều chỉnh theo ý muốn
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
