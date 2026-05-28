using UnityEngine;
using System.Collections; // for Coroutines

public class SequentialMovement : MonoBehaviour
{
    public GameObject [] mangObjectSeDiChuyen;
    public Transform [] mangDichDen;

    [Header("Hiệu chỉnh thông số")]
    public float moveDuration = 0.5f; // thoi gian di chuyen
    public float delay = 1f; // thoi gian doi truoc khi di chuyen obj tiep theo

    void Start()
    {
        // bat dau di chuyen tuan tu ("sequential movement")
        StartCoroutine(SequentialMoveRoutine());
    }

    IEnumerator SequentialMoveRoutine()
    {
        // tim gioi han nho nhat giua 2 mang de tranh loi crash do lech do lon giua 2 mang
        // tuc la neu co 1 mang co it phan tu hon thi dung gia tri cua mang do (min), gan vao bien limit
        // sau do chay vong lap for
        int limit = Mathf.Min(mangObjectSeDiChuyen.Length,mangDichDen.Length);
        
        for (int i = 0; i < limit; i++)
        {
            // check null doi tuong va diem dich
            if (mangObjectSeDiChuyen[i] != null && mangDichDen[i] != null)
            {
                // 1. goi ham di chuyen va DOI DEN KHI DI CHUYEN XONG
                yield return StartCoroutine(MoveObject(mangObjectSeDiChuyen[i].transform, mangDichDen[i].position));

                // 2. doi tiep 1s truoc khi move doi tuong tiep theo
                yield return new WaitForSeconds(delay);
            }
        }
    }
    IEnumerator MoveObject(Transform objTransform, Vector3 dich)
    {
        Vector3 startPosition = objTransform.position;
        float elapsedTime = 0f;

        // loop tung frame den khi du thoi gian di chuyen
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            // tinh % thoi gian hoan thanh (tu 0.00 de 1.00)
            float t = elapsedTime / moveDuration;

            // di chuyen
            objTransform.position = Vector3.Lerp(startPosition, dich, t);

            yield return null; // cho den next frame
        }

        // dam bao obj o dich tuyet doi (vi lerp ko bao gio den 100%)
        objTransform.position = dich;
    }
}