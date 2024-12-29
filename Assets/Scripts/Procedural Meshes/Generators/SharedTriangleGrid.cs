using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators{

    public struct SharedTriangleGrid : IMeshGenerator{
        public int VertexCount => (Resolution + 1) * (Resolution + 1);
        public int IndexCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution + 1;
        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams {
            int vi = (Resolution + 1) * z, ti = 2 * Resolution * (z - 1);

            int iA = -Resolution - 2, iB = -Resolution - 1, iC = -1, iD = 0;
            var tA = int3(iA, iC, iD);
            var tB = int3(iA, iD, iB);

            var vertex = new Vertex();

            float xOffset = -0.25f;
            float uOffset = 0f;

            if (z%2 == 0){
                xOffset = 0.25f;
                uOffset = 0.5f / (Resolution + 0.5f);
                tA = int3(iA, iC, iB);
                tB = int3(iB, iC, iD);
            }

            xOffset = xOffset / Resolution - 0.5f;

            vertex.normal.y = 1f;
            vertex.tangent.xw = half2(half(1f), half(-1f));

            vertex.position.x = xOffset;
            vertex.position.z = ((float)z / Resolution - 0.5f) * sqrt(3f) / 2f; ;
            vertex.texCoord0.x = half(uOffset);
            vertex.texCoord0.y = half(vertex.position.z / (1f + 0.5f / Resolution) + 0.5f);
            streams.SetVertex(vi, vertex);

            vi += 1;

            for (int x = 1; x <= Resolution; x++, vi++, ti+=2){
                vertex.position.x = (float)x / (Resolution - 0.5f) + xOffset;
                vertex.texCoord0.x = half(x / (Resolution + 0.5f) + uOffset);
                streams.SetVertex(vi, vertex);

                if (z > 0){
                    streams.SetTriangle(ti + 0, vi + tA);
                    streams.SetTriangle(ti + 1, vi + tB);
                }
            }
        }
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f + 0.5f / Resolution, 0f, sqrt(3f) / 2f));
        public int Resolution { get; set; }
    }
}
