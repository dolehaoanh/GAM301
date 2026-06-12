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
        
        // Tìm kiếm thông minh Animator thực tế (chọn cái có gắn Controller trong các con)
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            if (anim.runtimeAnimatorController != null)
            {
                animator = anim;
                break;
            }
        }

        // Fallback nếu không tìm thấy cái nào có controller thì lấy cái đầu tiên trong con
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning($"[RTS Unit Animation] Không tìm thấy Animator nào trên {gameObject.name} hoặc các con của nó!");
        }

        // Setup AudioSource for footsteps
        if (footstepClip != null)
        {
            footstepAudio = gameObject.AddComponent<AudioSource>();
            footstepAudio.clip = footstepClip;
            footstepAudio.loop = true;
            footstepAudio.playOnAwake = false;
            footstepAudio.spatialBlend = 1.0f; // 3D sound
            footstepAudio.minDistance = 2f;
            footstepAudio.maxDistance = 25f;
            footstepAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f) * 0.4f; // Muted a bit for footsteps
        }
    }

    private void Update()
    {
        // Safety checks: do nothing if animator is missing, agent is disabled, or unit is dead
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

        // 1. Lấy tốc độ di chuyển thực tế hiện tại của NavMeshAgent
        float currentSpeed = agent.velocity.magnitude;

        // 2. Truyền tốc độ này vào biến float "Speed" trong Animator Controller.
        animator.SetFloat("Speed", currentSpeed, speedDampTime, Time.deltaTime);

        // 3. Handle footstep SFX looping when moving
        if (footstepAudio != null)
        {
            if (currentSpeed > 0.2f)
            {
                if (!footstepAudio.isPlaying)
                {
                    // Update volume in case it changed in player prefs
                    footstepAudio.volume = PlayerPrefs.GetFloat("SFXVolume", 0.75f) * 0.4f;
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
