Shader "Custom/HalfLengthCenterRotatedLines" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _LineWidth ("Line Width", Range(0.001, 0.1)) = 0.01
        _XRotationAngle ("X Rotation Angle", Range(0, 360)) = 45
        _YRotationAngle ("Y Rotation Angle", Range(0, 360)) = 135
        _ZRotationAngle ("Z Rotation Angle", Range(0, 360)) = 225
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _LineWidth;
            float _XRotationAngle;
            float _YRotationAngle;
            float _ZRotationAngle;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Calculate UV coordinates relative to the center
                float2 centeredUV = v.uv - 0.5;
                // Reduce UV coordinates by half to adjust line length
                centeredUV *= 0.5;

                // Apply rotations to UV coordinates
                float cosX = cos(_XRotationAngle * 3.14159265 / 180.0);
                float sinX = sin(_XRotationAngle * 3.14159265 / 180.0);
                float2 rotatedUVX = float2(
                    centeredUV.x * cosX - centeredUV.y * sinX,
                    centeredUV.x * sinX + centeredUV.y * cosX
                );

                float cosY = cos(_YRotationAngle * 3.14159265 / 180.0);
                float sinY = sin(_YRotationAngle * 3.14159265 / 180.0);
                float2 rotatedUVY = float2(
                    centeredUV.x * cosY - centeredUV.y * sinY,
                    centeredUV.x * sinY + centeredUV.y * cosY
                );

                float cosZ = cos(_ZRotationAngle * 3.14159265 / 180.0);
                float sinZ = sin(_ZRotationAngle * 3.14159265 / 180.0);
                float2 rotatedUVZ = float2(
                    centeredUV.x * cosZ - centeredUV.y * sinZ,
                    centeredUV.x * sinZ + centeredUV.y * cosZ
                );

                o.uv = rotatedUVZ; // Use the rotated UV coordinates
                return o;
            }

            sampler2D _MainTex;

            half4 frag(v2f i) : SV_Target {
                half4 texCol = tex2D(_MainTex, i.uv);

                // Calculate distances from center in x, y, z flatness
                float xFlatness = abs(i.uv.x);
                float yFlatness = abs(i.uv.y);
                float zFlatness = length(i.uv);

                // Draw lines based on flatness
                half4 lineColor = texCol;
                if (xFlatness < _LineWidth) {
                    lineColor = half4(1, 0, 0, 1); // Line color for x rotation (red)
                } else if (yFlatness < _LineWidth) {
                    lineColor = half4(0, 1, 0, 1); // Line color for y rotation (green)
                } else if (zFlatness < _LineWidth) {
                    lineColor = half4(0, 0, 1, 1); // Line color for z rotation (blue)
                }

                return lineColor;
            }
            ENDCG
        }
    }
}
