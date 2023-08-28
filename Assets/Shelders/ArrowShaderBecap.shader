Shader "Custom/ArrowShaderBecap"
{

    Properties
    {
        _Rotation("Rotation", Vector) = (0, 0, 90)
        _Position("Position", Vector) = (400, 225, 0)
        _Resolution("Resolution", Vector) = (800, 450, 0)
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
            #define ITERS 64

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float3 _Rotation;
            uniform float2 _Position;
            uniform float2 _Resolution;

            float PI = 3.1415926;
            const float deg2rad = 0.01745329251; // This is equivalent to PI / 180.0            int ITERS = 64;
            int AA = 4;


            // Vertex shader
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Signed distance function for the capped cone primitive https://iquilezles.org/articles/distfunctions
            float sdCone(float3 p, float3 a, float3 b, float ra, float rb)
            {
                float rba = rb - ra;
                float baba = dot(b - a, b - a);
                float papa = dot(p - a, p - a);
                float paba = dot(p - a, b - a) / baba;

                float x = sqrt(papa - paba * paba * baba);

                float cax = max(0.0, x - ((paba < 0.5) ? ra : rb));
                float cay = abs(paba - 0.5) - 0.5;

                float k = rba * rba + baba;
                float f = clamp((rba * (x - ra) + paba * baba) / k, 0.0, 1.0);

                float cbx = x - ra - f * rba;
                float cby = paba - f;

                float s = (cbx < 0.0 && cay < 0.0) ? -1.0 : 1.0;

                return s * sqrt(min(cax * cax + cay * cay * baba, cbx * cbx + cby * cby * baba));
            }

            // Compute rotation matrix from Euler angles
            float3x3 eulerToMat3(float pitch, float yaw, float roll)
            {
                // Convention: YXZ order (yaw, pitch, roll)
                float cp = cos(pitch);
                float sp = sin(pitch);
                float cy = cos(yaw);
                float sy = sin(yaw);
                float cr = cos(roll);
                float sr = sin(roll);

                float3x3 m;
                m[0] = float3(cy * cr + sy * sp * sr, sr * cp, -sy * cr + cy * sp * sr);
                m[1] = float3(-cy * sr + sy * sp * cr, cr * cp, sr * sy + cy * sp * cr);
                m[2] = float3(sy * cp, -sp, cy * cp);

                return m;
            }

            // Exponential smoothing
            float smin(float a, float b, float k)
            {
                float h = saturate(0.5 + 0.5 * (b - a) / k);
                return lerp(b, a, h) - k * h * (1.0 - h);
            }

            // Comparing the distances
            float2 opU(float2 d1, float2 d2)
            {
                float k = 0.01;
                float dst = smoothstep(d1.x, d2.x, k);
                float mat = (d1.x < d2.x) ? d1.y : d2.y;
                return float2(dst, mat);
            }


            float2 draw_arrow(float3 pos, float3 axis, float ID)
            {
                float arrow_len = 0.5;
                float thickness = 0.01;

                float3 axis_1 = normalize(axis) * arrow_len;

                float2 cyl = float2(sdCone(pos, float3(0.0, 0.0, 0.0), axis_1, thickness, thickness), ID);
                float2 cone = float2(sdCone(pos, axis_1, axis_1 * 1.2, thickness * 4.0, 0.0), ID);

                return opU(cyl, cone);
            }


            float2 draw_axes(float3 ray_hit)
            {
                float3x3 R = eulerToMat3(_Rotation.x * deg2rad,
                    _Rotation.y * deg2rad,
                    _Rotation.z * deg2rad);

                float2 aX = draw_arrow(ray_hit, mul(R, float3(1.0, 0.0, 0.0)), 0.0);
                float2 aY = draw_arrow(ray_hit, mul(R, float3(0.0, -1.0, 0.0)), 1.0);
                float2 aZ = draw_arrow(ray_hit, mul(R, float3(0.0, 0.0, 1.0)), 2.0);
                float2 res = opU(aX, aY);
                res = opU(res, aZ);

                return res;
            }


            // Calculates normals from sampled signed distance https://iquilezles.org/articles/normalsSDF
            float3 calc_normals(float3 ray_hit)
            {
                float2 e = float2(1.0, -1.0) * 0.5773;
                const float eps = 0.0005;
                return normalize(e.xyy * draw_axes(ray_hit + e.xyy * eps).x +
                    e.yyx * draw_axes(ray_hit + e.yyx * eps).x +
                    e.yxy * draw_axes(ray_hit + e.yxy * eps).x +
                    e.xxx * draw_axes(ray_hit + e.xxx * eps).x);
            }

            float3x3 setCamera(float3 ro, float3 ta, float cr)
            {
                float3 cw = normalize(ta - ro);
                float3 cp = float3(sin(cr), cos(cr), 0.0);
                float3 cu = normalize(cross(cw, cp));
                float3 cv = cross(cu, cw);
                return float3x3(cu, cv, cw);
            }

            // Get arrow color depending on the material id in the pixel
            float3 getRGBVector(float value)
            {
                float3 colors[3];
                colors[0] = float3(1.0, 0.0, 0.0); // Red
                colors[1] = float3(0.0, 1.0, 0.0); // Green
                colors[2] = float3(0.0, 0.0, 1.0); // Blue

                int index = int(fmod(value, 3.0));
                float fraction = frac(value);
                float3 color1 = colors[index];
                float3 color2 = colors[(index + 1) % 3];
                return lerp(color1, color2, fraction);
            }


            // Vertex shader
            v2f vert(appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5; // Adjust UV mapping if needed
                return o;
            }

            // Fragment shader

            half4 frag(v2f i) : SV_Target
            {
                float2 screen_pos = 2.0 * i.uv - 1.0;
                float2 stretch = float2(-_ScreenParams.x / _ScreenParams.y, 1.0);

                float3 ta = float3(screen_pos * stretch, 0.0);
                float3 ro = ta + float3(0.0, 0.0, 1.0);
                float3x3 ca = setCamera(ro, ta, 0.0);

                half4 tot = half4(0.0, 0.0, 0.0, 0.0);

                for (int m = 0; m < AA; m++) // anti-aliasing loops
                {
                    for (int n = 0; n < AA; n++)
                    {
                        // Oversampled pixel coordinates
                        float2 o = float2(float(m), float(n)) / float(AA) - 0.5;
                        float2 p = (2.0 * (i.uv + o) - _Resolution.xy) / _Resolution.y;

                        // Initializing the ray in orthographic projection
                        float3 rd = mul(ca, float3(0.0, 0.0, 1.0)); // constant ray direction (down Z)
                        float3 rayOrigin = ro + mul(ca, float3(p, 0.0)); // ray origin changes with p

                        // Raymarching
                        const float tmax = 15.0; // far clip
                        float t = 0.0;
                        float mat_ID = 0.0;

                        for (int i = 0; i < ITERS; i++)
                        {
                            float3 ray_hit = rayOrigin + t * rd;
                            float2 buf = draw_axes(ray_hit);
                            float h = buf.x;
                            mat_ID = buf.y;
                            if (h < 0.0001 || t > tmax) break;
                            t += h;
                        }

                        // Shading/lighting	
                        float3 lightDir = float3(0.57703, 0.57703, 0.57703);
                        float3 amb = float3(0.1, 0.1, 0.1);
                        fixed4 col = fixed4(0.0, 0.0, 0.0, 0.0);

                        if (t < tmax)
                        {
                            float3 ray_hit = rayOrigin + t * rd;
                            float3 nor = calc_normals(ray_hit);
                            float dif = saturate(dot(nor, lightDir));
                            col = fixed4(getRGBVector(mat_ID) * (dif + amb), 1.0);
                        }

                        // Gamma correction, just a beauty touch
                        col.rgb = pow(col.rgb, fixed3(0.4545, 0.4545, 0.4545));
                        tot += col;
                    }
                }

                return tot / (AA * AA);
            }
            ENDCG
        }
    }
}