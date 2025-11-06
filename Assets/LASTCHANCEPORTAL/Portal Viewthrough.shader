Shader "Custom/Portal Viewthrough"{
    //show values to edit in inspector
    Properties{
        _Color("Tint", Color) = (1, 1, 1, 1)
        _LeftTex("LeftEyeTexture", 2D) = "white" {}
        _RightTex("RightEyeTexture", 2D) = "white" {}
    }

        SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

        Pass{
            CGPROGRAM
            
            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //texture and transforms of the texture
            sampler2D _LeftTex;
            float4 _LeftTex_ST;

            sampler2D _RightTex;
            float4 _RightTex_ST;

            //tint of the texture
            fixed4 _Color;

            //the object data that's put into the vertex shader
            struct appdata {
                float4 vertex : POSITION;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f {
                float4 position : SV_POSITION;
                float4 screenPosition : TEXCOORD0;
            };
            
            struct fragOut
            {
                fixed4 color : SV_TARGET;
            };

            //the vertex shader
            v2f vert(appdata v) {
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.position);
                return o;
            }

            //the fragment shader
            fragOut frag(v2f i) {
                fragOut o;
                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                
                fixed4 col = unity_StereoEyeIndex == 0 ? tex2D(_LeftTex, textureCoordinate) : tex2D(_RightTex, textureCoordinate);
                col *= _Color;
                o.color = col;

                return o;
            }

            ENDCG
        }
    }
}