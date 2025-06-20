using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public float lifetime = 0.4f;
    private SpriteRenderer sr;
    private Color startColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float fade = Time.deltaTime / lifetime;
        Color c = sr.color;
        c.a -= fade;
        sr.color = c;
    }
}
