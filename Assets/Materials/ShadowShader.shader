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
                // UV���W���t���O�����g�V�F�[�_�[�ɓn��
    o.texcoord = v.texcoord;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // �e�N�X�`���̐F��UV���W����擾���ăs�N�Z����`��
    return tex2D(_MainTex, i.texcoord);
}
            ENDCG
        }
    }
}