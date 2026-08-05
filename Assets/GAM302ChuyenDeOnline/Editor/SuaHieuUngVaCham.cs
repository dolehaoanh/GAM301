using UnityEditor;
using UnityEngine;

public class SuaHieuUngVaCham
{
    [MenuItem("Tools/Cap Nhat Vat Lieu Hieu Ung")]
    public static void CapNhatVatLieuHieuUng()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Color");

        if (shader == null) return;

        string pathVatLieu = "Assets/GAM302ChuyenDeOnline/HieuUngVaChamMaterial.mat";
        Material vatLieu = AssetDatabase.LoadAssetAtPath<Material>(pathVatLieu);

        if (vatLieu == null)
        {
            vatLieu = new Material(shader);
            vatLieu.color = new Color(1f, 0.9f, 0.3f, 1f);
            AssetDatabase.CreateAsset(vatLieu, pathVatLieu);
        }
        else
        {
            vatLieu.shader = shader;
            vatLieu.color = new Color(1f, 0.9f, 0.3f, 1f);
            EditorUtility.SetDirty(vatLieu);
        }

        string pathPrefab = "Assets/GAM302ChuyenDeOnline/HieuUngVaCham.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathPrefab);
        if (prefab != null)
        {
            ParticleSystem ps = prefab.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.duration = 0.3f;
                main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(6f, 14f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
                main.loop = false;

                var emission = ps.emission;
                emission.rateOverTime = 0;
                emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });

                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 55f;
                shape.radius = 0.1f;

                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(new Color(1f, 0.5f, 0f), 0.6f), new GradientColorKey(Color.red, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
                );
                colorOverLifetime.color = grad;

                var sizeOverLifetime = ps.sizeOverLifetime;
                sizeOverLifetime.enabled = true;
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0.0f, 1.0f);
                curve.AddKey(1.0f, 0.0f);
                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
            }

            ParticleSystemRenderer psr = prefab.GetComponent<ParticleSystemRenderer>();
            if (psr != null)
            {
                psr.sharedMaterial = vatLieu;
                psr.renderMode = ParticleSystemRenderMode.Stretch;
                psr.cameraVelocityScale = 0f;
                psr.velocityScale = 0.08f;
                psr.lengthScale = 2.2f;
            }

            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
        }
    }
}
