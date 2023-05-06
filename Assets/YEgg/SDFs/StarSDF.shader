Shader "Hidden/StarSDF"
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

			float sdf(in float2 p, in float r)
			{
				float rf = r * 3.0;
				const float2 k1 = float2(0.809016994375, -0.587785252292);
				const float2 k2 = float2(-k1.x,k1.y);
				p.x = abs(p.x);
				p -= 2.0*max(dot(k1,p),0.0)*k1;
				p -= 2.0*max(dot(k2,p),0.0)*k2;
				p.x = abs(p.x);
				p.y -= r;
				float2 ba = rf*float2(-k1.y,k1.x) - float2(0,1);
				float h = clamp( dot(p,ba)/dot(ba,ba), 0.0, r );
				return length(p-ba*h) * sign(p.y*ba.x-p.x*ba.y);
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
