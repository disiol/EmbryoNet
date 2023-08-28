Shader "Custom/Rotations"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rotation ("Rotation", Range(0, 360)) = 0
        _XLineThickness ("X Line Thickness", Range(0.001, 100)) = 5
        _YLineThickness ("Y Line Thickness", Range(0.001, 100)) = 5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Rotation;
            float _XLineThickness;
            float _YLineThickness;

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
                rotatedUV_Y = rotateUV(rotatedUV_Y, _Rotation);


                o.uv = rotatedUV_Y;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                float xDistanceToCenter = abs(i.uv.y - 0.5);
                float xLineCheck = step(xDistanceToCenter, _XLineThickness * 0.5) * step(i.uv.x, 0.5);

                float yLineDistanceToCenter = abs(i.uv.x - 0.5);
                float yLineCheck = step(yLineDistanceToCenter, _YLineThickness * 0.5) * step(i.uv.y, 0.5);

                if (xLineCheck > 0)
                {
                    color = half4(1, 0, 0, 1); // Set line color to red
                }
                else if (yLineCheck > 0)
                {
                    color = half4(0, 1, 0, 1); // Line color for y rotation (green)
                }
                return color;
            }
            ENDCG
        }
    }
}