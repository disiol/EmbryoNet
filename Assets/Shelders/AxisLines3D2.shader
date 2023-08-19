Shader "Custom/AxisLines2D2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColorX ("X Axis Color", Color) = (1, 0, 0, 1)
        _LineColorY ("Y Axis Color", Color) = (0, 1, 0, 1)
        _LineColorZ ("Z Axis Color", Color) = (0, 0, 1, 1)
        _Alpha ("Alpha", Range(0, 1)) = 1
        _Rotation ("Rotation", Range(0, 360)) = 0

    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _LineColorX;
            float4 _LineColorY;
            float4 _LineColorZ;
            float _Alpha;
            float _Rotation;


            float2 rotateUV(float2 uv, float angleDegrees)
            {
                float angleRadians = radians(angleDegrees);
                float2 center = float2(0.5, 0.5); // Center of the texture
                float2 rotatedUV;
                rotatedUV.x = cos(angleRadians) * (uv.x - center.x) - sin(angleRadians) * (uv.y - center.y) + center.y;
                rotatedUV.y = sin(angleRadians) * (uv.x - center.x) + cos(angleRadians) * (uv.y - center.y) + center.y;
                // Apply rotations to UV coordinates
                return rotatedUV;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);


                float2 rotatedUV_Y = v.vertex.xy * 0.5 + 0.5;
                rotatedUV_Y = -rotateUV(rotatedUV_Y, _Rotation);


                o.uv = rotatedUV_Y;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv.y);
                half4 lineColor = half4(0, 0, 0, 0);
                float2 center = float2(0.5, 0.5);

                float2 toCenter = i.uv - center;
                float angle = atan2(toCenter.y, toCenter.x);
                float distance = length(toCenter); // Manually calculate distance

                if (distance < 0.01)
                {
                    // Adjust the threshold for the center area
                    lineColor = texColor; // Preserve original color at the center
                }
                // else if (abs(angle) < 0.05)
                // {
                //     lineColor = _LineColorZ;
                // }
                else if (abs(angle - 2.57079633) < 0.05)
                {
                    lineColor = _LineColorY;
                }
                else if (abs(angle + 2.57079633) < 0.05)
                {
                    lineColor = _LineColorX;
                }

                // Apply transparency to lineColor based on _Alpha property
                lineColor.a *= _Alpha;

                return lerp(texColor, lineColor, 0.5); // Adjust intensity as needed
            }
            ENDCG
        }
    }
}