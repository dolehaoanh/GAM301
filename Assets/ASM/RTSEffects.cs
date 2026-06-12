using UnityEngine;

public class RTSEffects : MonoBehaviour
{
    private static Texture2D softCircleTex;
    private static Texture2D sharpCircleTex;
    private static Material alphaBlendedMaterial;
    private static Material additiveMaterial;

    public static Texture2D GetSoftCircleTexture()
    {
        if (softCircleTex != null) return softCircleTex;
        softCircleTex = new Texture2D(32, 32);
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dx = x - 16f;
                float dy = y - 16f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.Clamp01(1f - (dist / 16f));
                alpha = alpha * alpha; // Quadratic falloff for soft edges
                softCircleTex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        softCircleTex.Apply();
        return softCircleTex;
    }

    public static Texture2D GetSharpCircleTexture()
    {
        if (sharpCircleTex != null) return sharpCircleTex;
        sharpCircleTex = new Texture2D(32, 32);
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float dx = x - 16f;
                float dy = y - 16f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = 0f;
                if (dist <= 14f)
                {
                    alpha = Mathf.Clamp01(1f - (dist / 14f));
                    alpha = Mathf.Sqrt(alpha); // Fuller center for brighter sparks
                }
                sharpCircleTex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        sharpCircleTex.Apply();
        return sharpCircleTex;
    }

    public static Material GetAlphaBlendedMaterial()
    {
        if (alphaBlendedMaterial != null) return alphaBlendedMaterial;

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        bool isURP = true;
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
            isURP = false;
        }

        alphaBlendedMaterial = new Material(shader);
        Texture2D tex = GetSoftCircleTexture();

        if (isURP)
        {
            alphaBlendedMaterial.SetFloat("_Surface", 1.0f); // 1 = Transparent
            alphaBlendedMaterial.SetFloat("_Blend", 0.0f);   // 0 = Alpha blend
            alphaBlendedMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            alphaBlendedMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            alphaBlendedMaterial.SetFloat("_ZWrite", 0.0f);
            alphaBlendedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            alphaBlendedMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            alphaBlendedMaterial.SetTexture("_BaseMap", tex);
        }
        else
        {
            if (alphaBlendedMaterial.HasProperty("_MainTex"))
            {
                alphaBlendedMaterial.SetTexture("_MainTex", tex);
            }
        }

        return alphaBlendedMaterial;
    }

    public static Material GetAdditiveMaterial()
    {
        if (additiveMaterial != null) return additiveMaterial;

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        bool isURP = true;
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
            isURP = false;
        }

        additiveMaterial = new Material(shader);
        Texture2D tex = GetSharpCircleTexture();

        if (isURP)
        {
            additiveMaterial.SetFloat("_Surface", 1.0f); // Transparent
            additiveMaterial.SetFloat("_Blend", 1.0f);   // Additive
            additiveMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            additiveMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One); // Additive blend
            additiveMaterial.SetFloat("_ZWrite", 0.0f);
            additiveMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            additiveMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            additiveMaterial.SetTexture("_BaseMap", tex);
        }
        else
        {
            if (additiveMaterial.HasProperty("_MainTex"))
            {
                additiveMaterial.SetTexture("_MainTex", tex);
            }
        }

        return additiveMaterial;
    }

    public static void SpawnHarvestEffect(Vector3 position, RTSResourceType resourceType)
    {
        GameObject effectObj = new GameObject("HarvestEffect");
        effectObj.transform.position = position;

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        
        ParticleSystemRenderer psr = effectObj.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            // Sparks look much better and brighter with additive blending
            psr.material = GetAdditiveMaterial();
        }

        var main = ps.main;
        main.duration = 0.5f;
        main.startLifetime = 0.4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.2f);
        main.loop = false;

        if (resourceType == RTSResourceType.Gold)
        {
            main.startColor = new Color(1f, 0.85f, 0.1f, 0.9f); // Gold sparks
        }
        else
        {
            main.startColor = new Color(0.4f, 0.25f, 0.15f, 0.8f); // Wood chips
        }

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 15) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;
        main.gravityModifier = 1.0f;

        Destroy(effectObj, 0.6f);
    }

    public static void SpawnImpactEffect(Vector3 position)
    {
        GameObject effectObj = new GameObject("ImpactEffect");
        effectObj.transform.position = position;

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        
        ParticleSystemRenderer psr = effectObj.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            // Combat sparks are bright and glowy
            psr.material = GetAdditiveMaterial();
        }

        var main = ps.main;
        main.duration = 0.4f;
        main.startLifetime = 0.3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.startColor = new Color(1f, 0.25f, 0.1f, 1f); // Vibrant orange-red sparks
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 12) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.2f;

        // Add a second small dust cloud system (soft alpha-blended)
        GameObject dustObj = new GameObject("DustCloud");
        dustObj.transform.SetParent(effectObj.transform);
        dustObj.transform.localPosition = Vector3.zero;

        ParticleSystem dustPs = dustObj.AddComponent<ParticleSystem>();
        ParticleSystemRenderer dustPsr = dustObj.GetComponent<ParticleSystemRenderer>();
        if (dustPsr != null)
        {
            dustPsr.material = GetAlphaBlendedMaterial();
        }

        var dMain = dustPs.main;
        dMain.duration = 0.5f;
        dMain.startLifetime = 0.5f;
        dMain.startSpeed = 0.8f;
        dMain.startSize = 0.4f;
        dMain.startColor = new Color(0.7f, 0.7f, 0.7f, 0.35f); // Gray dust cloud
        dMain.loop = false;

        var dEmission = dustPs.emission;
        dEmission.rateOverTime = 0;
        dEmission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 4) });

        Destroy(effectObj, 0.6f);
    }

    public static void SpawnUnitTrainedEffect(Vector3 position)
    {
        GameObject effectObj = new GameObject("UnitTrainedEffect");
        effectObj.transform.position = position;

        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
        
        ParticleSystemRenderer psr = effectObj.GetComponent<ParticleSystemRenderer>();
        if (psr != null)
        {
            psr.material = GetAdditiveMaterial();
        }

        var main = ps.main;
        main.duration = 1.0f;
        main.startLifetime = 0.8f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
        main.startColor = new Color(0f, 0.8f, 1f, 0.8f); // Cyan magic glow
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 30) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.6f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);

        Light l = effectObj.AddComponent<Light>();
        l.color = new Color(0f, 0.8f, 1f);
        l.range = 6f;
        l.intensity = 2.0f;

        LightFader fader = effectObj.AddComponent<LightFader>();
        fader.duration = 1.0f;

        Destroy(effectObj, 1.2f);
    }
}
