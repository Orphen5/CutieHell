﻿Shader "Custom/TextureChanger" {
	Properties{
		_Color ("Color", Color) = (1,1,1,1)
		_BlendRadius("Blend Radius Start", Range(0.0, 1.0)) = 0.5
		_MainTex ("Main Texture", 2D) = "white" {}
		_AlternateTex("Alternate Texture", 2D) = "white" {}
		[Normal]
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalPower("Normal Power", Float) = 1.0
		_RoughnessMap("Roughness Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		int _ActiveEnemies = 0;
		int _BuildingsCount = 0;
		float4 _AiPositions[128];
		float _AiEffectRadius[128];

		fixed4 _Color;
		float _BlendRadius;
		sampler2D _MainTex;
		sampler2D _AlternateTex;
		sampler2D _NormalMap;
		float _NormalPower;
		sampler2D _RoughnessMap;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		struct Input {
			float2 uv_MainTex;
			float2 uv_AlternateTex;
			float2 uv_NormalMap;
			float2 uv_RoughnessMap;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float finalLerpFactor = 1;
			bool inEffectRadius = false;
			
			for (int i = 0; i < _BuildingsCount + _ActiveEnemies; ++i)
			{
				float distanceToEnemy = distance(IN.worldPos.xyz, _AiPositions[i].xyz);
				if (distanceToEnemy < _AiEffectRadius[i])
				{
					inEffectRadius = true;
					float inPct = distanceToEnemy / _AiEffectRadius[i];
					float blendRadius = i < _BuildingsCount ? _AiPositions[i].w : _BlendRadius;
					float lerpFactor = (inPct - blendRadius) / (1 - blendRadius);
					if (lerpFactor < finalLerpFactor)
						finalLerpFactor = lerpFactor;
					if (finalLerpFactor <= 0)
					{
						finalLerpFactor = 0;
						break;
					}
				}
			}

			fixed4 c;
			if (inEffectRadius)
			{
				if (finalLerpFactor <= 0)
					c = tex2D(_AlternateTex, IN.uv_AlternateTex) * _Color;
				else
					c = lerp(tex2D(_AlternateTex, IN.uv_AlternateTex), tex2D(_MainTex, IN.uv_MainTex), finalLerpFactor);
			}
			else
				c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = lerp(UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap)), fixed3(0,0,1), -_NormalPower + 1.0f);
			o.Smoothness = float4(1,1,1,1) - tex2D(_RoughnessMap, IN.uv_RoughnessMap);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
