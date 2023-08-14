Shader "Custom/CenterToRightLine" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5; // Map vertex position to UV coordinates
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 color = tex2D(_MainTex, i.uv);
                if (i.uv.x < 0.5 && i.uv.y > 0.5) { // Draw the line starting from the center
                    color = half4(1, 0, 0, 1); // Set line color to red
                }
                return color;
            }
            ENDCG
        }
    }
}
