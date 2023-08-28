Shader "Custom/CenterToLeftLine"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
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
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float4x4 _ObjectToWorld;
            float4 _Color;


            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                return o;
            }


            half4 frag(v2f i) : SV_Target
            {
                // Calculate the position along X where the line ends (left edge)
                float xPosition = -1.0;

                // If the pixel's X coordinate is to the left of xPosition, color it
                return (i.vertex.x <= xPosition) ? i.color : half4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}