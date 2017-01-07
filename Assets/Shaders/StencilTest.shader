Shader "Custom/StencilTest" {
	SubShader{
		Tags{ "Queue" = "Geometry" }

		Pass{
		ColorMask 0
		Stencil{
		Ref 1
		Comp always
		Pass replace

	}
	}
	}
}