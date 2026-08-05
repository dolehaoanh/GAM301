using Fusion;
using UnityEngine;
using Unity.Cinemachine;

public class DiChuyenNhanVat : NetworkBehaviour
{
    public float tocDoDiChuyen = 5f;
    public float lucNhay = 5f;
    public float tocDoXoay = 5f;

    private CharacterController dieuKhienNhanVat;
    private Vector3 viTriHienTai;

    private void Awake()
    {
        dieuKhienNhanVat = GetComponent<CharacterController>();
    }

    public override void Spawned()
    {
        // Trong Shared Mode, nguoi tao ra object se co StateAuthority
        if (Object.HasStateAuthority)
        {
            CinemachineCamera cameraAo = UnityEngine.Object.FindAnyObjectByType<CinemachineCamera>();
            if (cameraAo != null)
            {
                cameraAo.Follow = transform;
                cameraAo.LookAt = transform;
            }
        }
    }

    private float vanTocY;
    public float trongLuc = -9.81f;

    private Vector3 lucDayLui = Vector3.zero;

    public void AppDungLucDayLui(Vector3 huongDay, float luc)
    {
        lucDayLui = huongDay.normalized * luc;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            var chiSo = GetComponent<ChiSoNhanVat>();
            if (chiSo != null && chiSo.HP <= 0)
            {
                return;
            }

            float diChuyenX = Input.GetAxis("Horizontal");
            float diChuyenZ = Input.GetAxis("Vertical");

            // Lấy hướng của Camera chính
            Vector3 camForward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
            Vector3 camRight = Camera.main != null ? Camera.main.transform.right : Vector3.right;
            
            // Loại bỏ trục Y để nhân vật không bị bay lên/chui xuống đất khi camera ngước lên/xuống
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            // Tính toán hướng di chuyển dựa trên camera
            Vector3 huongDiChuyen = (camForward * diChuyenZ) + (camRight * diChuyenX);
            Vector3 diChuyen = Vector3.zero;
            
            if (huongDiChuyen.magnitude >= 0.1f)
            {
                // Tinh toan xoay theo huong di chuyen
                float gocMucTieu = Mathf.Atan2(huongDiChuyen.x, huongDiChuyen.z) * Mathf.Rad2Deg;
                float gocHienTai = Mathf.LerpAngle(transform.eulerAngles.y, gocMucTieu, tocDoXoay * Runner.DeltaTime);
                transform.rotation = Quaternion.Euler(0f, gocHienTai, 0f);

                diChuyen = huongDiChuyen.normalized * tocDoDiChuyen;
            }

            // Xử lý lực đẩy lùi (knockback decay)
            if (lucDayLui.magnitude > 0.1f)
            {
                diChuyen += lucDayLui;
                lucDayLui = Vector3.Lerp(lucDayLui, Vector3.zero, 10f * Runner.DeltaTime);
            }
            else
            {
                lucDayLui = Vector3.zero;
            }

            if (dieuKhienNhanVat != null)
            {
                if (dieuKhienNhanVat.isGrounded)
                {
                    if (vanTocY < 0)
                    {
                        vanTocY = -2f;
                        
                        if (chiSo != null && chiSo.DangNhay)
                        {
                            chiSo.DangNhay = false;
                        }
                    }
                }

                if (Input.GetKey(KeyCode.Space) && dieuKhienNhanVat.isGrounded)
                {
                    vanTocY = Mathf.Sqrt(lucNhay * -2f * trongLuc);
                }

                vanTocY += trongLuc * 2f * Runner.DeltaTime; // Nhan 2 de roi nhanh hon
                diChuyen.y = vanTocY;

                dieuKhienNhanVat.Move(diChuyen * Runner.DeltaTime);
            }
            else
            {
                transform.position += diChuyen * Runner.DeltaTime;
            }
        }
    }
}
