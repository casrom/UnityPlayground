Shader "My Pipeline/MC" {

Properties{
			_isolevel("iso-level",float) = 0
}
	SubShader{

		Pass {
			HLSLPROGRAM
			#pragma target 4.5
			//#pragma vertex UnlitPassVertex

			#pragma vertex vert
			//#pragma geometry geo
			#pragma fragment frag

			//#pragma surface surf Standard vertex:vert addshadow
			#include "../ShaderLibrary/MC.hlsl"

			ENDHLSL
		}
	}
	FallBack"HDRP/Lit"
}