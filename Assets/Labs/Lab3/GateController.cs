using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator animator;

    [Header("Double Door Settings")]
    [Tooltip("Drag the second door (Gate (1))'s Animator component here!")]
    public Animator secondDoorAnimator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        Debug.Log("🔔 Signal received: Opening both doors!");

        // 1. Open the original door (this one)
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        // 2. Open the second door (if assigned)
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