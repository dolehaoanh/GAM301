using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class MinimapCameraIndicator : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform indicatorRect;
    public Camera mainCamera;
    public float mapWidth = 128f;
    public float mapHeight = 128f;

    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (indicatorRect == null || mainCamera == null) return;

        
        Vector3 botLeft = GetGroundPoint(new Vector3(0, 0, 0));
        Vector3 botRight = GetGroundPoint(new Vector3(1, 0, 0));
        Vector3 topLeft = GetGroundPoint(new Vector3(0, 1, 0));
        Vector3 topRight = GetGroundPoint(new Vector3(1, 1, 0));

        
        float minX = Mathf.Min(botLeft.x, botRight.x, topLeft.x, topRight.x);
        float maxX = Mathf.Max(botLeft.x, botRight.x, topLeft.x, topRight.x);
        float minZ = Mathf.Min(botLeft.z, botRight.z, topLeft.z, topRight.z);
        float maxZ = Mathf.Max(botLeft.z, botRight.z, topLeft.z, topRight.z);

        
        minX = Mathf.Clamp(minX, 0f, mapWidth);
        maxX = Mathf.Clamp(maxX, 0f, mapWidth);
        minZ = Mathf.Clamp(minZ, 0f, mapHeight);
        maxZ = Mathf.Clamp(maxZ, 0f, mapHeight);

        
        float normMinX = minX / mapWidth;
        float normMaxX = maxX / mapWidth;
        float normMinZ = minZ / mapHeight;
        float normMaxZ = maxZ / mapHeight;

        
        float normHeight = normMaxZ - normMinZ;
        float centerX = (normMinX + normMaxX) / 2.0f;
        normMinX = centerX - normHeight / 2.0f;
        normMaxX = centerX + normHeight / 2.0f;

        
        indicatorRect.anchorMin = new Vector2(normMinX, normMinZ);
        indicatorRect.anchorMax = new Vector2(normMaxX, normMaxZ);
        indicatorRect.offsetMin = Vector2.zero;
        indicatorRect.offsetMax = Vector2.zero;
    }

    private Vector3 GetGroundPoint(Vector3 viewportPos)
    {
        Ray ray = mainCamera.ViewportPointToRay(viewportPos);
        float enter;
        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        
        return mainCamera.transform.position + ray.direction * 50f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCameraToMinimapPoint(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCameraToMinimapPoint(eventData);
    }

    private void MoveCameraToMinimapPoint(PointerEventData eventData)
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float normX = (localPoint.x - rectTransform.rect.xMin) / rectTransform.rect.width;
            float normY = (localPoint.y - rectTransform.rect.yMin) / rectTransform.rect.height;

            normX = Mathf.Clamp01(normX);
            normY = Mathf.Clamp01(normY);

            float worldX = normX * mapWidth;
            float worldZ = normY * mapHeight;

            RTSCameraController camController = mainCamera.GetComponent<RTSCameraController>();
            if (camController != null)
            {
                Vector3 targetGroundPoint = new Vector3(worldX, 0f, worldZ);
                Vector3 currentCenterGround = GetGroundPoint(new Vector3(0.5f, 0.5f, 0f));
                Vector3 cameraOffset = currentCenterGround - mainCamera.transform.position;
                
                Vector3 newCameraPos = targetGroundPoint - cameraOffset;
                camController.TargetPosition = newCameraPos;
            }
        }
    }
}
