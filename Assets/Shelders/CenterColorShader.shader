Shader "Custom/CenterColorShaderWithThickness" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Thickness ("Thickness", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags { "Queue"="Transparent" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 pos : SV_POSITION;
            };

            float4 _Color;
            float _Thickness;

            v2f vert(appdata_t v) {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Calculate the average world space position of the object's vertices
                float3 objectCenter = (i.worldPos + i.worldPos + i.worldPos) / 3.0f;

                // Calculate distance from the center to the vertex
                float distanceToCenter = length(i.worldPos - objectCenter);

                // Use distance and thickness to determine color intensity
                float intensity = saturate(1.0 - (distanceToCenter / _Thickness));
                fixed4 color = _Color * intensity;

                return color;
            }
            ENDCG
        }
    }
}
