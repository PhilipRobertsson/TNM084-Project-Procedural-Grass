using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators{

    public struct UVSphere : IMeshGenerator{
        int ResolutionV => 2 * Resolution;
        int ResolutionU => 4 * Resolution;
        public int VertexCount => (ResolutionU + 1) * (ResolutionV + 1) - 2;
        public int IndexCount => 6 * ResolutionU * (ResolutionV - 1);
        public int JobLength => ResolutionU + 1;

        public void Execute<S>(int u, S streams) where S : struct, IMeshStreams{
            if (u == 0){
                ExecuteSeam(streams);
            }
            else{
                ExecuteRegular(u, streams);
            }
        }

        public void ExecuteRegular<S>(int u, S streams) where S : struct, IMeshStreams {
            int vi = (ResolutionV + 1) * u - 2, ti = 2 * (ResolutionV - 1) * (u - 1);
            var vertex = new Vertex();
            vertex.position.y = vertex.normal.y = -1f;
            vertex.tangent.x = half(cos(2f * PI * (u - 0.5f) / ResolutionU));
            vertex.tangent.z = half(sin(2f * PI * (u - 0.5f) / ResolutionU));
            vertex.tangent.w = half(-1f);
            vertex.texCoord0.x = half((u - 0.5f) / ResolutionU);
            streams.SetVertex(vi, vertex);

            vertex.position.y = vertex.normal.y = 1f;
            vertex.texCoord0.y = half(1f);
            streams.SetVertex(vi + ResolutionV, vertex);

            vi += 1;

            float2 circle;
            sincos(2f * PI * u / ResolutionU, out circle.x, out circle.y);
            vertex.tangent.xz = half2(circle.yx);
            circle.y = -circle.y;

            vertex.texCoord0.x = half((float)u / ResolutionU);
            int shiftLeft = (u == 1 ? 0 : -1) - ResolutionV;

            streams.SetTriangle(ti, vi + int3(-1, shiftLeft, 0));
            ti += 1;

            for (int v = 1; v < ResolutionV; v++, vi++){
                if (v > 1){
                    streams.SetTriangle(ti + 0, vi + int3(shiftLeft - 1, shiftLeft, -1));
                    streams.SetTriangle(ti + 1, vi + int3(-1, shiftLeft, 0));
                    ti += 2;
                }

                sincos(
                    PI + PI * v / ResolutionV,
                    out float circleRadius, out vertex.position.y
                );

                vertex.position.xz = circle * -circleRadius;
                vertex.normal = vertex.position;
                vertex.texCoord0.y = half((float)v / ResolutionV);
                streams.SetVertex(vi, vertex);
            }
            streams.SetTriangle(ti, vi + int3(shiftLeft - 1, 0, -1));
        }

        public void ExecuteSeam<S>(S streams) where S : struct, IMeshStreams{
            var vertex = new Vertex();
            vertex.tangent.x = half(1f);
            vertex.tangent.w = half(-1f);
            vertex.texCoord0.x = half(0.5f / ResolutionU);

            vertex.position.y = vertex.normal.y = 1f;
            vertex.texCoord0.y = half(1f);

            vertex.tangent.xz = half2(half(1f), half(0f));

            vertex.texCoord0.x = half(0f);

            for (int v = 1; v < ResolutionV; v++){
                sincos(
                    PI + PI * v / ResolutionV,
                    out vertex.position.z, out vertex.position.y
                );
                vertex.normal = vertex.position;
                vertex.texCoord0.y = half((float)v / ResolutionV);
                streams.SetVertex(v -1, vertex);
            }
        }
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));
        public int Resolution { get; set; }
    }
}
