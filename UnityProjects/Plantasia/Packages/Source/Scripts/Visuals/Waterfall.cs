using UnityEngine;

public class Waterfall : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public Renderer ModelRenderer = null;
    public float    ScrollSpeed = 0.75f;

    // -------------------------------------------------------------------------------

    private Vector2 TextureOffset = Vector2.zero;

    // -------------------------------------------------------------------------------

    void Awake()
    {
        if (ModelRenderer == null)
        {
            ModelRenderer = GetComponent<Renderer>();
            if (ModelRenderer == null)
            {
                Debug.LogError("Waterfall could not find a Renderer to work with! Disabling...", gameObject);
                enabled = false;
            }
        }
	}

    // -------------------------------------------------------------------------------

    void Update()
    {
        TextureOffset.y = (Time.time * -ScrollSpeed);
        ModelRenderer.material.SetTextureOffset("_MainTex", TextureOffset);
    }

    // -------------------------------------------------------------------------------
}
