using UnityEngine;
using Unity.Cinemachine;

public class DieuKhienCamera : MonoBehaviour
{
    public float doNhayChuot = 3f;
    
    [Header("Khoang Cach Camera")]
    public float chieuCaoCamera = 2f;
    public float khoangCachSau = 6f;
    public float chieuCaoNhinVao = 1.5f;

    private float gocXoayX = 0f;
    private float gocXoayY = 0f;

    private CinemachineCamera cameraAo;
    private Transform mucTieu;

    private void Awake()
    {
        cameraAo = GetComponent<CinemachineCamera>();
    }

    private void LateUpdate()
    {
        // 1. Đảm bảo camera theo dõi đúng nhân vật
        if (cameraAo != null && cameraAo.Follow != null)
        {
            mucTieu = cameraAo.Follow;
        }

        if (mucTieu == null) return;

        // 2. Lắng nghe sự kiện xoay chuột
        float chuotX = Input.GetAxis("Mouse X");
        float chuotY = Input.GetAxis("Mouse Y");

        gocXoayX += chuotX * doNhayChuot;
        gocXoayY -= chuotY * doNhayChuot;
        
        // Giới hạn góc nhìn lên xuống để không bị lật camera
        gocXoayY = Mathf.Clamp(gocXoayY, -20f, 60f);

        // 3. Xoay camera theo hướng chuột xung quanh nhân vật
        Quaternion gocQuay = Quaternion.Euler(gocXoayY, gocXoayX, 0);
        Vector3 khoangCach = new Vector3(0, chieuCaoCamera, -khoangCachSau); 

        transform.position = mucTieu.position + gocQuay * khoangCach;
        transform.LookAt(mucTieu.position + Vector3.up * chieuCaoNhinVao); 
    }
}
