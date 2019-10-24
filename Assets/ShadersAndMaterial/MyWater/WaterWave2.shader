Shader "Unity Shaders Book/Chapter 15/WaterWave2"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (0, 0.15, 0.115, 1)
		_ReflectionTex("Internal reflection", 2D) = "white" {}
		_WaveMap("Wave Map", 2D) = "bump" {}
		//_CubeMap ("Environment Cubemap", Cube) = "_Skybox" {}
		_WaveXSpeed("Wave Horizontal Speed", Range(-0.1, 0.1)) = 0.01
		_WaveYSpeed("Wave Vertical Speed", Range(-0.1, 0.1)) = 0.01
		_Distortion("Distortion", Range(0, 100)) = 10
		_Gloss("Gloss", Range(8.0, 256)) = 20
		_Specular("Specular", Color) = (1, 1, 1, 1)
		_HorizonColor("Simple water horizon color", COLOR) = (.172, .463, .435, 1)
		//[NoScaleOffset] _ReflectiveColor("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }

			GrabPass {"_RefractionTex"}

			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }
				CGPROGRAM
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				#pragma multi_compile_fwdbase

				#pragma vertex vert
				#pragma fragment frag

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _WaveMap;
				float4 _WaveMap_ST;
				sampler2D _ReflectionTex;
				float4 _ReflectionTex_ST;
				fixed _WaveXSpeed;
				fixed _WaveYSpeed;
				float _Distortion;
				sampler2D _RefractionTex;
				float4 _RefractionTex_TexelSize;
				float _Gloss;
				fixed4 _Specular;
				//sampler2D _ReflectiveColor;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float4 scrPos : TEXCOORD0;
					float4 uv : TEXCOORD1;
					float4 TtoW0 : TEXCOORD2;
					float4 TtoW1 : TEXCOORD3;
					float4 TtoW2 : TEXCOORD4;
					float4 ref : TEXCOORD5;
				};

				v2f vert(a2v v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					o.ref = ComputeNonStereoScreenPos(o.pos);

					o.scrPos = ComputeGrabScreenPos(o.pos);

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _WaveMap);

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
					fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
					fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

					o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
					o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
					o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target{
					float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
					fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
					float2 speed = _Time.y * float2(_WaveXSpeed, _WaveYSpeed);

					fixed3 bump1 = UnpackNormal(tex2D(_WaveMap, i.uv.zw + speed)).rgb;
					fixed3 bump2 = UnpackNormal(tex2D(_WaveMap, i.uv.zw - speed)).rgb;
					fixed3 bump = normalize(bump1 + bump2);

					

					float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
					i.scrPos.xy = offset * i.scrPos.z + i.scrPos.xy;
					fixed3 refrCol = tex2D(_RefractionTex, i.scrPos.xy / i.scrPos.w).rgb;

					bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
					fixed4 texColor = tex2D(_MainTex, i.uv.xy + speed);
					fixed3 reflDir = reflect(-viewDir, bump);
					float3 worldNormal = float3(i.TtoW0.z, i.TtoW1.z, i.TtoW2.z);

					float4 uv1 = i.ref;
					uv1.xy += bump;
					fixed3 reflCol = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(uv1));

					fixed fresnel = pow(1 - saturate(dot(viewDir, bump)), 4);
					//fixed fresnel = 1;

					//calculate the Blinn Specular Light Model;
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
					fixed3 myviewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
					fixed3 myLightReflectDir = normalize(reflect(-worldLightDir, bump));

					//half fresnelFac = dot(viewDir, bump);
					//half4 water = tex2D(_ReflectiveColor, float2(fresnelFac, fresnelFac));
					//water.rgb = lerp(water.rgb, reflCol.rgb, water.a);
					
					fixed3 halfDir = normalize(worldLightDir + myviewDir);
					//fixed3 specular = _LightColor0.rgb * _Specular * pow(max(0, dot(bump, halfDir)), _Gloss);
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(myLightReflectDir, myviewDir)), _Gloss);
					fixed3 finalColor = (reflCol) * fresnel + refrCol * (1 - fresnel) + specular;
					//fixed3 finalColor = specular;

					return fixed4(finalColor, 1);
				}
				ENDCG
			}
		}
			FallBack Off
}
