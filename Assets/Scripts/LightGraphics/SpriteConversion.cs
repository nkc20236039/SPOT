using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteConversion : MonoBehaviour
{
    [SerializeField]
    GameObject shadowSprite;
    SpriteMask mask;
    [SerializeField]
    private RenderTexture renderTexture;
    private Texture2D texture;
    public Sprite spriteMask { get; private set; }
    Camera shadowCamera;

    private void Start()
    {
        mask = shadowSprite.GetComponent<SpriteMask>();
    }

    void Update()
    {
        shadowCamera = GetComponent<Camera>();
        texture = CreateTexture2D(renderTexture);
        spriteMask = CreateSprite(texture);
        mask.sprite = spriteMask;
    }

    /// <summary>
    /// texture2D‚É•ÏŠ·
    /// </summary>
    private Texture2D CreateTexture2D(RenderTexture texture)
    {
        //Texture2D‚ğ¶¬
        Texture2D texture2D = new Texture2D(
            texture.width,
            texture.height,
            TextureFormat.ARGB32,
            false,
            false
            );
        //ƒJƒƒ‰‚ğƒŒƒ“ƒ_ƒŠƒ“ƒO
        shadowCamera.targetTexture = texture;
        shadowCamera.Render();

        RenderTexture.active = texture;
        texture2D.ReadPixels(
            new Rect(
                0f,
                0f,
                texture.width,
                texture.height
                ),
            0,
            0
            );
        texture2D.Apply();

        return texture2D;
    }

    private Sprite CreateSprite(Texture2D tex2D)
    {
        return Sprite.Create(
            tex2D,
            new Rect(
                0f,
                0f,
                tex2D.width,
                tex2D.height
                ),
            new Vector2(
                0.5f,
                0.5f
                ),
            100f
            );
    }
    
}
