#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AutoSetupExecutor
{
    static AutoSetupExecutor()
    {
        // Tự động chạy SetupScene() ngay sau khi Unity hoàn tất biên dịch!
        // Điều này đảm bảo tất cả các thay đổi về màu sắc, quads minimap đỏ, xoay và mô hình được áp dụng tự động mà không cần click chuột.
        EditorApplication.delayCall += () =>
        {
            Debug.Log("[AutoSetupExecutor] Đang tự động áp dụng cấu hình và nhuộm màu hệ thống...");
            RTSSetupUtility.SetupScene();
        };
    }
}
#endif
