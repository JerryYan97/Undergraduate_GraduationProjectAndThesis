Shader "Unity Shaders Book/Chapter 13/MotionBlurWithDepthTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader
    {
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		sampler2D _CameraDepthTexture;
		float4x4 _CurrentViewProjectionInverseMatrix;
		float4x4 _PreviousViewProjectionMatrix;
		half _BlurSize;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv_depth : TEXCOORD1;
		};

		v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityWorldToClipPos(v.vertex);

			o.uv = v.texcoord;
			o.uv_depth = v.texcoord;

			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv_depth.y = 1 - o.uv_depth.y;
			#endif

			return o;
		}

		fixed4 frag(v2f i) :SV_Target{
			float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);

			float4 H = float4(i.uv.x * 2 - 1, i.uv.y * 2 - 1, d * 2 - 1, 1);//在NDC下，该片元的NDC坐标
			
			float4 D = mul(_CurrentViewProjectionInverseMatrix, H);

			float4 worldPos = D / D.w;//在世界空间下，该片元的世界空间坐标
			//float4 worldPos = D;
			//float4 worldPos = D * D.w;

			float4 currentPos = H;

			float4 previousPos = mul(_PreviousViewProjectionMatrix, worldPos);//在NDC下，该片元在上一帧的NDC坐标

			previousPos /= previousPos.w;
			//previousPos /= previousPos.w;

			float2 velocity = (currentPos.xy - previousPos.xy) / 2.0f;

			float2 uv = i.uv;
			float4 c = tex2D(_MainTex, uv);//因为，首先，脚本里有OnRenderImage抓取屏幕；之后，有Graphics.Blit把这个抓取到的屏幕作为材质，输出到_MainTex里；
			float2 moffset = velocity * _BlurSize;
			uv += moffset;
			float alpha = 0.5;

			for (int it = 1; it < 3; it++, uv += moffset, alpha-=0.2) {
				float4 currentColor = tex2D(_MainTex, uv);
				//c += currentColor;
				c.r = (alpha*(currentColor.r - c.r)) + c.r;
				c.g = (alpha*(currentColor.g - c.g)) + c.g;
				c.b = (alpha*(currentColor.b - c.b)) + c.b;
			}
			//c /= 3;
			return fixed4(c.rgb, 1.0);
		}

		ENDCG

		Pass{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
    }
	FallBack Off
}
