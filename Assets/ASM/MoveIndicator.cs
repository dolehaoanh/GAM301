using UnityEngine;

public class MoveIndicator : MonoBehaviour
{
    public float duration = 0.5f;
    public float maxScale = 2.0f;
    public Color color = Color.green;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;

    void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Procedurally generate a ring texture
        Texture2D texture = new Texture2D(128, 128);
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                float dx = x - 64f;
                float dy = y - 64f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (dist >= 48f && dist <= 60f)
                {
                    float alpha = 1f;
                    if (dist < 52f) alpha = (dist - 48f) / 4f;
                    else if (dist > 56f) alpha = (60f - dist) / 4f;
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, 0f));
                }
            }
        }
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 100f);
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = 10;
        
        Shader spriteShader = Shader.Find("Sprites/Default");
        if (spriteShader != null)
        {
            spriteRenderer.material = new Material(spriteShader);
        }
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = newColor;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        if (progress >= 1.0f)
        {
            Destroy(gameObject);
            return;
        }

        float currentScale = Mathf.Lerp(0.1f, maxScale, progress);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);

        Color c = color;
        c.a = Mathf.Lerp(1f, 0f, progress);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = c;
        }
    }
}
