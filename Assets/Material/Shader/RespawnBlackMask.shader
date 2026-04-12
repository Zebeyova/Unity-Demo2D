Shader "Hidden/RespawnMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Range(0,2)) = 2
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Overlay"
        }
        ZWrite Off
        ZTest Off
        Cull Off
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _Center;
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                fixed4 sceneColor = tex2D(_MainTex, i.uv);
                i.uv.x *= aspect;
                _Center.x *= aspect;
                float dis = distance(i.uv, _Center.xy);
                float alpha = smoothstep(_Radius - 0.01, _Radius, dis);
                return lerp(sceneColor, _Color, alpha);
            }
            ENDCG
        }
    }
}