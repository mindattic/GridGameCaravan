// Lambert-lit flat colour (the 3D walker's parts). In Resources so player builds include it — the built-in Standard
// shader behind primitives' default material is stripped from players when nothing references it.
Shader "Lab/FlatLit"
{
    Properties { _Color ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input { float3 worldPos; };
        fixed4 _Color;
        void surf (Input IN, inout SurfaceOutput o) { o.Albedo = _Color.rgb; }
        ENDCG
    }
    FallBack "Diffuse"
}
