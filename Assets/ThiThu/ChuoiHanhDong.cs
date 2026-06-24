using System.Collections;
using UnityEngine;

public class ChuoiHanhDong : MonoBehaviour
{
    public Transform viTriA;
    public Transform viTriB;
    public Transform viTriC;
    public float thoiGianDiChuyenAB = 1f;
    public float thoiGianDiChuyenBC = 1f;
    public Color mauDich = Color.red;

    private void Start()
    {
        StartCoroutine(ThucHienChuoi());
    }

    private IEnumerator ThucHienChuoi()
    {
        yield return StartCoroutine(DiChuyen(viTriA.position, viTriB.position, thoiGianDiChuyenAB));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DiChuyen(viTriB.position, viTriC.position, thoiGianDiChuyenBC));
        yield return new WaitForSeconds(1f);
        DoiMau();
        Debug.Log("Action Done");
    }

    private IEnumerator DiChuyen(Vector3 tuViTri, Vector3 denViTri, float thoiGian)
    {
        float thoiGianTroiQua = 0f;
        while (thoiGianTroiQua < thoiGian)
        {
            transform.position = Vector3.Lerp(tuViTri, denViTri, thoiGianTroiQua / thoiGian);
            thoiGianTroiQua += Time.deltaTime;
            yield return null;
        }
        transform.position = denViTri;
    }

    private void DoiMau()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = mauDich;
        }
    }
}
