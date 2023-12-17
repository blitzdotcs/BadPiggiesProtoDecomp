Shader "e2d/Fill" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Opaque" }
  ZWrite Off
  Cull Off
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant }
 }
}
}