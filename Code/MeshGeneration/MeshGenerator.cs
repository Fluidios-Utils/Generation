using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

namespace FluidiousUtils.Generation
{
    public static class MeshGenerator
    {
        public static Mesh NgonShape(int N, float radius, float height, float bevel, Color vertexColor, string customMeshName = null, BevelType bevelType = BevelType.AllEdges, ShapeType shapeType = ShapeType.Normal)
        {
            Mesh mesh = new Mesh();
            mesh.name = customMeshName != null ? customMeshName : string.Format("Runtime{0}Gon", N);
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();

            Vector3[] top = null, topWall, bottomWall, bottom = null;

            switch (bevelType)
            {
                case BevelType.AllEdges:
                    topWall = GetNGon(Vector3.up * (height / 2), N, radius, bevel);
                    bottomWall = GetNGon(Vector3.down * (height / 2), N, radius, bevel);
                    top = GetNGon(Vector3.up * (height / 2 + bevel), N, radius - bevel, 0);
                    bottom = GetNGon(Vector3.down * (height / 2 + bevel), N, radius - bevel, 0);

                    FillTopNGon(top, vertexColor, vertices, triangles, colors);
                    MergeToRing(topWall, top.Length, N, vertexColor, vertices, triangles, colors);
                    MergeToRing(bottomWall, topWall.Length, N, vertexColor, vertices, triangles, colors);
                    if (shapeType != ShapeType.WithoutBottomSide)
                    {
                        MergeToRing(bottom, bottomWall.Length, N, vertexColor, vertices, triangles, colors);
                        FillBottomNGon(bottom, vertices.Count, triangles);
                    }
                    break;
                case BevelType.OnlySideEdges:
                    topWall = GetNGon(Vector3.up * (height / 2), N, radius, bevel);
                    bottomWall = GetNGon(Vector3.down * (height / 2), N, radius, bevel);

                    FillTopNGon(topWall, vertexColor, vertices, triangles, colors);
                    MergeToRing(bottomWall, topWall.Length, N, vertexColor, vertices, triangles, colors);
                    if(shapeType != ShapeType.WithoutBottomSide) FillBottomNGon(bottomWall, vertices.Count, triangles);
                    break;
                case BevelType.OnlyTopEdges:
                    topWall = GetNGon(Vector3.up * (height / 2), N, radius, 0);
                    bottomWall = GetNGon(Vector3.down * (height / 2), N, radius, 0);
                    top = GetNGon(Vector3.up * (height / 2 + bevel), N, radius - bevel, 0);
                    bottom = GetNGon(Vector3.down * (height / 2 + bevel), N, radius - bevel, 0);

                    FillTopNGon(top, vertexColor, vertices, triangles, colors);
                    MergeToRing(topWall, top.Length, N, vertexColor, vertices, triangles, colors);
                    MergeToRing(bottomWall, topWall.Length, N, vertexColor, vertices, triangles, colors);
                    if (shapeType != ShapeType.WithoutBottomSide)
                    {
                        MergeToRing(bottom, bottomWall.Length, N, vertexColor, vertices, triangles, colors);
                        FillBottomNGon(bottom, vertices.Count, triangles);
                    }
                    break;
                default:
                    break;
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color, List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            int vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        public static Vector3[] GetNGon(Vector3 center, int N, float radius, float bevel)
        {
            Vector3[] points = new Vector3[bevel > 0 ? N * 2 : N];
            float bevelOffset = Mathf.Asin(bevel);
            float fi;

            for (int i = 0; i < N; i++)
            {
                if (bevel > 0)
                {
                    fi = 360f / N * i * Mathf.Deg2Rad - bevelOffset;
                    points[i * 2].x = center.x + radius * Mathf.Cos(fi);
                    points[i * 2].y = center.y;
                    points[i * 2].z = center.z + radius * Mathf.Sin(fi);

                    fi = 360f / N * i * Mathf.Deg2Rad + bevelOffset;
                    points[i * 2 + 1].x = center.x + radius * Mathf.Cos(fi);
                    points[i * 2 + 1].y = center.y;
                    points[i * 2 + 1].z = center.z + radius * Mathf.Sin(fi);
                }
                else
                {
                    fi = 360f / N * i * Mathf.Deg2Rad;
                    points[i].x = center.x + radius * Mathf.Cos(fi);
                    points[i].y = center.y;
                    points[i].z = center.z + radius * Mathf.Sin(fi);
                }
            }

            return points;
        }

        private static void MergeToRing(Vector3[] bottom, int topNgonSize, int N, Color color, List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            int t1,t2, t3, t4, t5;
            int bottomNgonSize = bottom.Length;
            if(bottomNgonSize > topNgonSize)
            {
                for (int i = 0; i < topNgonSize; i++)
                {
                    t1 = topNgonSize - N + i;
                    t2 = bottomNgonSize - N + i*2;
                    t3 = t2 + 1;
                    t4 = t1 + 1; if (t4 == bottomNgonSize - N) t4 = topNgonSize - N;
                    t5 = t2 + 2; if(t5 == (bottomNgonSize + topNgonSize + (topNgonSize/N-1)*N)) t5 = bottomNgonSize - N;
                    //Debug.Log(string.Format("t1:{0}; t2:{1}; t3:{2}; t4:{3}; t5:{4};", t1, t2, t3, t4, t5));
                    vertices.Add(bottom[i*2]);
                    vertices.Add(bottom[i*2+1]);
                    triangles.Add(t1);
                    triangles.Add(t3);
                    triangles.Add(t2);

                    triangles.Add(t1);
                    triangles.Add(t5);
                    triangles.Add(t3);

                    triangles.Add(t1);
                    triangles.Add(t4);
                    triangles.Add(t5);

                    colors.Add(color);
                    colors.Add(color);
                }
            }
            else if(bottomNgonSize == topNgonSize)
            {
                int vertexCount = vertices.Count;
                for (int i = 0; i < topNgonSize; i++)
                {
                    t1 = vertexCount + i;
                    t4 = vertexCount - topNgonSize + i;
                    t3 = t1 + 1; if (t3 == vertexCount + bottomNgonSize) t3 = vertexCount;
                    t2 = t4 + 1; if (t2 == vertexCount) t2 = vertexCount - topNgonSize;
                    //Debug.Log(string.Format("t1:{0}; t2:{1}; t3:{2}; t4:{3};", t1, t2, t3, t4));
                    vertices.Add(bottom[i]);

                    triangles.Add(t1);
                    triangles.Add(t2);
                    triangles.Add(t3);

                    triangles.Add(t1);
                    triangles.Add(t4);
                    triangles.Add(t2);

                    colors.Add(color);
                }
            }
            else
            {
                int vertexCount = vertices.Count;
                for (int i = 0; i < bottomNgonSize; i++)
                {
                    t1 = vertexCount - topNgonSize + i*2;
                    t2 = vertexCount + i;
                    t3 = t1 + 1;
                    t4 = t1 + 2; if (t4 == vertexCount) t4 = vertexCount - topNgonSize;
                    t5 = t2 + 1; if(t5 == vertexCount + bottomNgonSize) t5 = vertexCount;
                    //Debug.Log(string.Format("t1:{0}; t2:{1}; t3:{2}; t4:{3}; t5:{4};", t1, t2, t3, t4, t5));
                    vertices.Add(bottom[i]);

                    triangles.Add(t3);
                    triangles.Add(t2);
                    triangles.Add(t1);

                    triangles.Add(t4);
                    triangles.Add(t2);
                    triangles.Add(t3);

                    triangles.Add(t4);
                    triangles.Add(t5);
                    triangles.Add(t2);

                    colors.Add(color);
                }
            }
        }

        private static void FillTopNGon(Vector3[] ngon, Color color, List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            for (int i = 0; i < ngon.Length; i++)
            {
                vertices.Add(ngon[i]);
                colors.Add(color);
            }

            for (int i = 2; i < ngon.Length; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i-1);
            }
        }

        private static void FillBottomNGon(Vector3[] ngon, int vertexCount, List<int> triangles)
        {
            int t1 = vertexCount -1;
            int t2, t3;
            for (int i = 2; i < ngon.Length; i++)
            {
                t2 = t1 - (i - 1);
                t3 = t1 - i;

                //triangles.Add(t1);
                //triangles.Add(t2);
                //triangles.Add(t3);

                //triangles.Add(t2);
                //triangles.Add(t3);
                //triangles.Add(t1);

                //triangles.Add(t3);
                //triangles.Add(t1);
                //triangles.Add(t2);

                //triangles.Add(t1);
                //triangles.Add(t3);
                //triangles.Add(t2);

                //triangles.Add(t3);
                //triangles.Add(t2);
                //triangles.Add(t1);

                triangles.Add(t2);
                triangles.Add(t1);
                triangles.Add(t3);
            }
        }

        public enum BevelType
        {
            AllEdges, OnlySideEdges, OnlyTopEdges
        }
        public enum ShapeType
        {
            Normal, WithoutBottomSide
        }
    }
}