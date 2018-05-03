Shader "Unlit/readDistField"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Pallete("Texture", 2D) = "white"{}
		_Width("width", Float) = 24
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Pallete;
			
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			float _Width;
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			//col.rgb =  (col.w / 32.0).xxx;
			float dis = abs(col.w - col.g);
			//col.rgb = dis.xxx/25;
			/*col.xyz = col.www;
			if (col.w < 2)
			{
				col.rgb = float3(1, 0, 0);
			}*/
			

			
				
				

				
			col.xyz = lerp(float3(1.0, 0.6, 0.0), col.g / 25.0, smoothstep(0, 3, dis));
			//col.xyz = lerp(float3(0.2, 0.05, 0.1), col.g/25.0, smoothstep(0, 3, col.w));
				//col.xyz = lerp( col.ggg/25, float3(1.0, 0.6, 0.0), smoothstep(0, 0.1, col.w));
			col.w = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
