using System.Collections.Generic;
using UnityEngine;

public class SinhKhoiCubeNgauNhien : MonoBehaviour
{
    public GameObject mauCube;
    public int soLuongPool = 15;
    public float chuKySinh = 0.5f;
    public float chieuRongVung = 10f;
    public float chieuSauVung = 10f;
    public float chieuCaoSinh = 15f;

    private List<GameObject> danhSachCube;
    private float boDemThoiGian;

    private void Start()
    {
        danhSachCube = new List<GameObject>();
        for (int i = 0; i < soLuongPool; i++)
        {
            GameObject cube = Instantiate(mauCube);
            cube.SetActive(false);
            danhSachCube.Add(cube);
        }
    }

    private void Update()
    {
        boDemThoiGian += Time.deltaTime;
        if (boDemThoiGian >= chuKySinh)
        {
            boDemThoiGian = 0f;
            SinhCube();
        }
    }

    private void SinhCube()
    {
        GameObject cube = LayCubeTuPool();
        if (cube == null) return;

        float ngauNhienX = Random.Range(-chieuRongVung / 2f, chieuRongVung / 2f);
        float ngauNhienZ = Random.Range(-chieuSauVung / 2f, chieuSauVung / 2f);
        cube.transform.position = transform.position + new Vector3(ngauNhienX, chieuCaoSinh, ngauNhienZ);
        cube.transform.rotation = Quaternion.identity;

        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            
            // Thêm vừa lăn vừa xoay
            rb.angularVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }

        // Góc sinh random:
        cube.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        // Góc sinh mặc định:
        // cube.transform.rotation = Quaternion.identity;

        cube.SetActive(true);
    }

    private GameObject LayCubeTuPool()
    {
        for (int i = 0; i < danhSachCube.Count; i++)
        {
            if (!danhSachCube[i].activeInHierarchy)
            {
                return danhSachCube[i];
            }
        }

        GameObject cubeMoi = Instantiate(mauCube);
        cubeMoi.SetActive(false);
        danhSachCube.Add(cubeMoi);
        return cubeMoi;
    }
}
