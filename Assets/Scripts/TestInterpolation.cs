using UnityEngine;

public class TestInterpolation : MonoBehaviour
{
    Rigidbody rg;
    float timer;

    void Start()
    {
        rg = GetComponent<Rigidbody>();
        // rg.interpolation = RigidBodyInterpolation.None;
        // rg.interpolation = RigidBodyInterpolation.Interpolate;
        // rg.interpolation = RigidBodyInterpolation.Extrapolate;

    }
    void FixedUpdate()
    {
        // TEST INTERPOLATION + EXTRAPOLATION #1
        // Vector3 moveDirection = Vector3.forward * 5f * Time.fixedDeltaTime; 
        // //tiến về hướng trục Z dương, right là X dương, up là Y dương, back là Z âm; là vector hướng nên giá trị của forward là 0,0,1 (z = 1)
        // //fixedDeltaTime là delta time tính theo fixedUpdate (0.02s)

        // rg.MovePosition(rg.position + moveDirection); //giá trị 'hướng' để move ở đây là 5f/0.02s

        // TEST INTER/EXTRAPOLATION #2 - TẮT KINEMATIC MỚI HOẠT ĐỘNG (VÌ method linearVelocity KO HOẠT ĐỘNG VỚI IsKinematic)
        timer += Time.fixedDeltaTime;
        float dir = Mathf.Sin(timer * 5f); // Hàm sin làm cho giá trị chuyển đổi qua lại giữa âm và dương đều đặn theo chu kỳ
        rg.linearVelocity = new Vector3(0,0,dir * 50f);
    }

    void Update()
    {
        Debug.Log("Vị trí hiện tại: " + transform.position);
    }
}
