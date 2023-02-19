Shader "GRD/PirateGame/LoadingFade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Open("Open", Range(0,1)) = 1
        _ScreenWidth("Screen Width", float) = 1280
        _ScreenHeight("Screen Height", float) = 720
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed _Open;
            fixed _ScreenWidth;
            fixed _ScreenHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;

                fixed2 screenPoint = fixed2(i.uv.x - 0.5, (i.uv.y - 0.5) * _ScreenHeight / _ScreenWidth);
                fixed centerDist = length(screenPoint);
                col.a = lerp(0, col.a, step(_Open, centerDist));
                return col;
            }
            ENDCG
        }
    }
}
