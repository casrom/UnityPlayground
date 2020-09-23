using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Voxel {
    class MCJobs {

        public void ComputeNoise(Octree node) {
            int size = (int)node.bounds.size.x;
            //size = 2;

            NativeArray<float4> result = new NativeArray<float4>(size * size * size, Allocator.TempJob);

            NoiseJob jobData = new NoiseJob();
            jobData.size = size;
            jobData.noiseScale = 10f;
            jobData.minCorner = node.bounds.min;
            jobData.noiseData = result;

            JobHandle handle = jobData.Schedule();
            handle.Complete();
            //Debug.Log(string.Join(",", result.ToArray()));
            Debug.Log(result.Length);
            result.Dispose();

        }

        public struct NoiseJob : IJob {
            public int size;
            public float noiseScale;
            public Vector3 minCorner;
            public NativeArray<float4> noiseData;


            public void Execute() {
                for (int x = 0; x < size; x++) {
                    for (int y = 0; y < size; y++) {
                        for (int z = 0; z < size; z++) {

                            float3 location = new float3(x, y, z);
                            float d = y - OctavePerlinNoise(location.xz / noiseScale / 10, 5, .5f) * 100;
                            if (location.y < 0) {
                                //d = location.y - OctavePerlinNoise(location.xz / noiseScale / 10, 5, .3f)*8;
                                d = location.y - OctavePerlinNoise(location.xz / noiseScale / 10, 5, .5f) * 10;
                            }
                            noiseData[CoordToIndex(x, y, z, size)] = new float4(x, y, z, d);
                        }
                    }
                }
                
            }
        }
        
        private static float3 IndexToCoord(int i, int size) {
            return new float3(i / (size * size), (i / size) % size, i % size);
        }

        private static int CoordToIndex(int x, int y, int z, int size) {
            return x * size * size + y * size + z;
        }

        private static float OctavePerlinNoise(float3 location, int octaves, float persistence) {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++) {
                total += Perlin.Noise(location.x * frequency, location.y * frequency, location.z * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        private static float OctavePerlinNoise(float2 location, int octaves, float persistence) {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++) {
                total += Perlin.Noise(location.x * frequency, location.y * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }

}


