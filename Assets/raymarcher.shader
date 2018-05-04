// adapted from:
// http://developer.download.nvidia.com/SDK/10/opengl/samples.html


Shader "raymarcher"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Volume("Volume", 3D) = "" {}
		_Axis("Axes orientation(0,1,2)", Vector) = (0, 1, 2)
		_Intensity("Intensity", Range(0.1, 5.0)) = 1.2
		_Threshold("Threshold", Range(0.0, 1.0)) = 0.95
			_Steps("Max number of steps", Range(1,1024)) = 128
			_Palette("Palette", 2D) = ""{}
		_SeedNumber("num of seeds", Int)= 32
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass{
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZTest LEqual
		ZWrite Off
		Fog{ Mode off }

			CGPROGRAM
#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 ray_o : TEXCOORD1; 
				float3 ray_d : TEXCOORD2; 
			};


			static const float3 colors[16] =
			{
				float3(0.5, 0.5, 0.2),float3(1, 0, 0),float3(1, 1, 0),float3(0, 1, 0),
				float3(0, 0, 1),float3(1, 0, 1),float3(1, 1, 1),float3(0, 1, 1),
				float3(0.25, 0.195, 0.681),float3(0.889, 0.058, 0.759),float3(0.99, 0.582, 0.617),float3(0.478, 0.142, 0.241),
				float3(0.1, 0.33, 0.283),float3(0.089, 0.929, 0.459),float3(0.78, 0.56, 0.3),float3(0.99, 0.23, 0.12)
			};

			sampler3D _Volume;
			half _Intensity, _Threshold;
			float3 _Axis;
			float4 _Color;
			sampler2D _Palette;
			int _SeedNumber;
			float4 get_data4(float3 pos) {
				// sample texture (pos is normalized in [0,1])
				float4 posTex = float4(pos[_Axis[0]], pos[_Axis[1]], pos[_Axis[2]], 0);
				//0 in w is the LOD we are looking up
				float4 data =  tex3Dlod(_Volume, posTex).rgba* _Intensity;
				
				//option 1 built in pal
				//float4 palColor = float4(colors[data.r/2], 1.0);
				//data.rgba = palColor *clamp(1-data.g/255.0, 0, 1);

				//option 2 pal and point
				/*float distance = data.g;
				distance =  step(2, data.g);
				float4 f = tex2D(_Palette, float2(data.r / _SeedNumber, 1));
				f.a = 0.1;
				data.rgba = f*distance + (1-distance)*float4(4,4,4,1);*/


				//option 3 distance field comment out others
				data.rgb =  (data.w - data.g) / 50;

				data.a = 0.8;// clamp(smoothstep(0, 15, data.r), 0, 1);
				/*float dis = abs(data.w - data.g);
				
				data.xyzw = lerp(float4(1.0, 0.6, 0.0, 1.0), float4((data.g / 75.0).xxx,0.01), step(1, dis));*/
			
				

				return data;
				//threshold
				//data *= step(_DataMin, data); //assuming data is 1d here
				//data *= step(data, _DataMax);


			}

			float get_data(float3 pos) {
				// sample texture (pos is normalized in [0,1])
				float4 posTex = float4(pos[_Axis[0]], pos[_Axis[1]], pos[_Axis[2]], 0);
				return tex3Dlod(_Volume, posTex).r* _Intensity;
				
				//threshold
				//data *= step(_DataMin, data); //assuming data is 1d here
				//data *= step(data, _DataMax);

				
			}

			// calculates intersection between a ray and a box
			// http://www.siggraph.org/education/materials/HyperGraph/raytrace/rtinter3.htm
			bool IntersectBox(float3 ray_o, float3 ray_d, float3 boxMin, float3 boxMax, out float tNear, out float tFar)
			{
				// compute intersection of ray with all six bbox planes
				float3 invR = 1.0 / ray_d;
				float3 tBot = invR * (boxMin.xyz - ray_o);
				float3 tTop = invR * (boxMax.xyz - ray_o);
				// re-order intersections to find smallest and largest on each axis
				float3 tMin = min(tTop, tBot);
				float3 tMax = max(tTop, tBot);
				// find the largest tMin and the smallest tMax
				float2 t0 = max(tMin.xx, tMin.yz);
				float largest_tMin = max(t0.x, t0.y);
				t0 = min(tMax.xx, tMax.yz);
				float smallest_tMax = min(t0.x, t0.y);
				// check for hit
				bool hit = (largest_tMin <= smallest_tMax); //change to a clip
				tNear = largest_tMin;
				tFar = smallest_tMax;
				return hit;
			}
			int _Steps;

			
			v2f vert (appdata v)
			{
				v2f o;
				// calculate eye ray in object space
				o.ray_d = -ObjSpaceViewDir(v.vertex);
				o.ray_o = v.vertex.xyz - o.ray_d;
				o.vertex = UnityObjectToClipPos(v.vertex);
			
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

			i.ray_d = normalize(i.ray_d);
			// calculate eye ray intersection with cube bounding box
			float3 boxMin = { -0.5, -0.5, -0.5 };
			float3 boxMax = { 0.5,  0.5,  0.5 };
			float tNear, tFar;
			bool hit = IntersectBox(i.ray_o, i.ray_d, boxMin, boxMax, tNear, tFar);
			if (!hit) discard;
			tNear = max(0.0, tNear);

			// calculate intersection points
			float3 pNear = i.ray_o + i.ray_d*tNear;
			float3 pFar = i.ray_o + i.ray_d*tFar;
			// convert to texture space
			pNear = pNear + 0.5.xxx;
			pFar = pFar + 0.5.xxx;
			float3 ray_pos = pNear;
			float3 ray_dir = pFar - pNear;

			


			float3 ray_step = normalize(ray_dir) * sqrt(3) / _Steps;
			//float3 ray_step = ray_dir / _Steps;
			//float3 ray_step = normalize(ray_dir) *(length(ray_dir) / _Steps);
			

			float4 dst = float4(0, 0, 0, 0); //destination "buffer"
			float3 p = pNear;

			const int s = _Steps;
			for (int iter = 0; iter < s; iter++) {
				
				//float v = get_data(p);
				//float4 src = float4(v, v, v, v); //if b&w data only .r value needed

				float4 src = get_data4(p); //color data

				src.a *= 0.5; //was 0.5f
				src.rgb *= src.a;

				// blend
				dst = (1.0 - dst.a) * src + dst;

				
				p += ray_step;

				//NOTE UNCOMENT THIS OR DEAL WITH ALPHA OVER 1 exit condition
				/*if (dst.a > _Threshold) {
					break;
				}*/
			}

			return saturate(dst) * _Color;
			}
			ENDCG
		}
	}
}
