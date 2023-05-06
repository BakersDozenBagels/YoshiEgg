Shader "Hidden/HeartSDF"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
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
			
			uniform fixed4 _Color;
			uniform float _Points[12];
			uniform float _Scale;

			float dot2(float2 x)
			{
				return dot(x,x);
			}

			float sdf(in float2 p, in float r)
			{
				p = p / r;
				p.x = abs(p.x);

				if( p.y+p.x>1.0 )
					return sqrt(dot2(p-float2(0.25,0.75))) - sqrt(2.0)/4.0;
				return sqrt(min(dot2(p-float2(0.00,1.00)),
								dot2(p-0.5*max(p.x+p.y,0.0)))) * sign(p.x-p.y);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.94,0.9,0.86,1.0);
				for(int j = 0; j < 6; j++)
				{
					float x = (1-sign(sdf(i.uv - float2(_Points[2*j], _Points[2*j+1]), _Scale))) / 2.0;
					col = (1.0 - x) * col + x * _Color;
				}
				return col;
			}
			ENDCG
		}
	}
}
