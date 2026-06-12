using UnityEngine;

public class LightFader : MonoBehaviour
{
    public float duration = 1.0f;
    private Light targetLight;
    private float startIntensity;
    private float elapsedTime = 0f;

    void Start()
    {
        targetLight = GetComponent<Light>();
        if (targetLight != null)
        {
            startIntensity = targetLight.intensity;
        }
    }

    void Update()
    {
        if (targetLight == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        targetLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
    }
}
