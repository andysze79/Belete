// Version for 2018.3


#ifndef UNITY_STANDARD_META_INCLUDED
#define UNITY_STANDARD_META_INCLUDED

// Functionality for Standard shader "meta" pass
// (extracts albedo/emission for lightmapper etc.)

// define meta pass before including other files; they have conditions
// on that in some places
#define UNITY_PASS_META 1

#include "UnityCG.cginc"
//#include "UnityStandardInput.cginc"
#include "../Lux Core/Lux Setup/LuxInputs.cginc"

//#include "../Lux Core/Lux Setup/LuxStructs.cginc"
//#include "../Lux Core/Lux Setup/LuxInputs.cginc"

#include "UnityMetaPass.cginc"
#include "Lux Plus StandardCore.cginc"

struct v2f_meta
{
    float4 pos      : SV_POSITION;
    float4 uv       : TEXCOORD0;
    #if UNITY_VERSION >=201801
        #ifdef EDITOR_VISUALIZATION
            float2 vizUV        : TEXCOORD1;
            float4 lightCoord   : TEXCOORD2;
        #endif
    #endif
    half3 normalWorld : TEXCOORD3;
    float4 posWorld : TEXCOORD4;
    half3 viewDir   : TEXCOORD5;
    fixed4 color    : COLOR0;
};

// Lux: custom vertex input structure
v2f_meta vert_meta (LuxVertexInput v)
{
    v2f_meta o;
    o.pos = UnityMetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);

    #if UNITY_VERSION >=201801
        #ifdef EDITOR_VISUALIZATION
            o.vizUV = 0;
            o.lightCoord = 0;
            if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
                o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.uv0.xy, v.uv1.xy, v.uv2.xy, unity_EditorViz_Texture_ST);
                //o.vizUV = UnityMetaVizUV(2, v.uv0.xy, v.uv1.xy, v.uv2.xy, unity_EditorViz_Texture_ST);
            else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
            {
                o.vizUV = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
            }
        #endif
    #endif


//  Lux
    o.uv = LuxTexCoords(v);
    o.color = v.color;
    o.normalWorld = UnityObjectToWorldNormal(v.normal);
    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.posWorld.xyz = posWorld.xyz;
    o.posWorld.w = distance(_WorldSpaceCameraPos, posWorld);
    #if defined (_PARALLAXMAP)
        TANGENT_SPACE_ROTATION;
        o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
    #else
        o.viewDir = 0;
    #endif
//  End Lux

    

    return o;
}

// Albedo for lightmapping should basically be diffuse color.
// But rough metals (black diffuse) still scatter quite a lot of light around, so
// we want to take some of that into account too.
half3 UnityLightmappingAlbedo (half3 diffuse, half3 specular, half oneMinusRoughness)
{
    half roughness = 1 - oneMinusRoughness;
    half3 res = diffuse;
    res += specular * roughness * roughness * 0.5;
    return res;
}
#if UNITY_VERSION >=201701
    
    // ----------------------
    // 2018 +
    #if UNITY_VERSION >=201801
        float4 frag_meta (v2f_meta i) : SV_Target
        {
            // we're interested in diffuse & specular colors,
            // and surface roughness to produce final albedo.
            // FragmentCommonData data = UNITY_SETUP_BRDF_INPUT (i.uv);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

            FRAGMENT_META_SETUP(data)

        #ifdef EDITOR_VISUALIZATION
            o.Albedo = data.diffColor;
            o.VizUV = i.vizUV;
            o.LightCoord = i.lightCoord;
        #else
            o.Albedo = UnityLightmappingAlbedo (data.diffColor, data.specColor, data.oneMinusRoughness);
        #endif
            o.SpecularColor = data.specColor;
            o.Emission = data.emission;
            return UnityMetaFragment(o);
        }
    
    // ----------------------
    // 2017 +
    #else
        float4 frag_meta (v2f_meta i) : SV_Target
        {
            // we're interested in diffuse & specular colors,
            // and surface roughness to produce final albedo.
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

            FRAGMENT_META_SETUP(data)

        #if defined(EDITOR_VISUALIZATION)
            o.Albedo = data.diffColor;
        #else
            o.Albedo = UnityLightmappingAlbedo (data.diffColor, data.specColor, data.smoothness);
        #endif
            o.SpecularColor = data.specColor;
            o.Emission = Emission(i.uv.xy);

            return UnityMetaFragment(o);
        }
    #endif

    // ----------------------
    // Unity 5+
    #else

    float4 frag_meta (v2f_meta i) : SV_Target
    {
        // we're interested in diffuse & specular colors,
        // and surface roughness to produce final albedo.
    //  FragmentCommonData s = UNITY_SETUP_BRDF_INPUT (i.uv);

        UnityMetaInput o;
        UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

        FRAGMENT_META_SETUP(data)

        o.Albedo = UnityLightmappingAlbedo (data.diffColor, data.specColor, data.oneMinusRoughness);
        o.Emission = data.emission;

        return UnityMetaFragment(o);
    }

#endif

#endif // UNITY_STANDARD_META_INCLUDED