// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SnowShader"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_MainColor("MainColor", Color) = (1,1,1,0)
		_NoiseScale("NoiseScale", Float) = 1
		_VoronoiStrength("VoronoiStrength", Float) = 1
		_DisplacementTex("DisplacementTex", 2D) = "white" {}
		_SnowTex("SnowTex", 2D) = "white" {}
		_Control("Control", 2D) = "black" {}
		_SnowNormal("SnowNormal", 2D) = "bump" {}
		_DisplacedSnowTex("DisplacedSnowTex", 2D) = "white" {}
		_DisplacedSnowNormal("DisplacedSnowNormal", 2D) = "bump" {}
		_ControlView("ControlView", Range( 0 , 1)) = 0
		_CameraNearWorldHeight("CameraNearWorldHeight", Float) = 0
		_CameraCaptureHeight("CameraCaptureHeight", Float) = 10
		_MetallicGlossMap("MetallicGlossMap", 2D) = "black" {}
		_MinSnowHeight("MinSnowHeight", Float) = 0.1
		_NoiseSnowHeightMultiplier("NoiseSnowHeightMultiplier", Float) = 0
		_SnowHeightMultiplier("SnowHeightMultiplier", Float) = 0
		_Splat2("Splat2", 2D) = "white" {}
		_NoiseSnowControlMapUsage("NoiseSnowControlMapUsage", Range( 0 , 1)) = 0
		_Splat1("Splat1", 2D) = "white" {}
		_SnowControlMapUsage("SnowControlMapUsage", Range( 0 , 1)) = 0
		[ASEEnd]_NoiseOffset("NoiseOffset", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 16
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _DisplacedSnowTex;
			sampler2D _SnowTex;
			sampler2D _SnowNormal;
			sampler2D _DisplacedSnowNormal;
			sampler2D _MetallicGlossMap;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 CombineSamplesSharp128_g22( float S0, float S1, float S2, float Strength )
			{
				{
				    float3 va = float3( 0.13, 0, ( S1 - S0 ) * Strength );
				    float3 vb = float3( 0, 0.13, ( S2 - S0 ) * Strength );
				    return normalize( cross( va, vb ) );
				}
			}
			
			float3 CombineSamplesSharp128_g23( float S0, float S1, float S2, float Strength )
			{
				{
				    float3 va = float3( 0.13, 0, ( S1 - S0 ) * Strength );
				    float3 vb = float3( 0, 0.13, ( S2 - S0 ) * Strength );
				    return normalize( cross( va, vb ) );
				}
			}
			
					float2 voronoihash71( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash71( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						
			F1 = 8.0;
			for ( int j = -2; j <= 2; j++ )
			{
			for ( int i = -2; i <= 2; i++ )
			{
			float2 g = mg + float2( i, j );
			float2 o = voronoihash71( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = dot( 0.5 * ( r + mr ), normalize( r - mr ) );
			F1 = min( F1, d );
			}
			}
			return F1;
					}
			
			float3 PerturbNormal107_g24( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_Splat1 = IN.ase_texcoord7.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 GroundTexture278 = tex2D( _Splat1, uv_Splat1 );
				float2 uv_Control = IN.ase_texcoord7.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float GroundTextureUsage277 = tex2DNode269.g;
				float4 lerpResult285 = lerp( float4( 0,0,0,0 ) , GroundTexture278 , GroundTextureUsage277);
				float4 GroundColor287 = lerpResult285;
				float2 uv_Splat2 = IN.ase_texcoord7.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 RockTexture279 = tex2D( _Splat2, uv_Splat2 );
				float RockTextureUsage275 = tex2DNode269.b;
				float4 lerpResult284 = lerp( float4( 0,0,0,0 ) , RockTexture279 , RockTextureUsage275);
				float4 RockColor286 = lerpResult284;
				float3 appendResult43 = (float3(_MainColor.r , _MainColor.g , _MainColor.b));
				float2 texCoord58 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord80 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord49 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2D( _DisplacementTex, appendResult55 ).r );
				float MeshYPos223 = WorldPosition.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = IN.ase_texcoord7.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + WorldPosition.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float DisplacementPercent183 = ( 1.0 - clampResult113 );
				float4 lerpResult86 = lerp( tex2D( _DisplacedSnowTex, texCoord58 ) , tex2D( _SnowTex, texCoord80 ) , DisplacementPercent183);
				float SnowTextureUsage276 = tex2DNode269.r;
				float4 lerpResult304 = lerp( float4( 0,0,0,0 ) , ( float4( appendResult43 , 0.0 ) * lerpResult86 ) , SnowTextureUsage276);
				float4 SnowColor305 = lerpResult304;
				float4 lerpResult265 = lerp( ( GroundColor287 + RockColor286 + SnowColor305 ) , ControlColor301 , _ControlView);
				float4 FinalColor267 = lerpResult265;
				
				float localCalculateUVsSharp110_g22 = ( 0.0 );
				float2 texCoord81 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_85_0_g22 = texCoord81;
				float2 UV110_g22 = temp_output_85_0_g22;
				float4 TexelSize110_g22 = _SnowNormal_TexelSize;
				float2 UV0110_g22 = float2( 0,0 );
				float2 UV1110_g22 = float2( 0,0 );
				float2 UV2110_g22 = float2( 0,0 );
				{
				{
				    UV110_g22.y -= TexelSize110_g22.y * 0.5;
				    UV0110_g22 = UV110_g22;
				    UV1110_g22 = UV110_g22 + float2( TexelSize110_g22.x, 0 );
				    UV2110_g22 = UV110_g22 + float2( 0, TexelSize110_g22.y );
				}
				}
				float4 break134_g22 = tex2D( _SnowNormal, UV0110_g22 );
				float S0128_g22 = break134_g22.r;
				float4 break136_g22 = tex2D( _SnowNormal, UV1110_g22 );
				float S1128_g22 = break136_g22.r;
				float4 break138_g22 = tex2D( _SnowNormal, UV2110_g22 );
				float S2128_g22 = break138_g22.r;
				float temp_output_91_0_g22 = 1.5;
				float Strength128_g22 = temp_output_91_0_g22;
				float3 localCombineSamplesSharp128_g22 = CombineSamplesSharp128_g22( S0128_g22 , S1128_g22 , S2128_g22 , Strength128_g22 );
				float localCalculateUVsSharp110_g23 = ( 0.0 );
				float2 temp_output_85_0_g23 = texCoord81;
				float2 UV110_g23 = temp_output_85_0_g23;
				float4 TexelSize110_g23 = _DisplacedSnowNormal_TexelSize;
				float2 UV0110_g23 = float2( 0,0 );
				float2 UV1110_g23 = float2( 0,0 );
				float2 UV2110_g23 = float2( 0,0 );
				{
				{
				    UV110_g23.y -= TexelSize110_g23.y * 0.5;
				    UV0110_g23 = UV110_g23;
				    UV1110_g23 = UV110_g23 + float2( TexelSize110_g23.x, 0 );
				    UV2110_g23 = UV110_g23 + float2( 0, TexelSize110_g23.y );
				}
				}
				float4 break134_g23 = tex2D( _DisplacedSnowNormal, UV0110_g23 );
				float S0128_g23 = break134_g23.r;
				float4 break136_g23 = tex2D( _DisplacedSnowNormal, UV1110_g23 );
				float S1128_g23 = break136_g23.r;
				float4 break138_g23 = tex2D( _DisplacedSnowNormal, UV2110_g23 );
				float S2128_g23 = break138_g23.r;
				float temp_output_91_0_g23 = 1.5;
				float Strength128_g23 = temp_output_91_0_g23;
				float3 localCombineSamplesSharp128_g23 = CombineSamplesSharp128_g23( S0128_g23 , S1128_g23 , S2128_g23 , Strength128_g23 );
				float3 lerpResult118 = lerp( localCombineSamplesSharp128_g22 , localCombineSamplesSharp128_g23 , DisplacementPercent183);
				float3 SnowNormal191 = lerpResult118;
				float3 surf_pos107_g24 = WorldPosition;
				float3 surf_norm107_g24 = WorldNormal;
				float time71 = 0.0;
				float2 voronoiSmoothId71 = 0;
				float2 coords71 = texCoord26 * 0.3;
				float2 id71 = 0;
				float2 uv71 = 0;
				float voroi71 = voronoi71( coords71, time71, id71, uv71, 0, voronoiSmoothId71 );
				float Voronoi207 = ( voroi71 * _VoronoiStrength );
				float height107_g24 = ( Voronoi207 + NoiseSnowHeight201 );
				float scale107_g24 = 1.0;
				float3 localPerturbNormal107_g24 = PerturbNormal107_g24( surf_pos107_g24 , surf_norm107_g24 , height107_g24 , scale107_g24 );
				float3x3 ase_worldToTangent = float3x3(WorldTangent,WorldBiTangent,WorldNormal);
				float3 worldToTangentDir42_g24 = mul( ase_worldToTangent, localPerturbNormal107_g24);
				float3 temp_output_70_40 = worldToTangentDir42_g24;
				float3 Normal232 = BlendNormalRNM( SnowNormal191 , temp_output_70_40 );
				
				float2 uv_MetallicGlossMap = IN.ase_texcoord7.xy * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
				float4 Control2Color290 = tex2D( _MetallicGlossMap, uv_MetallicGlossMap );
				
				float4 break213 = ControlColor301;
				
				float3 Albedo = FinalColor267.rgb;
				float3 Normal = Normal232;
				float3 Emission = float3(0,0,0);
				float3 Specular = 0.5;
				float Metallic = Control2Color290.r;
				float Smoothness = break213.a;
				float Occlusion = 1;
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.ase_texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float4 ControlColor301 = tex2DNode269;
				float4 break213 = ControlColor301;
				
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _DisplacedSnowTex;
			sampler2D _SnowTex;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.ase_texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Splat1 = IN.ase_texcoord2.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 GroundTexture278 = tex2D( _Splat1, uv_Splat1 );
				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float GroundTextureUsage277 = tex2DNode269.g;
				float4 lerpResult285 = lerp( float4( 0,0,0,0 ) , GroundTexture278 , GroundTextureUsage277);
				float4 GroundColor287 = lerpResult285;
				float2 uv_Splat2 = IN.ase_texcoord2.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 RockTexture279 = tex2D( _Splat2, uv_Splat2 );
				float RockTextureUsage275 = tex2DNode269.b;
				float4 lerpResult284 = lerp( float4( 0,0,0,0 ) , RockTexture279 , RockTextureUsage275);
				float4 RockColor286 = lerpResult284;
				float3 appendResult43 = (float3(_MainColor.r , _MainColor.g , _MainColor.b));
				float2 texCoord58 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord80 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord49 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2D( _DisplacementTex, appendResult55 ).r );
				float MeshYPos223 = WorldPosition.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = IN.ase_texcoord2.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + WorldPosition.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float DisplacementPercent183 = ( 1.0 - clampResult113 );
				float4 lerpResult86 = lerp( tex2D( _DisplacedSnowTex, texCoord58 ) , tex2D( _SnowTex, texCoord80 ) , DisplacementPercent183);
				float SnowTextureUsage276 = tex2DNode269.r;
				float4 lerpResult304 = lerp( float4( 0,0,0,0 ) , ( float4( appendResult43 , 0.0 ) * lerpResult86 ) , SnowTextureUsage276);
				float4 SnowColor305 = lerpResult304;
				float4 lerpResult265 = lerp( ( GroundColor287 + RockColor286 + SnowColor305 ) , ControlColor301 , _ControlView);
				float4 FinalColor267 = lerpResult265;
				
				float4 break213 = ControlColor301;
				
				
				float3 Albedo = FinalColor267.rgb;
				float3 Emission = float3(0,0,0);
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _DisplacedSnowTex;
			sampler2D _SnowTex;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.ase_texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Splat1 = IN.ase_texcoord2.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 GroundTexture278 = tex2D( _Splat1, uv_Splat1 );
				float2 uv_Control = IN.ase_texcoord2.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float GroundTextureUsage277 = tex2DNode269.g;
				float4 lerpResult285 = lerp( float4( 0,0,0,0 ) , GroundTexture278 , GroundTextureUsage277);
				float4 GroundColor287 = lerpResult285;
				float2 uv_Splat2 = IN.ase_texcoord2.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 RockTexture279 = tex2D( _Splat2, uv_Splat2 );
				float RockTextureUsage275 = tex2DNode269.b;
				float4 lerpResult284 = lerp( float4( 0,0,0,0 ) , RockTexture279 , RockTextureUsage275);
				float4 RockColor286 = lerpResult284;
				float3 appendResult43 = (float3(_MainColor.r , _MainColor.g , _MainColor.b));
				float2 texCoord58 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord80 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord49 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2D( _DisplacementTex, appendResult55 ).r );
				float MeshYPos223 = WorldPosition.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = IN.ase_texcoord2.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + WorldPosition.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float DisplacementPercent183 = ( 1.0 - clampResult113 );
				float4 lerpResult86 = lerp( tex2D( _DisplacedSnowTex, texCoord58 ) , tex2D( _SnowTex, texCoord80 ) , DisplacementPercent183);
				float SnowTextureUsage276 = tex2DNode269.r;
				float4 lerpResult304 = lerp( float4( 0,0,0,0 ) , ( float4( appendResult43 , 0.0 ) * lerpResult86 ) , SnowTextureUsage276);
				float4 SnowColor305 = lerpResult304;
				float4 lerpResult265 = lerp( ( GroundColor287 + RockColor286 + SnowColor305 ) , ControlColor301 , _ControlView);
				float4 FinalColor267 = lerpResult265;
				
				float4 break213 = ControlColor301;
				
				
				float3 Albedo = FinalColor267.rgb;
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.ase_texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.ase_texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.ase_texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Control = IN.ase_texcoord3.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float4 ControlColor301 = tex2DNode269;
				float4 break213 = ControlColor301;
				
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
			}
			ENDHLSL
		}

		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
	UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"

		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define TESSELLATION_ON 1
			#pragma require tessellation tessHW
			#pragma hull HullFunction
			#pragma domain DomainFunction
			#define ASE_DISTANCE_TESSELLATION
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#define ASE_NEEDS_VERT_POSITION
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _MetallicGlossMap_ST;
			float4 _Control_ST;
			float4 _DisplacedSnowNormal_TexelSize;
			float4 _SnowNormal_TexelSize;
			float4 _MainColor;
			float4 _Splat2_ST;
			float4 _Splat1_ST;
			float2 _NoiseOffset;
			float _ControlView;
			float _CameraNearWorldHeight;
			float _VoronoiStrength;
			float _MinSnowHeight;
			float _NoiseSnowHeightMultiplier;
			float _NoiseSnowControlMapUsage;
			float _NoiseScale;
			float _SnowControlMapUsage;
			float _CameraCaptureHeight;
			float _SnowHeightMultiplier;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Control;
			sampler2D _DisplacementTex;
			sampler2D _Splat1;
			sampler2D _Splat2;
			sampler2D _DisplacedSnowTex;
			sampler2D _SnowTex;
			sampler2D _SnowNormal;
			sampler2D _DisplacedSnowNormal;
			sampler2D _MetallicGlossMap;
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				TEXTURE2D(_TerrainHeightmapTexture);//ASE Terrain Instancing
				TEXTURE2D( _TerrainNormalmapTexture);//ASE Terrain Instancing
				SAMPLER(sampler_TerrainNormalmapTexture);//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
				UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
			UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
			CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
				#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
					float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
					float4 _TerrainHeightmapScale;//ASE Terrain Instancing
				#endif//ASE Terrain Instancing
			CBUFFER_END//ASE Terrain Instancing


			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float3 CombineSamplesSharp128_g22( float S0, float S1, float S2, float Strength )
			{
				{
				    float3 va = float3( 0.13, 0, ( S1 - S0 ) * Strength );
				    float3 vb = float3( 0, 0.13, ( S2 - S0 ) * Strength );
				    return normalize( cross( va, vb ) );
				}
			}
			
			float3 CombineSamplesSharp128_g23( float S0, float S1, float S2, float Strength )
			{
				{
				    float3 va = float3( 0.13, 0, ( S1 - S0 ) * Strength );
				    float3 vb = float3( 0, 0.13, ( S2 - S0 ) * Strength );
				    return normalize( cross( va, vb ) );
				}
			}
			
					float2 voronoihash71( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi71( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
					{
						float2 n = floor( v );
						float2 f = frac( v );
						float F1 = 8.0;
						float F2 = 8.0; float2 mg = 0;
						for ( int j = -1; j <= 1; j++ )
						{
							for ( int i = -1; i <= 1; i++ )
						 	{
						 		float2 g = float2( i, j );
						 		float2 o = voronoihash71( n + g );
								o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
								float d = 0.5 * dot( r, r );
						 		if( d<F1 ) {
						 			F2 = F1;
						 			F1 = d; mg = g; mr = r; id = o;
						 		} else if( d<F2 ) {
						 			F2 = d;
						
						 		}
						 	}
						}
						
			F1 = 8.0;
			for ( int j = -2; j <= 2; j++ )
			{
			for ( int i = -2; i <= 2; i++ )
			{
			float2 g = mg + float2( i, j );
			float2 o = voronoihash71( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
			float d = dot( 0.5 * ( r + mr ), normalize( r - mr ) );
			F1 = min( F1, d );
			}
			}
			return F1;
					}
			
			float3 PerturbNormal107_g24( float3 surf_pos, float3 surf_norm, float height, float scale )
			{
				// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
				float3 vSigmaS = ddx( surf_pos );
				float3 vSigmaT = ddy( surf_pos );
				float3 vN = surf_norm;
				float3 vR1 = cross( vSigmaT , vN );
				float3 vR2 = cross( vN , vSigmaS );
				float fDet = dot( vSigmaS , vR1 );
				float dBs = ddx( height );
				float dBt = ddy( height );
				float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
				return normalize ( abs( fDet ) * vN - vSurfGrad );
			}
			
			VertexInput ApplyMeshModification( VertexInput v )
			{
			#ifdef UNITY_INSTANCING_ENABLED
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP( Terrain, _TerrainPatchInstanceData );
				float2 sampleCoords = ( patchVertex.xy + instanceData.xy ) * instanceData.z;
				float height = UnpackHeightmap( _TerrainHeightmapTexture.Load( int3( sampleCoords, 0 ) ) );
				v.vertex.xz = sampleCoords* _TerrainHeightmapScale.xz;
				v.vertex.y = height* _TerrainHeightmapScale.y;
				#ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
					v.ase_normal = float3(0, 1, 0);
				#else
					v.ase_normal = _TerrainNormalmapTexture.Load(int3(sampleCoords, 0)).rgb* 2 - 1;
				#endif
				v.texcoord.xy = sampleCoords* _TerrainHeightmapRecipSize.zw;
			#endif
				return v;
			}
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				v = ApplyMeshModification(v);
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float2 uv_Control = v.texcoord.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2Dlod( _Control, float4( uv_Control, 0, 0.0) );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = v.texcoord.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float2 texCoord49 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2Dlod( _DisplacementTex, float4( appendResult55, 0, 0.0) ).r );
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float MeshYPos223 = ase_worldPos.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + ase_worldPos.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float3 SnowWorldPos230 = ( ase_worldNormal * ( TotalSnowHeight202 * clampResult113 ) );
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = SnowWorldPos230;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			FragmentOutput frag ( VertexOutput IN 
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_Splat1 = IN.ase_texcoord7.xy * _Splat1_ST.xy + _Splat1_ST.zw;
				float4 GroundTexture278 = tex2D( _Splat1, uv_Splat1 );
				float2 uv_Control = IN.ase_texcoord7.xy * _Control_ST.xy + _Control_ST.zw;
				float4 tex2DNode269 = tex2D( _Control, uv_Control );
				float GroundTextureUsage277 = tex2DNode269.g;
				float4 lerpResult285 = lerp( float4( 0,0,0,0 ) , GroundTexture278 , GroundTextureUsage277);
				float4 GroundColor287 = lerpResult285;
				float2 uv_Splat2 = IN.ase_texcoord7.xy * _Splat2_ST.xy + _Splat2_ST.zw;
				float4 RockTexture279 = tex2D( _Splat2, uv_Splat2 );
				float RockTextureUsage275 = tex2DNode269.b;
				float4 lerpResult284 = lerp( float4( 0,0,0,0 ) , RockTexture279 , RockTextureUsage275);
				float4 RockColor286 = lerpResult284;
				float3 appendResult43 = (float3(_MainColor.r , _MainColor.g , _MainColor.b));
				float2 texCoord58 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord80 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 texCoord49 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break53 = texCoord49;
				float2 appendResult55 = (float2(break53.x , ( 1.0 - break53.y )));
				float DisplacementTexValue217 = ( 1.0 - tex2D( _DisplacementTex, appendResult55 ).r );
				float MeshYPos223 = WorldPosition.y;
				float YPosToCameraNearDistance225 = ( MeshYPos223 - _CameraNearWorldHeight );
				float temp_output_107_0 = ( YPosToCameraNearDistance225 / _CameraCaptureHeight );
				float4 ControlColor301 = tex2DNode269;
				float4 break211 = ControlColor301;
				float lerpResult200 = lerp( 1.0 , break211.b , _SnowControlMapUsage);
				float2 texCoord26 = IN.ase_texcoord7.xy * float2( 50,50 ) + _NoiseOffset;
				float simpleNoise23 = SimpleNoise( texCoord26*_NoiseScale );
				float lerpResult198 = lerp( 1.0 , break211.g , _NoiseSnowControlMapUsage);
				float NoiseSnowHeight201 = ( simpleNoise23 * lerpResult198 * _NoiseSnowHeightMultiplier );
				float TotalSnowHeight202 = ( ( _SnowHeightMultiplier * lerpResult200 ) + NoiseSnowHeight201 + _MinSnowHeight );
				float DisplacedSnowY220 = ( TotalSnowHeight202 + WorldPosition.y );
				float clampResult113 = clamp( ( ( DisplacementTexValue217 - temp_output_107_0 ) / ( ( DisplacedSnowY220 / _CameraCaptureHeight ) - temp_output_107_0 ) ) , 0.0 , 1.0 );
				float DisplacementPercent183 = ( 1.0 - clampResult113 );
				float4 lerpResult86 = lerp( tex2D( _DisplacedSnowTex, texCoord58 ) , tex2D( _SnowTex, texCoord80 ) , DisplacementPercent183);
				float SnowTextureUsage276 = tex2DNode269.r;
				float4 lerpResult304 = lerp( float4( 0,0,0,0 ) , ( float4( appendResult43 , 0.0 ) * lerpResult86 ) , SnowTextureUsage276);
				float4 SnowColor305 = lerpResult304;
				float4 lerpResult265 = lerp( ( GroundColor287 + RockColor286 + SnowColor305 ) , ControlColor301 , _ControlView);
				float4 FinalColor267 = lerpResult265;
				
				float localCalculateUVsSharp110_g22 = ( 0.0 );
				float2 texCoord81 = IN.ase_texcoord7.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_85_0_g22 = texCoord81;
				float2 UV110_g22 = temp_output_85_0_g22;
				float4 TexelSize110_g22 = _SnowNormal_TexelSize;
				float2 UV0110_g22 = float2( 0,0 );
				float2 UV1110_g22 = float2( 0,0 );
				float2 UV2110_g22 = float2( 0,0 );
				{
				{
				    UV110_g22.y -= TexelSize110_g22.y * 0.5;
				    UV0110_g22 = UV110_g22;
				    UV1110_g22 = UV110_g22 + float2( TexelSize110_g22.x, 0 );
				    UV2110_g22 = UV110_g22 + float2( 0, TexelSize110_g22.y );
				}
				}
				float4 break134_g22 = tex2D( _SnowNormal, UV0110_g22 );
				float S0128_g22 = break134_g22.r;
				float4 break136_g22 = tex2D( _SnowNormal, UV1110_g22 );
				float S1128_g22 = break136_g22.r;
				float4 break138_g22 = tex2D( _SnowNormal, UV2110_g22 );
				float S2128_g22 = break138_g22.r;
				float temp_output_91_0_g22 = 1.5;
				float Strength128_g22 = temp_output_91_0_g22;
				float3 localCombineSamplesSharp128_g22 = CombineSamplesSharp128_g22( S0128_g22 , S1128_g22 , S2128_g22 , Strength128_g22 );
				float localCalculateUVsSharp110_g23 = ( 0.0 );
				float2 temp_output_85_0_g23 = texCoord81;
				float2 UV110_g23 = temp_output_85_0_g23;
				float4 TexelSize110_g23 = _DisplacedSnowNormal_TexelSize;
				float2 UV0110_g23 = float2( 0,0 );
				float2 UV1110_g23 = float2( 0,0 );
				float2 UV2110_g23 = float2( 0,0 );
				{
				{
				    UV110_g23.y -= TexelSize110_g23.y * 0.5;
				    UV0110_g23 = UV110_g23;
				    UV1110_g23 = UV110_g23 + float2( TexelSize110_g23.x, 0 );
				    UV2110_g23 = UV110_g23 + float2( 0, TexelSize110_g23.y );
				}
				}
				float4 break134_g23 = tex2D( _DisplacedSnowNormal, UV0110_g23 );
				float S0128_g23 = break134_g23.r;
				float4 break136_g23 = tex2D( _DisplacedSnowNormal, UV1110_g23 );
				float S1128_g23 = break136_g23.r;
				float4 break138_g23 = tex2D( _DisplacedSnowNormal, UV2110_g23 );
				float S2128_g23 = break138_g23.r;
				float temp_output_91_0_g23 = 1.5;
				float Strength128_g23 = temp_output_91_0_g23;
				float3 localCombineSamplesSharp128_g23 = CombineSamplesSharp128_g23( S0128_g23 , S1128_g23 , S2128_g23 , Strength128_g23 );
				float3 lerpResult118 = lerp( localCombineSamplesSharp128_g22 , localCombineSamplesSharp128_g23 , DisplacementPercent183);
				float3 SnowNormal191 = lerpResult118;
				float3 surf_pos107_g24 = WorldPosition;
				float3 surf_norm107_g24 = WorldNormal;
				float time71 = 0.0;
				float2 voronoiSmoothId71 = 0;
				float2 coords71 = texCoord26 * 0.3;
				float2 id71 = 0;
				float2 uv71 = 0;
				float voroi71 = voronoi71( coords71, time71, id71, uv71, 0, voronoiSmoothId71 );
				float Voronoi207 = ( voroi71 * _VoronoiStrength );
				float height107_g24 = ( Voronoi207 + NoiseSnowHeight201 );
				float scale107_g24 = 1.0;
				float3 localPerturbNormal107_g24 = PerturbNormal107_g24( surf_pos107_g24 , surf_norm107_g24 , height107_g24 , scale107_g24 );
				float3x3 ase_worldToTangent = float3x3(WorldTangent,WorldBiTangent,WorldNormal);
				float3 worldToTangentDir42_g24 = mul( ase_worldToTangent, localPerturbNormal107_g24);
				float3 temp_output_70_40 = worldToTangentDir42_g24;
				float3 Normal232 = BlendNormalRNM( SnowNormal191 , temp_output_70_40 );
				
				float2 uv_MetallicGlossMap = IN.ase_texcoord7.xy * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
				float4 Control2Color290 = tex2D( _MetallicGlossMap, uv_MetallicGlossMap );
				
				float4 break213 = ControlColor301;
				
				float3 Albedo = FinalColor267.rgb;
				float3 Normal = Normal232;
				float3 Emission = float3(0,0,0);
				float3 Specular = 0.5;
				float Metallic = Control2Color290.r;
				float Smoothness = break213.a;
				float Occlusion = 1;
				float Alpha = ( break213.r + break213.g + break213.b + break213.a );
				float AlphaClipThreshold = 0.9;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				BRDFData brdfData;
				InitializeBRDFData( Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);
				half4 color;
				color.rgb = GlobalIllumination( brdfData, inputData.bakedGI, Occlusion, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif
				
				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;
				
					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
				
					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;
				
					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );
				
							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif
				
				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal, 0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif
				
				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif
				
				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				
				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Standard"
	
}
/*ASEBEGIN
Version=18923
6.4;22.4;1523.2;772.6;1135.019;227.7905;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;252;-4245.465,-4346.951;Inherit;False;2352.578;1027.003;Comment;10;287;286;285;284;283;282;281;280;257;253;ControlMaps;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;253;-4190.941,-4186.469;Inherit;False;1013.646;362.1724;;6;277;276;275;269;268;301;ControlMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;268;-4140.941,-4135.102;Inherit;True;Property;_Control;Control;5;0;Create;True;0;0;0;False;0;False;None;182143b1d9bfbe04fbeb575e3c3c07e7;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;269;-3889.963,-4133.053;Inherit;True;Property;_TextureSample13;Texture Sample 13;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;301;-3443.406,-4157.703;Inherit;False;ControlColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;215;-4222.75,-1087.718;Inherit;False;2844.611;940.2597;;27;35;207;73;82;71;72;202;32;148;156;205;200;201;155;33;165;198;154;23;25;211;163;26;45;210;219;251;Snow Height;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;45;-4172.75,-799.4335;Inherit;False;Constant;_Tiling;Tiling;3;0;Create;True;0;0;0;False;0;False;50,50;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;251;-4176.803,-654.4063;Inherit;False;Property;_NoiseOffset;NoiseOffset;21;0;Create;True;0;0;0;False;0;False;0,0;50,50;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;210;-3198.358,-819.4758;Inherit;True;301;ControlColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-3296.508,-525.9229;Inherit;False;Property;_NoiseSnowControlMapUsage;NoiseSnowControlMapUsage;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-3982.081,-810.678;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;211;-2972.568,-815.5829;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;25;-3556.634,-442.5087;Inherit;False;Property;_NoiseScale;NoiseScale;1;0;Create;True;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-2648.513,-483.6858;Inherit;False;Property;_NoiseSnowHeightMultiplier;NoiseSnowHeightMultiplier;14;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-3603.928,-658.3958;Inherit;True;Simple;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;198;-2880.583,-570.8193;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;165;-2808.908,-967.323;Inherit;False;Property;_SnowControlMapUsage;SnowControlMapUsage;20;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-2597.119,-627.04;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;229;-4258.747,605.1669;Inherit;False;3343.796;791.6506;Calculates Snow Height Percentage based on Camera Near, Camera Far and the Displacement Information from the texture.;20;230;30;50;29;203;183;144;113;112;110;111;218;102;227;228;225;104;224;101;247;Displacement Calculations;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;155;-2250.155,-984.0424;Inherit;False;Property;_SnowHeightMultiplier;SnowHeightMultiplier;15;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;200;-2398.604,-892.7703;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;216;-4231.169,-6.363519;Inherit;False;1996.334;352.0592;;8;143;48;47;55;124;53;49;217;Displacement Texture Value;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;201;-2464.536,-633.3912;Inherit;False;NoiseSnowHeight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-2201.05,-580.2992;Inherit;False;201;NoiseSnowHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;49;-3903.187,115.4286;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;247;-4158.801,738.1824;Inherit;False;911.8241;305.011;;5;223;204;139;220;250;Mesh Y Pos;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;156;-2208.567,-910.12;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-2015.806,-476.4009;Inherit;False;Property;_MinSnowHeight;MinSnowHeight;13;0;Create;True;0;0;0;False;0;False;0.1;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;250;-4120.293,798.6561;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;53;-3642.898,163.8127;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-1974.678,-599.1046;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;223;-3905.559,924.9735;Inherit;False;MeshYPos;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;124;-3459.544,237.0697;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;202;-1818.234,-602.3912;Inherit;False;TotalSnowHeight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-3281.015,165.0427;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;-4077.62,1110.627;Inherit;False;223;MeshYPos;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-3890.437,805.6093;Inherit;False;202;TotalSnowHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-4084.875,1217.371;Inherit;False;Property;_CameraNearWorldHeight;CameraNearWorldHeight;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;47;-4181.169,43.63647;Inherit;True;Property;_DisplacementTex;DisplacementTex;3;0;Create;True;0;0;0;False;0;False;d2f55d20cd432914583e1c23ce001291;a4d935db798009440b42857d3722acb2;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-3653.17,817.8441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;104;-3818.587,1174.692;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;48;-3063.157,89.31762;Inherit;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;225;-3653.907,1169.81;Inherit;False;YPosToCameraNearDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;228;-3130.846,1101.592;Inherit;False;492.2762;192.7844;Low Snow divided by camera range;2;226;107;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;220;-3485.378,810.5935;Inherit;False;DisplacedSnowY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;227;-3154.271,705.3668;Inherit;False;424.3704;189.3101;High Snow divided by camera Range;2;221;108;;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;143;-2748.325,117.6409;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;221;-3104.271,755.3668;Inherit;False;220;DisplacedSnowY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;226;-3080.846,1151.592;Inherit;False;225;YPosToCameraNearDistance;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-3096.489,961.6898;Inherit;False;Property;_CameraCaptureHeight;CameraCaptureHeight;11;0;Create;True;0;0;0;False;0;False;10;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;217;-2535.37,112.1368;Inherit;False;DisplacementTexValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;108;-2882.3,760.877;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;218;-2599.344,993.1692;Inherit;False;217;DisplacementTexValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;107;-2790.97,1160.576;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;111;-2511.25,1078.517;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;110;-2617.224,775.1974;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-2279.951,949.5762;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;113;-2125.018,946.2362;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;254;-4291.719,-5634.737;Inherit;False;2415.924;1187.086;;2;256;255;Splats;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;144;-2039.774,851.1921;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;187;-4607.552,-3124.875;Inherit;False;1831.876;973.5203;Snow Color lerped between the un-displaced and displaced snow textures.;14;58;185;80;43;78;86;303;305;56;24;57;27;79;304;Snow Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;255;-3140.924,-5565.873;Inherit;False;978.9736;498.8228;;6;300;299;297;278;273;271;Ground;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;256;-3118.842,-5000.014;Inherit;False;910.3767;483.6118;;6;296;295;291;279;274;272;Rock;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-4545.191,-2300.281;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;56;-4489.671,-2866.707;Inherit;True;Property;_SnowTex;SnowTex;4;0;Create;True;0;0;0;False;0;False;0e16611dc4905bc40b3602e5077d5d94;61d722b8b3707b8489b91ed4af7c8918;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;272;-3075.293,-4928.512;Inherit;True;Property;_Splat2;Splat2;16;0;Create;True;0;0;0;False;0;False;None;9d2a3cc38fc249747b99b3eee6df014c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;80;-4471.064,-2644.393;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-1877.799,845.0496;Inherit;False;DisplacementPercent;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;271;-3094.266,-5501.122;Inherit;True;Property;_Splat1;Splat1;18;0;Create;True;0;0;0;False;0;False;None;0fb8c73ff20ed414fbb3b14b5850a734;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;78;-4557.552,-2499.572;Inherit;True;Property;_DisplacedSnowTex;DisplacedSnowTex;7;0;Create;True;0;0;0;False;0;False;None;58c48ac2ea6b10a439f0e9c5244c75e1;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;185;-3733.669,-2322.547;Inherit;False;183;DisplacementPercent;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;79;-4251.706,-2496.995;Inherit;True;Property;_TextureSample12;Texture Sample 12;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;72;-3568.674,-747.059;Inherit;False;Constant;_VoronoiScale;VoronoiScale;7;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;57;-4226.714,-2872.664;Inherit;True;Property;_TextureSample2;Texture Sample 2;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;274;-2782.038,-4927.29;Inherit;True;Property;_TextureSample14;Texture Sample 14;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-4212.733,-3074.875;Inherit;False;Property;_MainColor;MainColor;0;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.854902,0.9660377,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;273;-2801.014,-5499.899;Inherit;True;Property;_TextureSample16;Texture Sample 16;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;278;-2462.692,-5498.797;Inherit;False;GroundTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;190;-4239.197,-1928.377;Inherit;False;1212.617;747.1235;Snow normal lerped between the un-displaced and displaced snow textures.;8;191;118;121;75;184;81;76;117;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;279;-2443.716,-4926.187;Inherit;False;RockTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;277;-3444.756,-4005.868;Inherit;False;GroundTextureUsage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;71;-3580.214,-1009.025;Inherit;True;0;2;1;4;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;275;-3448.756,-3925.869;Inherit;False;RockTextureUsage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-3865.934,-3044.827;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;86;-3749.034,-2543.18;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;276;-3450.756,-4082.868;Inherit;False;SnowTextureUsage;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3397.178,-927.4188;Inherit;False;Property;_VoronoiStrength;VoronoiStrength;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;117;-4174.878,-1874.187;Inherit;True;Property;_SnowNormal;SnowNormal;6;0;Create;True;0;0;0;False;0;False;None;a4e60d649c7f3f444b01ffe03e9d6658;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-4189.197,-1681.324;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;76;-4160.573,-1529.369;Inherit;True;Property;_DisplacedSnowNormal;DisplacedSnowNormal;8;0;Create;True;0;0;0;False;0;False;None;a4e60d649c7f3f444b01ffe03e9d6658;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;282;-2711.639,-4163.953;Inherit;False;278;GroundTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;281;-2381.112,-3864.393;Inherit;False;275;RockTextureUsage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;-2379.386,-4058.311;Inherit;False;277;GroundTextureUsage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;303;-3189.67,-2883.721;Inherit;False;276;SnowTextureUsage;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-3420.671,-2974.775;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-3365.75,-1029.56;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;283;-2711.638,-3969.075;Inherit;False;279;RockTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;-3233.226,-1037.717;Inherit;False;Voronoi;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;75;-3923.574,-1528.669;Inherit;True;Normal From Texture;-1;;23;9728ee98a55193249b513caf9a0f1676;13,149,0,147,0,143,0,141,0,139,0,151,0,137,0,153,0,159,0,157,0,155,0,135,0,108,0;4;87;SAMPLER2D;0;False;85;FLOAT2;0,0;False;74;SAMPLERSTATE;0;False;91;FLOAT;1.5;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;-3517.753,-1409.302;Inherit;False;183;DisplacementPercent;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;304;-3156.378,-3000.041;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;121;-3894.715,-1878.377;Inherit;True;Normal From Texture;-1;;22;9728ee98a55193249b513caf9a0f1676;13,149,0,147,0,143,0,141,0,139,0,151,0,137,0,153,0,159,0,157,0,155,0,135,0,108,0;4;87;SAMPLER2D;0;False;85;FLOAT2;0,0;False;74;SAMPLERSTATE;0;False;91;FLOAT;1.5;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;257;-4192.234,-3769.914;Inherit;False;901.6064;379.139;;3;290;289;288;ControlMap 2;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;284;-2353.662,-3984.432;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;234;-2441.514,-2595.779;Inherit;False;1182.921;316.4538;Comment;9;209;74;70;192;77;232;206;15;307;Final Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;285;-2352.936,-4178.352;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;286;-2138.895,-3987.82;Inherit;False;RockColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-1699.798,1045.873;Inherit;False;202;TotalSnowHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;258;-1643.472,-3917.568;Inherit;False;1585.724;1131.139;Final Color;8;267;265;264;263;262;261;260;259;Final Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;287;-2145.895,-4183.82;Inherit;False;GroundColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;118;-3518.274,-1625.821;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;209;-2360.796,-2477.586;Inherit;False;207;Voronoi;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;305;-2985.806,-3002.315;Inherit;False;SnowColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;206;-2391.514,-2394.726;Inherit;False;201;NoiseSnowHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;288;-4163.324,-3719.732;Inherit;True;Property;_MetallicGlossMap;MetallicGlossMap;12;0;Create;True;0;0;0;False;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;289;-3916.425,-3720.132;Inherit;True;Property;_TextureSample15;Texture Sample 15;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;260;-1625.76,-3646.862;Inherit;False;305;SnowColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1667.353,942.8319;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-2148.075,-2457.423;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-3227.181,-1626.818;Inherit;False;SnowNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;29;-1598.636,731.9095;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;261;-1637.76,-3811.862;Inherit;False;287;GroundColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;259;-1625.76,-3729.862;Inherit;False;286;RockColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;263;-1381.76,-3750.862;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-811.3335,157.6044;Inherit;False;301;ControlColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;290;-3577.256,-3721.536;Inherit;False;Control2Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;262;-1291.649,-3491.972;Inherit;False;Property;_ControlView;ControlView;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-1408.472,-3601.929;Inherit;False;301;ControlColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1354.631,819.4614;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;70;-1996.59,-2453.769;Inherit;False;Normal From Height;-1;;24;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;-1908.138,-2545.779;Inherit;False;191;SnowNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;265;-1207.694,-3618.492;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;230;-1177.672,812.6375;Inherit;False;SnowWorldPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;307;-1664.886,-2414.878;Inherit;False;1;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;-826.0646,10.53999;Inherit;False;290;Control2Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;213;-584.6219,160.7816;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;240;-586.8648,17.03999;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;232;-1483.392,-2528.561;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-102.1254,228.7193;Inherit;False;Constant;_AlphaClip;Alpha Clip;19;0;Create;True;0;0;0;False;0;False;0.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;231;-129.0904,309.4058;Inherit;False;230;SnowWorldPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-989.4838,-3624.802;Inherit;False;FinalColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;242;-287.7461,289.6672;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;59;-607.1797,-133.3555;Inherit;False;Constant;_Emission;Emission;8;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WireNode;246;51.25391,295.6672;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1640.148,-2516.123;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;296;-2787.531,-4724.899;Inherit;True;Property;_TextureSample17;Texture Sample 17;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;295;-2441.216,-4723.649;Inherit;False;RockNormal;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;302;-156.4719,223.8022;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;291;-3070.08,-4727.146;Inherit;True;Property;_Normal2;Normal2;22;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.GetLocalVarNode;233;-118.8053,-6.440423;Inherit;False;232;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;236;-188.0164,147.5003;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;299;-2460.191,-5296.26;Inherit;False;GroundNormal;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;35;-3957.868,-945.7463;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;245;30.25391,247.6672;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;297;-3086.903,-5297.606;Inherit;True;Property;_Normal1;Normal1;19;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;241;-190.292,93.20966;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;-115.3329,-171.2112;Inherit;False;267;FinalColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;235;-173.7163,57.80025;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;300;-2804.356,-5295.36;Inherit;True;Property;_TextureSample18;Texture Sample 18;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;20;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;18;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;17;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;19;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;21;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;15;-1314.673,-2343.627;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;16;140.4,-1.3;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;SnowShader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;False;0;Standard;0;0;Standard;38;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Fragment Normal Space,InvertActionOnDeselection;0;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;0;  Use Shadow Threshold;0;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;0;_FinalColorxAlpha;0;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;1;  Phong;0;  Strength;0,False,34;  Type;1;  Tess;6,False,-1;  Min;30,False,-1;  Max;40,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Write Depth;0;  Early Z;0;Vertex Position,InvertActionOnDeselection;1;0;8;False;True;False;True;True;True;True;True;True;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;0,0;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.CommentaryNode;219;-4183.204,-321.4637;Inherit;False;799.1575;100;The snow height is independent from the Displacement, we displace the snow height basically.;0;;1,1,1,1;0;0
WireConnection;269;0;268;0
WireConnection;301;0;269;0
WireConnection;26;0;45;0
WireConnection;26;1;251;0
WireConnection;211;0;210;0
WireConnection;23;0;26;0
WireConnection;23;1;25;0
WireConnection;198;1;211;1
WireConnection;198;2;163;0
WireConnection;33;0;23;0
WireConnection;33;1;198;0
WireConnection;33;2;154;0
WireConnection;200;1;211;2
WireConnection;200;2;165;0
WireConnection;201;0;33;0
WireConnection;156;0;155;0
WireConnection;156;1;200;0
WireConnection;53;0;49;0
WireConnection;32;0;156;0
WireConnection;32;1;205;0
WireConnection;32;2;148;0
WireConnection;223;0;250;2
WireConnection;124;0;53;1
WireConnection;202;0;32;0
WireConnection;55;0;53;0
WireConnection;55;1;124;0
WireConnection;139;0;204;0
WireConnection;139;1;250;2
WireConnection;104;0;224;0
WireConnection;104;1;101;0
WireConnection;48;0;47;0
WireConnection;48;1;55;0
WireConnection;225;0;104;0
WireConnection;220;0;139;0
WireConnection;143;0;48;1
WireConnection;217;0;143;0
WireConnection;108;0;221;0
WireConnection;108;1;102;0
WireConnection;107;0;226;0
WireConnection;107;1;102;0
WireConnection;111;0;218;0
WireConnection;111;1;107;0
WireConnection;110;0;108;0
WireConnection;110;1;107;0
WireConnection;112;0;111;0
WireConnection;112;1;110;0
WireConnection;113;0;112;0
WireConnection;144;0;113;0
WireConnection;183;0;144;0
WireConnection;79;0;78;0
WireConnection;79;1;58;0
WireConnection;57;0;56;0
WireConnection;57;1;80;0
WireConnection;274;0;272;0
WireConnection;273;0;271;0
WireConnection;278;0;273;0
WireConnection;279;0;274;0
WireConnection;277;0;269;2
WireConnection;71;0;26;0
WireConnection;71;2;72;0
WireConnection;275;0;269;3
WireConnection;43;0;24;1
WireConnection;43;1;24;2
WireConnection;43;2;24;3
WireConnection;86;0;79;0
WireConnection;86;1;57;0
WireConnection;86;2;185;0
WireConnection;276;0;269;1
WireConnection;27;0;43;0
WireConnection;27;1;86;0
WireConnection;73;0;71;0
WireConnection;73;1;82;0
WireConnection;207;0;73;0
WireConnection;75;87;76;0
WireConnection;75;85;81;0
WireConnection;304;1;27;0
WireConnection;304;2;303;0
WireConnection;121;87;117;0
WireConnection;121;85;81;0
WireConnection;284;1;283;0
WireConnection;284;2;281;0
WireConnection;285;1;282;0
WireConnection;285;2;280;0
WireConnection;286;0;284;0
WireConnection;287;0;285;0
WireConnection;118;0;121;40
WireConnection;118;1;75;40
WireConnection;118;2;184;0
WireConnection;305;0;304;0
WireConnection;289;0;288;0
WireConnection;50;0;203;0
WireConnection;50;1;113;0
WireConnection;74;0;209;0
WireConnection;74;1;206;0
WireConnection;191;0;118;0
WireConnection;263;0;261;0
WireConnection;263;1;259;0
WireConnection;263;2;260;0
WireConnection;290;0;289;0
WireConnection;30;0;29;0
WireConnection;30;1;50;0
WireConnection;70;20;74;0
WireConnection;265;0;263;0
WireConnection;265;1;264;0
WireConnection;265;2;262;0
WireConnection;230;0;30;0
WireConnection;307;0;192;0
WireConnection;307;1;70;40
WireConnection;213;0;212;0
WireConnection;240;0;239;0
WireConnection;232;0;307;0
WireConnection;267;0;265;0
WireConnection;242;0;213;0
WireConnection;242;1;213;1
WireConnection;242;2;213;2
WireConnection;242;3;213;3
WireConnection;246;0;231;0
WireConnection;77;0;192;0
WireConnection;77;1;70;40
WireConnection;296;0;291;0
WireConnection;295;0;296;0
WireConnection;302;0;242;0
WireConnection;236;0;213;3
WireConnection;299;0;300;0
WireConnection;35;0;26;0
WireConnection;245;0;174;0
WireConnection;241;0;240;0
WireConnection;235;0;59;0
WireConnection;300;0;297;0
WireConnection;16;0;196;0
WireConnection;16;1;233;0
WireConnection;16;2;235;0
WireConnection;16;3;241;0
WireConnection;16;4;236;0
WireConnection;16;6;302;0
WireConnection;16;7;245;0
WireConnection;16;8;246;0
ASEEND*/
//CHKSM=03DBCE86962661F61087261F1DFB704475D09B4F