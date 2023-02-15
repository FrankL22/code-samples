Shader "Custom/FloorColorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 当前染色信息
            uniform StructuredBuffer<float> sprays : register(t1);

            // 地板标记点数量
            uniform int floorWidth = 0;
            uniform int floorHeight = 0;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算离当前像素最近的标记点
                int closest_x = i.uv.x * floorWidth + 0.5;
                int closest_y = i.uv.y * floorHeight + 0.5;
                int id = (closest_x * floorHeight) + closest_y;
                if (id > floorWidth * floorHeight - 1) id = floorWidth * floorHeight - 1;

                if (sprays[id] > 0.0)
                {
                    return (col + 3 * fixed4(1, 0, 1, 1)) / 4;
                }
                else if (sprays[id] < 0.0)
                {
                    return (col + 3 * fixed4(0, 1, 0, 1)) / 4;
                }
                return col;
            }
            ENDCG
        }
    }
}
