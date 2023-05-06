Shader "Hidden/MoonSDF"
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

			float sdf(in float2 p, in float3 x)
			{
				float d = x.x;
				float ra = x.y;
				float rb = x.z;
				p.y = abs(p.y);
				float a = (ra*ra - rb*rb + d*d)/(2.0*d);
				float b = sqrt(max(ra*ra-a*a,0.0));
				if( d*(p.x*b-p.y*a) > d*d*max(b-p.y,0.0) )
					  return length(p-float2(a,b));
				return max( (length(p          )-ra),
						   -(length(p-float2(d,0))-rb));
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.94,0.9,0.86,1.0);
				for(int j = 0; j < 6; j++)
				{
					float x = (1-sign(sdf(i.uv - float2(_Points[2*j], _Points[2*j+1]), _Scale * float3(0.6, 1.0, 0.8)))) / 2.0;
					col = (1.0 - x) * col + x * _Color;
				}
				return col;
			}
			ENDCG
		}
	}
}
