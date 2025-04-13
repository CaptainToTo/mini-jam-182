Shader "Skybox/SkyboxCubemapBlend"
{
    Properties
    {
        _DayCubemap ("Day Cubemap", Cube) = "" {}
        _NightCubemap ("Night Cubemap", Cube) = "" {}
        _Blend ("Blend Factor", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Background" "RenderType" = "Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            samplerCUBE _DayCubemap;
            samplerCUBE _NightCubemap;
            float _Blend;

            struct v2f
            {
                float3 texcoord : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.texcoord = vertex.xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 dayColor = texCUBE(_DayCubemap, i.texcoord).rgb;
                float3 nightColor = texCUBE(_NightCubemap, i.texcoord).rgb;
                return half4(lerp(nightColor, dayColor, _Blend), 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}