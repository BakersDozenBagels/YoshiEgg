Shader "Hidden/TeardropSDF"
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

			float sdf(in float2 p, in float3 d)
			{
			    p.x = abs(p.x);
				float b = (d.x-d.y)/d.z;
				float a = sqrt(1.0-b*b);
				float k = dot(p,float2(-b,a));
				if( k < 0.0 ) return length(p) - d.x;
				if( k > a*d.z ) return length(p-float2(0.0,d.z)) - d.y;
				return dot(p, float2(a,b) ) - d.x;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.94,0.9,0.86,1.0);
				for(int j = 0; j < 6; j++)
				{
					float x = (1-sign(sdf(i.uv - float2(_Points[2*j], _Points[2*j+1]), float3(_Scale / 3.0, 0.0, _Scale)))) / 2.0;
					col = (1.0 - x) * col + x * _Color;
				}
				return col;
			}
			ENDCG
		}
	}
}
