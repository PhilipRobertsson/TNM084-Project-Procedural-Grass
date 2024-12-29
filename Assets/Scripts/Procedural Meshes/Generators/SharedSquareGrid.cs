using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators{

    public struct SharedSquareGrid : IMeshGenerator{
        public int VertexCount => (Resolution + 1) * (Resolution + 1);
        public int IndexCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution + 1;
        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams {
            int vi = (Resolution + 1) * z, ti = 2 * Resolution * (z - 1); ;
            var vertex = new Vertex();

            vertex.normal.y = 1f;
            vertex.tangent.xw = half2(half(1f), half(-1f));

            vertex.position.x = -0.5f;
            vertex.position.z = (float)z / Resolution - 0.5f;
            vertex.texCoord0.y = half((float)z / Resolution);
            streams.SetVertex(vi, vertex);

            vi += 1;

            for (int x = 1; x <= Resolution; x++, vi++, ti+=2){
                vertex.position.x = (float)x / Resolution - 0.5f;
                vertex.texCoord0.x = half((float)x / Resolution);
                streams.SetVertex(vi, vertex);

                if (z > 0){
                    streams.SetTriangle(ti + 0, vi + int3(-Resolution - 2, -1, -Resolution - 1));
                    streams.SetTriangle(ti + 1, vi + int3(-Resolution - 1, -1, 0));
                }
            }
        }
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1.0f, 0.0f,1.0f));
        public int Resolution { get; set; }
    }
}
