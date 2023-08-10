Shader "Custom/RotatedArrowsFromCoordinates"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Coordinates ("Coordinates", Vector) = (0, 0, 0)
        _Rotation("Rotation", Vector) = (0, 0, 90)

    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            const float deg2rad = 0.01745329251; // This is equivalent to PI / 180.0            int ITERS = 64;

            uniform float3 _Rotation;


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
            float3 _Coordinates;


        


            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);
                float axisThickness = 0.02;
                float arrowSize = 0.08;

                // Use the coordinates to determine rotation
                float cosX = cos(_Coordinates.x);
                float sinX = sin(_Coordinates.x);
                float cosY = cos(_Coordinates.y);
                float sinY = sin(_Coordinates.y);
                float cosZ = cos(_Coordinates.z);
                float sinZ = sin(_Coordinates.z);

                float2 rotatedUvX = float2(
                    (uv.x - center.x) * cosX + (uv.y - center.y) * sinX + center.x,
                    (uv.y - center.y) * cosY - (uv.x - center.x) * sinY + center.y
                );

                float2 rotatedUvY = float2(
                    (uv.x - center.x) * cosY + (uv.y - center.y) * sinY + center.x,
                    (uv.y - center.y) * cosY - (uv.x - center.x) * sinY + center.y
                );

                float2 rotatedUvZ = float2(
                    (uv.x - center.x) * cosZ + (uv.y - center.y) * sinZ + center.x,
                    (uv.y - center.y) * cosZ - (uv.x - center.x) * sinZ + center.y
                );

                float2 axisVectorX = rotatedUvX - center;
                float2 axisVectorY = rotatedUvY - center;
                float2 axisVectorZ = rotatedUvZ - center;

                float distX = abs(axisVectorX.x);
                float distY = abs(axisVectorY.y);
                float distZ = abs(axisVectorZ.y);

                // Define the arrow colors
                fixed4 xColor = fixed4(1, 0, 0, 1); // Red for x-axis
                fixed4 yColor = fixed4(0, 1, 0, 1); // Green for y-axis
                fixed4 zColor = fixed4(0, 0, 1, 1); // Blue for z-axis

                // Calculate arrow length and width based on distance from center
                float arrowLength = arrowSize;
                float arrowWidth = axisThickness;

                // Check if the pixel is inside an arrow
                if (distX < arrowLength && distX < arrowWidth)
                {
                    return xColor;
                }
                if (distY < arrowLength && distY < arrowWidth)
                {
                    return yColor;
                }
                if (distZ < arrowLength && distZ < arrowWidth)
                {
                    return zColor;
                }

                // Otherwise, sample the original texture
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}