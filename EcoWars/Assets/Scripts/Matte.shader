 //name "customShader" whatever you like to name it
 
 Shader "Custom/customShader" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Main textyre", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _Transparency ("Transparency", Range(0.0,1.0)) = 1
     }
     SubShader {
         Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha
         
         CGPROGRAM
         // Physically based Standard lighting model, and enable shadows on all light types
         #pragma surface surf Standard alpha
 
         sampler2D _MainTex;
 
         struct Input {
             float2 uv_MainTex;
         };
 
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
         half _Transparency;
 
         void surf (Input IN, inout SurfaceOutputStandard o) {
             // Albedo comes from a texture tinted by color
             fixed4 alpha = fixed4(1, 1, 1, _Transparency);
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * alpha * _Color;
             o.Albedo = c.rgb;
             o.Alpha =  alpha.a;
             // Metallic and smoothness come from slider variables
             o.Smoothness = _Glossiness;
             o.Metallic = _Metallic;
         }
         ENDCG
     }
     FallBack "Diffuse"
 }