Shader "Unlit/TextAnimated"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _TimeOffset ("Time Offset", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            // Matriz con los vértices de los caracteres animados
            float _AnimateVerts[16];
            float _TimeOffset;

            v2f vert(appdata_t v, uint id : SV_VertexID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                // Buscamos si el vértice actual pertenece a una letra animada
                for (int i = 0; i < 16; i++)
                {
                    if (_AnimateVerts[i] == id)
                    {
                        // Aplicar animación tipo onda
                        float wave = sin(_Time.y * 5 + v.vertex.x * 10) * 0.05;
                        o.vertex.y += wave;
                    }
                }

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }

            ENDCG
        }
    }
}
