Shader "_Custom/Unlit_Color_Geometry" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
  Material {
   Diffuse [_Color]
  }
  ZWrite Off
  Cull Off
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant }
 }
}
}