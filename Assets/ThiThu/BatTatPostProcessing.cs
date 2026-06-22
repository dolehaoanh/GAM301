using UnityEngine;
using UnityEngine.Rendering;

public class BatTatPostProcessing : MonoBehaviour
{
    public Volume boHieuUng;
    public KeyCode phimKichHoat = KeyCode.J;
    public float doDamDich = 1f;

    private bool dangKichHoat = false;

    private void Start()
    {
        if (boHieuUng == null)
        {
            boHieuUng = GetComponent<Volume>();
        }
        CapNhatHieuUng();
    }

    private void Update()
    {
        if (Input.GetKeyDown(phimKichHoat))
        {
            dangKichHoat = !dangKichHoat;
            CapNhatHieuUng();
        }
    }

    private void CapNhatHieuUng()
    {
        if (boHieuUng != null)
        {
            boHieuUng.weight = dangKichHoat ? doDamDich : 0f;
        }
    }
}
