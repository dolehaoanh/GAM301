using UnityEngine;
using UnityEngine.Rendering;

public class VungDacBiet : MonoBehaviour
{
    public Volume boHieuUngDacBiet;
    public float tocDoChuyenDoi = 0f; // = 0 thì xảy ra ngay lập tức, số càng to thì chuyển càng nhanh, vd: 0.5 --> chuyển đổi hết 2 giây, 1 -> 1 giây, 2 --> 0.5 giây (công thức: 1/tocDoChuyenDoi)

    private float weightMucTieu = 0f;

    private void Update()
    {
        if (boHieuUngDacBiet == null) return;

        if (tocDoChuyenDoi <= 0f)
        {
            boHieuUngDacBiet.weight = weightMucTieu;
        }
        else
        {
            boHieuUngDacBiet.weight = Mathf.MoveTowards(
                boHieuUngDacBiet.weight,
                weightMucTieu,
                tocDoChuyenDoi * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Entered special zone");
            weightMucTieu = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("Exited special zone");
            weightMucTieu = 0f;
        }
    }
}
