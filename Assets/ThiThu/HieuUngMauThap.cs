using UnityEngine;
using UnityEngine.Rendering;

public class HieuUngMauThap : MonoBehaviour
{
    public Volume boHieuUngMauThap;
    public float nguongMauThap = 0.3f;

    public float mauHienTai = 100f;
    public float mauToiDa = 100f;

    private void Update()
    {
        float tiLeMau = mauHienTai / mauToiDa;

        if (boHieuUngMauThap != null)
        {
            boHieuUngMauThap.weight = (tiLeMau <= nguongMauThap) ? 1f : 0f;
        }
    }
}
