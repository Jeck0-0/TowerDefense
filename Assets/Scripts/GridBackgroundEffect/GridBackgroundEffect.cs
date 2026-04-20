using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridBackgroundEffect : MonoBehaviour
{
    public ComputeShader computeShader; 
    public Material instanceMaterial;   
    public Mesh mesh;                   

    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public float spacing = 1.0f;
    [ColorPalette] public Color baseColor;
    public AnimationCurve offsetByDistance;
    public AnimationCurve sizeByDistance;
    public AnimationCurve opacityByDistance;
    public float curveMaxDistance;
    public int curvePrecision = 512;

    protected int squareCount;

    struct SquareData
    {
        public Vector3 center;
        public Vector3 currentPosition;
        public float scale;
        public Color color;
    }

    private ComputeBuffer argsBuffer; // vertices data
    private ComputeBuffer squaresBuffer; // squares data
    private ComputeBuffer offsetByDistanceBuffer;
    private ComputeBuffer sizeByDistanceBuffer;
    private ComputeBuffer opacityByDistanceBuffer;
    private ComputeBuffer excludeBuffer;
    
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private int kernelHandle; // reference to the "CSMain" #pragma inside the compute shader
    private int[] excludedIds;
    private Camera cam;
    
    void Awake()
    {
        cam = Camera.main;
        if (mesh == null)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            mesh = temp.GetComponent<MeshFilter>().sharedMesh;
            Destroy(temp);
        }
        squareCount = gridSizeX * gridSizeY;
        excludedIds = new int[squareCount];
    }

    protected virtual void Start()
    {
        InitializeBuffers();
    }

    void InitializeBuffers()
    {
        int count = gridSizeX * gridSizeY;
        
        // GPU memory for the square data
        squaresBuffer = new ComputeBuffer(count, sizeof(float) * 11);

        SquareData[] data = new SquareData[count];
        Vector3 pos = Vector3.zero;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                int index = x + y * gridSizeX;
                pos = new Vector3(x * spacing - (gridSizeX * spacing) / 2, y * spacing - (gridSizeY * spacing) / 2, 0);
                
                data[index] = new SquareData
                {
                    center = pos,
                    currentPosition = pos,
                    scale = .1f,
                    color = baseColor.linear,
                };
            }
        }
        // "SetData" uploads data from the RAM (CPU) to the VRAM (GPU)
        squaresBuffer.SetData(data);

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        if (mesh != null)
        {
            args[0] = (uint)mesh.GetIndexCount(0);  // How many points in one quad?
            args[1] = (uint)count;                          // How many quads to draw?
            args[2] = (uint)mesh.GetIndexStart(0); 
            args[3] = (uint)mesh.GetBaseVertex(0);
        }
        argsBuffer.SetData(args);

        // get the ID of the "CSMain" #pragma inside the compute shader
        kernelHandle = computeShader.FindKernel("CSMain");
        
        // "SetBuffer" links our allocated memory to the variables named "squaresBuffer" in the shaders.
        instanceMaterial.SetBuffer("squaresBuffer", squaresBuffer);
        computeShader.SetBuffer(kernelHandle, "squaresBuffer", squaresBuffer);

        SetBufferFromAnimationCurve(offsetByDistance, out offsetByDistanceBuffer);
        SetBufferFromAnimationCurve(opacityByDistance, out opacityByDistanceBuffer);
        SetBufferFromAnimationCurve(sizeByDistance, out sizeByDistanceBuffer);
        computeShader.SetBuffer(kernelHandle, "offsetByDistance", offsetByDistanceBuffer);
        computeShader.SetBuffer(kernelHandle, "opacityByDistance", opacityByDistanceBuffer);
        computeShader.SetBuffer(kernelHandle, "sizeByDistance", sizeByDistanceBuffer);
        
        computeShader.SetFloat("maxDistance", curveMaxDistance);
        computeShader.SetFloat("curvePrecision", curvePrecision);
        computeShader.SetVector("baseColor", baseColor.linear);
        
        excludeBuffer = new ComputeBuffer(gridSizeX * gridSizeY, sizeof(int));
        excludeBuffer.SetData(excludedIds);
        computeShader.SetBuffer(kernelHandle, "exclude", excludeBuffer);
    }

    void SetBufferFromAnimationCurve(AnimationCurve curve, out ComputeBuffer buffer)
    {
        float[] array = new float[curvePrecision];
        for (int i = 0; i < array.Length; i++)
            array[i] = curve.Evaluate((i / (float) array.Length) * curveMaxDistance);
        
        buffer = new ComputeBuffer(array.Length, sizeof(float));
        buffer.SetData(array);
    }
    
    
    void Update()
    {
        if (mesh == null || instanceMaterial == null || computeShader == null) return;

        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        computeShader.SetVector("mousePosition", mousePos);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        // runs the compute shader
        int count = gridSizeX * gridSizeY;
        int threadGroups = Mathf.CeilToInt(count / 64.0f);
        computeShader.Dispatch(kernelHandle, threadGroups, 1, 1);

        // draws the squares using instructions from argsBuffer and positions from squaresBuffer
        Graphics.DrawMeshInstancedIndirect(mesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100, 100, 100)), argsBuffer);
    }

    void OnDestroy()
    {
        // release GPU memory
        if (squaresBuffer != null) squaresBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();
        if (offsetByDistanceBuffer != null) offsetByDistanceBuffer.Release();
        if (sizeByDistanceBuffer != null) sizeByDistanceBuffer.Release();
        if (opacityByDistanceBuffer != null) opacityByDistanceBuffer.Release();
        if (excludeBuffer != null) excludeBuffer.Release();
    }

    
    bool updatedExcludeBuffer = false;
    private void LateUpdate()
    {
        if (updatedExcludeBuffer)
        {
            updatedExcludeBuffer = false;
            excludeBuffer.SetData(excludedIds);
            computeShader.SetBuffer(kernelHandle, "exclude", excludeBuffer);
        }
    }


    public void SetExcludedIndex(int index, bool excluded)
    {
        if(excludedIds.Length <= index || index < 0 || excludedIds[index] == (excluded ? 1 : 0))
            return;
        
        excludedIds[index] = excluded ? 1 : 0;
        updatedExcludeBuffer = true;
    }
}
