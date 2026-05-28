using System.Collections;
using UnityEngine;

public class SpawnGameObject : MonoBehaviour
{
    public GameObject bullet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnBullet());
    }

    IEnumerator SpawnBullet()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            // Instantiate(bullet, Vector3.zero, Quaternion.identity);
            Instantiate(bullet, transform.position, transform.rotation);
            //đổi vector3.up thành transform.position để viên đạn sinh ra đúng tại vị trí của gameobject được gắn script
            //đổi quaternion.identity thành transform.rotation để viên đạn có thể xoay theo cùng hướng với gameobject họng súng
        }
    }
}
