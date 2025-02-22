Shader "Hidden/Custom/NeoGlitch"
{
    Properties
    {
        _MainTex("Source", 2D) = "white" {}
        _GlitchIntensity("Glitch Intensity", Range(0, 1)) = 0.5
        _GlitchFrequency("Glitch Frequency", Range(0, 10)) = 1.0
        _GlitchDisplacement("Glitch Displacement", Range(0, 0.2)) = 0.05
        _GlitchDirection("Glitch Direction", Vector) = (1, 0, 0, 0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
            Pass
            {
                Name "NeoGlitch"
                ZTest Always Cull Off ZWrite Off

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_TexelSize;
                float _GlitchIntensity;
                float _GlitchFrequency;
                float _GlitchDisplacement;
                float2 _GlitchDirection;
                // Do not redeclare _Time - Unity provides it automatically.
                // float _Time;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                // A simple pseudo-random function based on UV coordinates.
                float random2d(float2 st)
                {
                    return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;

                    // Divide the screen into small blocks to create discrete glitch segments.
                    float blockSize = 0.05; // Adjust the block size as needed.
                    float2 blockUV = floor(uv / blockSize) * blockSize;

                    // Use Unity's built-in _Time.x for time (in seconds)
                    float time = _Time.x;

                    // Generate a random value per block, animated over time.
                    float rand = random2d(blockUV + time * _GlitchFrequency);

                    // Create a glitch modulation using a sine wave based on time and the vertical UV coordinate.
                    float glitchFactor = sin(time * _GlitchFrequency + uv.y * 100.0) * _GlitchIntensity;

                    // Compute the offset for the glitch effect using the specified direction and displacement.
                    float2 offset = _GlitchDirection * _GlitchDisplacement * rand * glitchFactor;

                    // Apply the offset to the UV coordinates.
                    uv += offset;

                    // Sample the source texture with the modified UVs.
                    fixed4 col = tex2D(_MainTex, uv);
                    //return col;
                    return fixed4(1, 0, 0, 1);
                }
                ENDHLSL
            }
        }
            FallBack "Hidden/BlitCopy"
}
