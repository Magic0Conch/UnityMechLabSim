﻿Shader "Unlit/RimLight"
{
	Properties{
		_SpecularGlass("SpecularGlassStrength", Range(0,64)) = 32
		_ObjColor("ObjColor", color) = (1,1,1,1)
		_RimColor("RimColor", color) = (1,1,1,1)
		_RimStrength("RimStrength", Range(0.0001,3.0)) = 0.1
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float3 worldNormal:NORMAL;
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD1;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	float _SpecularGlass;
	fixed4 _ObjColor;

	fixed4 _RimColor;
	float _RimStrength;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed3 worldNormal = normalize(i.worldNormal);

		fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

		fixed3 diffuse = max(0.0, dot(worldNormal, -worldLightDir));

		fixed3 diffuseColor = diffuse * _LightColor0.xyz;

		//计算reflect方向
		fixed3 reflectDir = normalize(reflect(worldLightDir, worldNormal));
		//计算视角方向
		fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
		//specular分量
		fixed3 specular = pow(max(0.0, dot(reflectDir, viewDir)), _SpecularGlass);

		fixed3 specularColor = specular * _LightColor0.xyz;

		//环境光
		fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _ObjColor;

		//RimLight
		fixed3 worldViewDir = normalize(viewDir);

		float rim = 1 - max(0, dot(worldViewDir, worldNormal));

		fixed3 rimColor = _RimColor * pow(rim, 1 / _RimStrength);

		//最后相加
		return fixed4(diffuseColor + specularColor + ambient + rimColor, 0);
	}

		ENDCG
	}
	}
}

