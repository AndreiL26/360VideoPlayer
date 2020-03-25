// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Skybox/Panoramic" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    [NoScaleOffset] _MainTex ("Spherical  (HDR)", 2D) = "grey" {}
    [KeywordEnum(6 Frames Layout, Latitude Longitude Layout)] _Mapping("Mapping", Float) = 1
    [Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType("Image Type", Float) = 0
    [Toggle] _MirrorOnBack("Mirror on Back", Float) = 0
    [Enum(None, 0, Side by Side, 1, Over Under, 2)] _Layout("3D Layout", Float) = 0
    _SelectedRegionSizeX ("Selected Region SizeX", Float) = 0
    _SelectedRegionSizeY ("Selected Region SizeY", Float) = 0
    _SelectedRegionCenterX ("Selected Region CenterX", Float) = 0
    _SelectedRegionCenterY ("Selected Region CenterY", Float) = 0
    _Data("_Data", Float) = 0
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        #pragma multi_compile_local __ _MAPPING_6_FRAMES_LAYOUT

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        half4 _MainTex_HDR;
        half4 _Tint;
        half _Exposure;
        float _Rotation;
        float _SelectedRegionSizeX;
        float _SelectedRegionSizeY;
        float _SelectedRegionCenterX;
        float _SelectedRegionCenterY;

        int _RegionsSize;
        uniform float _RegionsData[40];

        inline float2 ToRadialCoords(float3 coords)
        {
            float3 normalizedCoords = normalize(coords);
            float latitude = acos(normalizedCoords.y);
            float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
            float2 sphereCoords = float2(longitude, latitude) * float2(0.5/UNITY_PI, 1.0/UNITY_PI);
            return float2(0.5,1.0) - sphereCoords;
        }

        float3 RotateAroundYInDegrees (float3 vertex, float degrees)
        {
            float alpha = degrees * UNITY_PI / 180.0;
            float sina, cosa;
            sincos(alpha, sina, cosa);
            float2x2 m = float2x2(cosa, -sina, sina, cosa);
            return float3(mul(m, vertex.xz), vertex.y).xzy;
        }

        struct appdata_t {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct Region
        {
            float2 center;
            float2 size;
        };

        v2f vert (appdata_t v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
            o.vertex = UnityObjectToClipPos(rotated);
            o.texcoord = v.vertex.xyz;
           
            return o;
        }

        bool checkBox(float2 pt, Region reg)
        {
            if ((abs(pt.x - reg.center.x) < abs(reg.size.x)) &&
                (abs(pt.y - reg.center.y) < abs(reg.size.y)))
                return true;
            else 
                return false;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            float2 tc = ToRadialCoords(i.texcoord);

            half4 tex = tex2D (_MainTex, tc);

			// Iterate over all the 'active' regions and apply the shader transormation
            for (int i = 0; i < _RegionsSize; i++)
            {
                Region curr_reg;
                curr_reg.center.x = _RegionsData[i * 4 + 0];
                curr_reg.center.y = _RegionsData[i * 4 + 1];
                curr_reg.size.x   = _RegionsData[i * 4 + 2];
                curr_reg.size.y   = _RegionsData[i * 4 + 3];

                if (checkBox(tc, curr_reg))
                {
					// Simple highlight effect
                    float2 r = abs(tc-curr_reg.center)/curr_reg.size;
                    float col = sin(r.y + r.x*3. -_Time * 4.0 *9.) * 0.9;
                    col *= col * col * 0.6;
        
                    col = clamp(col, 0., 1.);
                    tex += half4(col, col, col, col);
                    break;
                }
            }

            return half4(tex);
        }
        ENDCG
    }
}


CustomEditor "SkyboxPanoramicShaderGUI"
Fallback Off

}
