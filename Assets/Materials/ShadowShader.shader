Shader"Unlit/SimpleTexture" {
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

sampler2D _MainTex;

v2f vert(appdata v)
{
    v2f o;
                // UV座標をフラグメントシェーダーに渡す
    o.texcoord = v.texcoord;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // テクスチャの色をUV座標から取得してピクセルを描画
    return tex2D(_MainTex, i.texcoord);
}
            ENDCG
        }
    }
}