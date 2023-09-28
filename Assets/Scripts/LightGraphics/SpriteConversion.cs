using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public class SpriteConversion : MonoBehaviour
{
    [SerializeField]
    private RenderTexture renderTexture;
    private Texture2D texture;
    private Sprite spriteMask;
    Camera shadowCamera;
    [SerializeField] SpotLightArea spotLightScript;

    // 0で実行
    int isInvoke = 0;
    int tickRate = 5;

    private void Start()
    {
        shadowCamera = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        if (isInvoke == 0)
        {
            void SetTex(Texture2D texture2D)
            {
                texture = texture2D;
            }
            StartCoroutine(CreateTexture2D(renderTexture, SetTex));
            spotLightScript.shadowTexture = texture;
        }
        isInvoke = (isInvoke + 1) % tickRate;
    }

    /// <summary>
    /// texture2Dに変換
    /// </summary>
    private IEnumerator CreateTexture2D(RenderTexture texture, Action<Texture2D> onResult)
    {
        //yield return null;
        //Texture2Dを生成
        Texture2D texture2D = new Texture2D(
            texture.width,
            texture.height,
            TextureFormat.RGBA32,
            false,
            false
            );


        //カメラをレンダリング
        //shadowCamera.targetTexture = texture;
        //shadowCamera.Render();
        RenderTexture.active = texture;
        if (SystemInfo.supportsAsyncGPUReadback)
        {
            var reqest = AsyncGPUReadback.Request(renderTexture);
            yield return new WaitUntil(() => reqest.done);
            Unity.Collections.NativeArray<Color32> buffer = reqest.GetData<Color32>();
            texture2D.LoadRawTextureData(buffer);
            texture2D.Apply();
        }
        else
        {
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
        }
        onResult?.Invoke(texture2D);
    }
}
