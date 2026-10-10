// Lambert-lit vertex colour × tint (terrain, walker parts). Lives in Resources so player builds include it.
Shader "Lab/VertexColorLit"
{
    Properties { _Color ("Tint", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input { float4 color : COLOR; };
        fixed4 _Color;
        void surf (Input IN, inout SurfaceOutput o) { o.Albedo = IN.color.rgb * _Color.rgb; }
        ENDCG
    }
    FallBack "Diffuse"
}
