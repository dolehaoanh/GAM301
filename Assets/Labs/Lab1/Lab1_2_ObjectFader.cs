// QUAN TRONG: Material cua obj phai doi Surface Type ve Transparent thi moi dieu khien alpha (Renderer.material.color) de lam mo dc

using System.Collections; // for Coroutines
using UnityEngine;
using UnityEngine.InputSystem; // for new input sys

public class ObjectFader : MonoBehaviour
{
    public float fadeDuration = 5f; // thoi gian lam mo doi tuong
    private Renderer objectRenderer; // bien luu component Renderer cua doi tuong
    private bool isFading = false; // flag de kiem tra trang thai fading

    void Start()
    {
        // Lay component Renderer gan tren Obj nay
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError("GObj thieu component Renderer! Ko the la mo!");
        }
    }

    void Update()
    {
        //chi khi: 1. user an Space VA 2. doi tuong chua o trang thai fading thi moi kich hoat!
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !isFading)
        {
            Debug.Log("Da an Space! Bat dau coroutine 5s!");
            StartCoroutine(CoroutineLamMo());
        }
    }

    IEnumerator CoroutineLamMo()
    {
        isFading = true; // bat flag!
        // QUAN TRONG: vi alpha khong the cap nhat truc tiep nen phai gan gia tri thong qua bien trung gian mauGoc co dang Color
        Color mauGoc = objectRenderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime; // tinh thoi gian da troi qua

            // tinh ti le tu 0 de 1 dua tren thoi gian
            float t = elapsedTime / fadeDuration;
            // giam bien luu alpha tu 1f ve 0f theo ti le t
            float newAlpha = Mathf.Lerp(1f, 0f, t);
            // cap nhat mau voi alpha moi 
            objectRenderer.material.color = new Color(mauGoc.r, mauGoc.g, mauGoc.b, newAlpha);

            yield return null; // cho tiep du 1 frame de chuyen dong muot ma
        }

        // dam bao alpha = 0 (set trc tiep) khi ket thuc
        objectRenderer.material.color = new Color(mauGoc.r, mauGoc.g, mauGoc.b, 0f);

        // Set tro lai ve 1 de user biet la qua trinh da hoan thanh va co the thu lai
        objectRenderer.material.color = new Color(mauGoc.r, mauGoc.g, mauGoc.b, 1f);
        isFading = false; // cho phep lam mo tiep lan nua neu user lai an Space
        Debug.Log("Hoan thanh! An Space de thu lai lan nua!");

    }
}