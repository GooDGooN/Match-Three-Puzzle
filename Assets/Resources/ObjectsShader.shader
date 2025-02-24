Shader "Custom/Decoration"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color3 ("Main Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjection"="true"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _MaskTex;
            float4 _Color3;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_MainTex, i.uv);
                fixed3 normal = UnpackNormal(tex2D(_NormalMap, i.uv));
                fixed mask = tex2D(_MaskTex, i.uv).r;

                col2.a *= normal; 
                col2 += mask;
                col += col2;
                col *= _Color3;

                return col;
            }
            ENDCG
        }
    }
}