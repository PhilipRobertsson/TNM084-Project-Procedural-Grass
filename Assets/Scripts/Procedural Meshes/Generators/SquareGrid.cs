using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators{

    public struct SquareGrid : IMeshGenerator{
        public int VertexCount => 4 * Resolution * Resolution;
        public int IndexCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution;
        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams {
            int vi = 4 * Resolution * z, ti = 2 * Resolution * z;

            for(int x = 0; x < Resolution; x++, vi += 4, ti += 2){
                var vertex = new Vertex();
                var xCoordinates = float2(x, x + 1f) / Resolution - 0.5f;
                var zCoordinates = float2(z, z + 1f) / Resolution - 0.5f;

                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.x;
                vertex.normal.y = 1f;
                vertex.tangent.xw = half2(half(1f), half(-1f));
                streams.SetVertex(vi + 0, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texCoord0 = half2(half(1f), half(0f));
                streams.SetVertex(vi + 1, vertex);

                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.y;
                vertex.texCoord0 = half2(half(0f), half(1f));
                streams.SetVertex(vi + 2, vertex);

                vertex.position.x = xCoordinates.y;
                vertex.texCoord0 = half(1f);
                streams.SetVertex(vi + 3, vertex);

                streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
                streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
            }
        }
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1.0f, 0.0f,1.0f));
        public int Resolution { get; set; }
    }
}
