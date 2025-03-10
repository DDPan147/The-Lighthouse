Shader "Unlit/VertexAnimator"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", float) = 1.0
        _WaveAmplitude ("Wave Amplitude", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                uint vid : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _AnimateVerts[16];
            float _WaveSpeed;
            float _WaveAmplitude;

            v2f vert (appdata v)
            {
                v2f o;
                float4 pos = UnityObjectToClipPos(v.vertex);
                bool inRange = false;

                for (uint av = 0; av < _AnimateVerts.Length - 1; av += 2)
                {
                    if (_AnimateVerts[av] < 0 || _AnimateVerts[av+1] <= 0) // -1 is used to indicate the end of valid data. Also, no valid end index could possibly be 0, though a start index could.
                        break;
                    if (v.vid >= _AnimateVerts[av] && v.vid <= _AnimateVerts[av+1])
                    {
                        inRange = true;
                        break;
                        /*
                        OUT.vertex.y += (cos(OUT.vertex.x / 4 * 80 + _Time * -120) / 80); // Vertical wave. The exact values used are arbitrary; tinker with them to suit your needs.
                        float t = (_Time + v.vid / 4 * 2.0) * 100; // Rainbow timing. The values are also arbitrary. This uses v.vid so each character is entirely one color.
                        float r = cos(t) * 0.5 + 0.5; // 0 * pi
                        float g = cos(t + 2.094395) * 0.5 + 0.5; // 2/3 * pi
                        float b = cos(t + 4.188790) * 0.5 + 0.5; // 4/3 * pi
                        OUT.color.rgba = float4(r, g, b, 1);*/
                    }
                }
                if(inRange)
                {
                    float displacement = floor(v.vid/4);
                    float letterMovement = sin(_Time.y * _WaveSpeed + displacement) * _WaveAmplitude;
                    //pos += letterMovement;
                    float4 newPos = pos;
                    newPos.y += letterMovement;
                    pos = newPos;
                }

                o.vertex = pos;
                //o.vertex
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
