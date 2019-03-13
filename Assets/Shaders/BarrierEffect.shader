
Shader "Custom/BarrierEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 1
		_Params("Params", Vector) = (0, 0, 0, 1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float _Scale;
			float4 _Params; // hitCenter.x, hitCenter.y, hitTimeSince, intensity

			float4 frag(v2f i) : SV_Target
			{
				// Extract the full intensity color.
				float4 color = tex2D(_MainTex, i.uv * _Scale);

				float2 hitCenter = _Params.xy;
				float hitTimeSince = _Params.z;
				float intensity = _Params.w;

				float distance = length(i.uv - hitCenter);
				distance *= 6.0f;

				// Prorate the distance based on time.
				//float overshoot = distance - hitTimeSince;
				distance = max(0, distance - hitTimeSince); //pow(1.0f + hitTimeSince, 2);
				intensity /= pow(1.0f + distance, 6);
				//intensity /= pow(1.0f + (distance / pow(1.0f + (hitTimeSince * 0.3f), 1)), 8);

				// Determine our proper intensity.
				color.a *= intensity;
				//float4 color = tex2D(_MainTex, i.uv) + float4(0, 0, _Greenification, 0); //float4(i.uv.r, i.uv.g, _Greenification, 1);
				return color;
			}
			ENDCG
		}
	}
}

