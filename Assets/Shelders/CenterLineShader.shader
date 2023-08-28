// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CenterLineShader" {
    Properties {
        _Center ("Center", Vector) = (0.5, 0.5, 0.5, 0.5)
        _LineLength ("Line Length", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags { "RenderType"="Transparent" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            float4 _Center;
            float _LineLength;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 fragPos = i.vertex.xy / i.vertex.w; // Normalize to screen space
                float2 center = _Center.xy;

                float distance = length(fragPos - center);
                float lineWidth = _LineLength * 0.5;

                if (distance >= 1.0 - lineWidth && distance <= 1.0 + lineWidth) {
                    return half4(1, 0, 0, 1); // Red line
                } else {
                    return half4(0, 0, 0, 1); // Black background
                }
            }
            ENDCG
        }
    }
}
