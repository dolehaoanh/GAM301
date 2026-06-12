using UnityEngine;

public enum RTSResourceType
{
    None,
    Gold,
    Wood
}

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Settings")]
    public RTSResourceType resourceType = RTSResourceType.Wood;
    public int remainingResources = 300;
    public float harvestRange = 2.0f; 

    private bool isDepleted = false;

    
    public int Gather(int amount)
    {
        if (isDepleted) return 0;

        int gathered = Mathf.Min(amount, remainingResources);
        remainingResources -= gathered;

        if (remainingResources <= 0)
        {
            isDepleted = true;
            DepleteNode();
        }

        return gathered;
    }

    private void DepleteNode()
    {
        Debug.Log($"[ResourceNode] Bãi tài nguyên {gameObject.name} đã bị khai thác cạn kiệt!");
        
        Destroy(gameObject);
    }
}
