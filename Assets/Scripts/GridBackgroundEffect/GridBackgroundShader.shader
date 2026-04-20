Shader "Custom/GridBackground"
{
    
    Properties // inspector variables
    {
        // Internal Name ("Inspector Label", Type) = Default Value
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("Scale", Float) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            // "CGPROGRAM" marks the beginning of the shader code itself (written in HLSL/Cg).
            // Everything before this was "ShaderLab" (Unity's custom wrapper language).
            CGPROGRAM

            // vertex stage calculates the position of the mesh's corners on the 2D screen.
            #pragma vertex vert
            
            // fragment stage determines the color of each pixel on the screen
            #pragma fragment frag

            // allows drawing thousands of squares at once
            #pragma multi_compile_instancing

            // minimum shader model required by the GPU. 4.5 allows using StructuredBuffer (Compute Buffers).
            #pragma target 4.5

            // math, lighting and helper functions library by Unity 
            #include "UnityCG.cginc"

            // data coming directly from the 3D model (Mesh)
            struct appdata
            {
                // POSITION = 3D local coordinates of the vertex
                float4 vertex : POSITION;
                // TEXCOORD0 = first set of UV coordinates
                float2 uv : TEXCOORD0;
                // "SV_InstanceID" is a special system variable. 
                // When drawing 1000 squares, the first square gets ID 0, the second gets ID 1, etc.
                uint instanceID : SV_InstanceID;
            };

            struct v2f // vertex to fragment
            {
                float2 uv : TEXCOORD0; // UV coords?
                float4 vertex : SV_POSITION; // final 2D screen coords
                float4 color : COLOR;
            };

            // This struct must match the memory layout of the one in the C# script and Compute Shader.
            struct SquareData
            {
                float3 center;
                float3 currentPosition;
                float scale;
                float4 color;
            };

            // "StructuredBuffer" is a Read-Only array stored on the GPU.
            // This is how we access the positions and rotations calculated by our Compute Shader.
            StructuredBuffer<SquareData> squaresBuffer;

            // These variables are linked to the "Properties" block at the top of the file.
            // "sampler2D" is a special type used for sampling (reading) pixels from a texture.
            sampler2D _MainTex;
            // "[Name]_ST" is a naming convention Unity uses to store Tiling (Scale) and Offset (Translate).
            float4 _MainTex_ST; 
            float4 _Color;
            float _Scale;

            // The Vertex Shader function.
            // It runs once for every single corner (vertex) of the mesh.
            v2f vert (appdata v)
            {
                v2f o;
                SquareData data = squaresBuffer[v.instanceID];

                // 2. Local Scale: Shrink or grow the vertex before moving it.
                float3 scaledVertex = v.vertex.xyz * data.scale;

                // 4. World Position: Offset the rotated square by its world position from the buffer.
                float3 worldPos = scaledVertex + data.currentPosition;

                // 5. Projection: "UnityObjectToClipPos" is a built-in function that converts 
                // a 3D coordinate into a 2D coordinate on your screen, accounting for the camera's perspective.
                o.vertex = UnityObjectToClipPos(float4(worldPos, 1.0));
                
                // 6. UV Transformation: "TRANSFORM_TEX" handles the tiling and offset settings for the texture.
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = data.color * _Color;
                
                return o;
            }

            // The Fragment (Pixel) Shader function.
            // It runs once for every single pixel covered by the triangles of our mesh.
            // "SV_Target" is a semantic telling the GPU: "Output this final color to the screen."
            fixed4 frag (v2f i) : SV_Target
            {
                // "tex2D" reads the color of the texture at the specific UV coordinate.
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            
            ENDCG
        }
    }
}
