
//The second shader will pass only for the pixels which the first(red) shader passed, because it is checking for equality with the value ‘2’.It will also decrement the value in the buffer wherever it fails the Z test.

Shader "Custom/StencilTest2" {
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
		Pass{
		Stencil{
		Ref 2
		Comp equal
		Pass keep
		ZFail decrWrap
	}

		CGPROGRAM
#include "UnityCG.cginc"
#pragma vertex vert
#pragma fragment frag
	struct appdata {
		float4 vertex : POSITION;
	};
	struct v2f {
		float4 pos : SV_POSITION;
	};
	v2f vert(appdata v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}
	half4 frag(v2f i) : SV_Target{
		return half4(0,1,0,1);
	}
		ENDCG
	}
	}
}