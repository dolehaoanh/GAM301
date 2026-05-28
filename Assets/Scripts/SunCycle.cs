// Cách dùng:
// - Tạo object cha, gơi ý tên là SunPivot ("điểm neo để quay mặt trời")
// - cho Directional Light làm con của object vừa tạo (kéo thả)
// - gán script này vào object cha (SunPivot)
// - Lúc ban đầu test, sửa SerializeField 'CycleSeconds' trong inspector xuống thấp hơn 60s nhiều (ví dụ 6s) để test nhanh! 


using System.Collections;
using UnityEngine;

public class SunCycle : MonoBehaviour
{
    // Duration (giây) cho một chu kỳ cả ngày + đêm
    // Mặc định 60 giây => 30s ban ngày, 30s ban đêm (nếu dùng nửa vòng cho ban ngày)
    [SerializeField] private float cycleSeconds = 60f;

    // Lưu rotation ban đầu của SunPivot (được thiết lập trong Inspector)
    // Rotation này là mốc bắt đầu cho việc quay mặt trời.
    private Quaternion initialRotation;

    private void Start()
    {
        // Record lại initial rotation từ Inspector
        initialRotation = transform.localRotation;

        // Khởi chạy coroutine điều khiển chu kỳ mặt trời
        StartCoroutine(RunSunCycle());
    }

    private IEnumerator RunSunCycle()
    {
        // Loop vô hạn để chu kỳ ngày/đêm tái diễn
        while (true)
        {
            // Biến elapsed là thời gian đã trôi qua trong chu kỳ hiện tại
            float elapsed = 0f;

            // Run đủ 1 chu kỳ trong số giây lưu bởi cycleSeconds
            while (elapsed < cycleSeconds)
            {
                // t là tiến trình (cycle progress) từ 0 đến 1 của chu kỳ
                float t = elapsed / cycleSeconds;

                // Tính góc quay (360 độ trên toàn chu kỳ).
                // ? THÊM SAU: Nếu muốn chỉ hiển thị nửa vòng ban ngày --> giới hạn angle vào khoảng -90..90
                float angle = t * 360f;

                // Áp dụng quay quanh trục X dựa trên rotation ban đầu của SunPivot (object cha)
                // Directional Light (con của SunPivot) sẽ nhận hướng này (rotation quyết định hướng chiếu sáng)
                transform.localRotation = initialRotation * Quaternion.Euler(angle, 0f, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}