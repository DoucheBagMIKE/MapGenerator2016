Shader "prime[31]/Sprite Light Kit/Mesh Light"
{
	Properties
	{
		 //_MainTex ( "Sprite Texture", 2D ) = "white" {}
		_Color ( "Tint", Color ) = ( 1, 1, 1, 1 )
		[MaterialToggle] PixelSnap ( "Pixel snap", Float ) = 0
		[Enum( UnityEngine.Rendering.BlendMode )] _BlendSrc ( "Blend Source", Float ) = 5
		[Enum( UnityEngine.Rendering.BlendMode )] _BlendDst ( "Blend Destination", Float ) = 1
		_UVXOffset("UV X Offset", float) = 0			//Receive input from UV coordinate X offset
		_UVYOffset("UV Y Offset", float) = 0			//Receive input from UV coordinate Y offset
		_UVXScale("UV X Scale", float) = 1.0			//Receive input from UV X scale
		_UVYScale("UV Y Scale", float) = 1.0			//Receive input from UV Y scale
		_Offset("Offset", float) = 0					//Receive input from a float
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Cull Off
		Lighting Off
		ZWrite Off

		Blend [_BlendSrc] [_BlendDst]

		Pass
		{
CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

uniform fixed4 _Color;	//Use the Color _Color provided by Unity
uniform float _UVXOffset;
uniform float _UVYOffset;
uniform float _UVXScale;
uniform float _UVYScale;
uniform float _Offset;

struct appdata_t
{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 vertex   : POSITION;
	fixed4 color    : COLOR;
	half2 texcoord  : TEXCOORD0;
}; 

v2f vert( appdata_t IN )
{
	v2f OUT;
	OUT.vertex = mul( UNITY_MATRIX_MVP, IN.vertex );
	OUT.texcoord = half2((IN.texcoord.x + _UVXOffset)*_UVXScale, (IN.texcoord.y + _UVYOffset)*_UVYScale);
	//OUT.color = IN.color * _Color;
	#ifdef PIXELSNAP_ON
	OUT.vertex = UnityPixelSnap( OUT.vertex );
	#endif

	return OUT;
}


fixed4 frag( v2f IN ) : Color
{
	return fixed4(lerp(_Color,fixed4(_Color.rgb,0),sqrt((IN.texcoord.x*IN.texcoord.x) + (IN.texcoord.y*IN.texcoord.y)) + _Offset));
}

ENDCG
		}
	}
}
