using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Voxel;

public class NoiseVisualize : MonoBehaviour
{
    Material material;
    ComputeBuffer noiseOut, argsBuf;
    ComputeShader noiseCS;
    public Mesh mesh;
    public float isolevel = 0;
    int size;

    MCJobs mcj = new MCJobs();

    // Start is called before the first frame update
    void Start()
    {


    }

    

    // Update is called once per frame
    void Update()
    {
        mcj.ComputeNoise(new Octree(32, Vector3.zero));

        //material.SetFloat("_isolevel", isolevel);
        //Graphics.DrawMeshInstancedProcedural(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * size), size * size * size);
    }


    void InitShader() {
        material = new Material(Shader.Find("My Pipeline/MC"));
        material.SetPass(0);
        size = 64;
        noiseOut = new ComputeBuffer(size * size * size, (sizeof(float) * 4));
        noiseCS = (ComputeShader)Resources.Load("Perlin");

        material.SetBuffer("_data", noiseOut);


        noiseCS.SetBuffer(0, "output", noiseOut);
        noiseCS.SetInt("sampleSize", size);
        noiseCS.SetVector("minCorner", new float4(Vector3.zero - Vector3.one * size / 2, 0));
        int groupSize = size / 8;
        noiseCS.Dispatch(0, groupSize + 1, groupSize + 1, groupSize + 1);
    }
}
