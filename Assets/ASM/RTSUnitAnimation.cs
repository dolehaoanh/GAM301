using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RTSUnitAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Animation Smooth Settings")]
    [Tooltip("Độ mượt khi chuyển đổi thông số tốc độ trong Animator")]
    public float speedDampTime = 0.15f;

    [Header("Audio Settings")]
    public AudioClip footstepClip;
    private AudioSource footstepAudio;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
            {
                animator = anim;
                break;
            }
        }

        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning($"[RTS Unit Animation] Không tìm thấy Animator nào trên {gameObject.name} hoặc các con của nó!");
        }

        
        if (footstepClip != null)
        {
            footstepAudio = gameObject.AddComponent<AudioSource>();
            footstepAudio.clip = footstepClip;
            footstepAudio.loop = true;
            footstepAudio.playOnAwake = false;
            footstepAudio.spatialBlend = 0.2f; 
            footstepAudio.minDistance = 10f;
            footstepAudio.maxDistance = 100f;
            footstepAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f) * 0.8f; 
        }
    }

    private void Update()
    {
        
        if (animator == null || agent == null || !agent.enabled)
        {
            if (footstepAudio != null && footstepAudio.isPlaying) footstepAudio.Stop();
            return;
        }

        RTSUnit unit = GetComponent<RTSUnit>();
        if (unit != null && unit.currentState == RTSUnit.RTSUnitState.Dead)
        {
            if (footstepAudio != null && footstepAudio.isPlaying) footstepAudio.Stop();
            return;
        }

        
        float currentSpeed = agent.velocity.magnitude;

        
        animator.SetFloat("Speed", currentSpeed, speedDampTime, Time.deltaTime);

        
        if (footstepAudio != null)
        {
            if (currentSpeed > 0.2f)
            {
                if (!footstepAudio.isPlaying)
                {
                    
                    footstepAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f) * 0.8f;
                    footstepAudio.Play();
                }
            }
            else
            {
                if (footstepAudio.isPlaying)
                {
                    footstepAudio.Stop();
                }
            }
        }
    }

    private void OnDisable()
    {
        if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }
}
