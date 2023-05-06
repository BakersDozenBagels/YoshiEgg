Shader "Hidden/SquareSDF"
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

			float sdf(in float2 p, in float2 b)
			{
				float2 d = abs(p)-b;
				return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0.94,0.9,0.86,1.0);
				for(int j = 0; j < 6; j++)
				{
					float x = (1-sign(sdf(i.uv - float2(_Points[2*j], _Points[2*j+1]), float2(_Scale, _Scale)))) / 2.0;
					col = (1.0 - x) * col + x * _Color;
				}
				return col;
			}
			ENDCG
		}
	}
}
