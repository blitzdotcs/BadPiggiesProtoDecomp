Shader "_Custom/Unlit_ColorTransparent_Geometry" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Material {
   Diffuse [_Color]
  }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant }
 }
}
}