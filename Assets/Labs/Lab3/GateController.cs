using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator animator;

    [Header("Cài đặt Cửa đôi")]
    [Tooltip("Kéo thành phần Animator của cánh cửa thứ hai (Gate (1)) vào đây!")]
    public Animator secondDoorAnimator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        // 1. Mở cánh cửa gốc (cánh cửa này)
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        // 2. Mở cánh cửa thứ hai (nếu được gán)
        if (secondDoorAnimator != null)
        {
            secondDoorAnimator.SetTrigger("Open");
        }
    }
}

// using UnityEngine;

// public class GateController : MonoBehaviour
// {
//     private Animator animator;

//     void Start()
//     {
//         // đưa vào cache component Animator component đã gắn vào Gate
//         animator = GetComponent<Animator>();

//         if (animator == null)
//         {
//             Debug.LogError("Không tìm. thấy Animator gắn trên Gate");
//         }
//     }

//     public void OpenGate()
//     {
//         Debug.Log("🇻🇳 đã nhận đc Signal, bật trigger 'Open'!🇻🇳");

//         if (animator != null)
//         {
//             // 'fire' trigger đã đặt trong Animator để chuyển sang anim ở cửa
//             animator.SetTrigger("Open");
//         }
//     }
// }