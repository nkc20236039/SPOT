// Copyright 2021 Alejandro Villalba Avila
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

using Game.Utils.Math;
using Game.Utils.Triangulation;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A test client for the triangulation algorithm. Intended to be used via Inspector.
/// </summary>
public class DelaunayTriangulationTester : MonoBehaviour
{
    [Header("Polygons")]
    [Tooltip("Put here the collider that contains the main point cloud.")]
    public PolygonCollider2D MainPointCloud;

    [Tooltip("Put here the collider that contains the polygons formed by constrained edges.")]
    public PolygonCollider2D ConstrainedEdges;

    [Header("Tilemaps")]
    [Tooltip("Put here the collider of the tilemap that is used as background.")]
    public CompositeCollider2D TilemapBackground;

    [Tooltip("Put here the collider of the tilemap that is used as walls, ground, platforms...")]
    public CompositeCollider2D TilemapBlockers;

    [Header("Settings")]
    [Tooltip("When enabled, the output triangle edges are displayed.")]
    public bool DrawTriangles = true;

    [Tooltip("The mesh that displays the output triangles.")]
    public MeshFilter VisualRepresentation;

    [Tooltip("Enables tesselation (before calculating constrained edges) when greater than zero. It subdivides the triangles until each of them has an area smaller than this value.")]
    public float TesselationMaximumTriangleArea = 0.0f;

    protected List<Triangle2D> m_outputTriangles = new List<Triangle2D>();

    protected DelaunayTriangulation m_triangulation = new DelaunayTriangulation();

    [SerializeField] Player playerScript;

    public void RunTestPolygonColliders()
    {

        List<Vector2> pointsToTriangulate = new List<Vector2>();
        ExtractPointsFromCollider(MainPointCloud, pointsToTriangulate);

        List<List<Vector2>> constrainedEdgePoints = new List<List<Vector2>>();

        if (ConstrainedEdges != null)
        {
            ExtractPointsFromCollider(ConstrainedEdges, constrainedEdgePoints);
        }

        m_outputTriangles.Clear();
        m_triangulation.Triangulate(pointsToTriangulate, TesselationMaximumTriangleArea, constrainedEdgePoints);
        m_triangulation.GetTrianglesDiscardingHoles(m_outputTriangles);

        VisualRepresentation.mesh = CreateMeshFromTriangles(m_outputTriangles);

    }

    protected void RunTestTilemapColliders()
    {
        Debug.Log("Running Delaunay triangulation test...");

        List<Vector2> pointsToTriangulate = new List<Vector2>();
        ExtractPointsFromCollider(TilemapBackground, pointsToTriangulate);

        List<List<Vector2>> constrainedEdgePoints = new List<List<Vector2>>();

        if (TilemapBlockers != null)
        {
            ExtractPointsFromCollider(TilemapBlockers, constrainedEdgePoints);
        }

        m_outputTriangles.Clear();
        m_triangulation.Triangulate(pointsToTriangulate, TesselationMaximumTriangleArea, constrainedEdgePoints);
        m_triangulation.GetTrianglesDiscardingHoles(m_outputTriangles);

        VisualRepresentation.mesh = CreateMeshFromTriangles(m_outputTriangles);

        Debug.Log("Test finished.");
    }

    private void ExtractPointsFromCollider(CompositeCollider2D collider, List<Vector2> outputPoints)
    {
        int pathCount = collider.pathCount;

        for (int i = 0; i < pathCount; ++i)
        {
            List<Vector2> pathPoints = new List<Vector2>();
            collider.GetPath(i, pathPoints);
            outputPoints.AddRange(pathPoints);
        }
    }

    private void ExtractPointsFromCollider(CompositeCollider2D collider, List<List<Vector2>> outpuColliderPolygons)
    {
        int pathCount = collider.pathCount;

        for (int i = 0; i < pathCount; ++i)
        {
            List<Vector2> pathPoints = new List<Vector2>();
            collider.GetPath(i, pathPoints);
            outpuColliderPolygons.Add(pathPoints);
        }
    }

    private Mesh CreateMeshFromTriangles(List<Triangle2D> triangles)
    {
        List<Vector3> vertices = new List<Vector3>(triangles.Count * 3);
        List<int> indices = new List<int>(triangles.Count * 3);

        for (int i = 0; i < triangles.Count; ++i)
        {
            vertices.Add(triangles[i].p0);
            vertices.Add(triangles[i].p1);
            vertices.Add(triangles[i].p2);
            indices.Add(i * 3 + 2); // Changes order
            indices.Add(i * 3 + 1);
            indices.Add(i * 3);
        }

        Mesh mesh = new Mesh();
        mesh.subMeshCount = 1;
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, CreateMeshUV(vertices.ToArray()));
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        return mesh;
    }

    private void ExtractPointsFromCollider(PolygonCollider2D collider, List<Vector2> outputPoints)
    {
        int pathCount = collider.pathCount;

        for (int i = 0; i < pathCount; ++i)
        {
            List<Vector2> pathPoints = new List<Vector2>();
            collider.GetPath(i, pathPoints);
            outputPoints.AddRange(pathPoints);
        }
    }

    private void ExtractPointsFromCollider(PolygonCollider2D collider, List<List<Vector2>> outpuColliderPolygons)
    {
        int pathCount = collider.pathCount;

        for (int i = 0; i < pathCount; ++i)
        {
            List<Vector2> pathPoints = new List<Vector2>();
            collider.GetPath(i, pathPoints);
            outpuColliderPolygons.Add(pathPoints);
        }
    }

    private void OnDrawGizmos()
    {
        if (m_outputTriangles == null || !DrawTriangles)
            return;

        Color triangleColor = Color.black;

        for (int i = 0; i < m_outputTriangles.Count; ++i)
        {
            Debug.DrawLine(m_outputTriangles[i].p0, m_outputTriangles[i].p1, triangleColor);
            Debug.DrawLine(m_outputTriangles[i].p1, m_outputTriangles[i].p2, triangleColor);
            Debug.DrawLine(m_outputTriangles[i].p2, m_outputTriangles[i].p0, triangleColor);
        }
    }

    private Vector2[] CreateMeshUV(Vector3[] vertices)
    {
        Camera renderingCamera = Camera.main;
        Vector2[] UVs = new Vector2[vertices.Length];
        // カメラサイズを取得
        float cameraHeight = renderingCamera.orthographicSize;
        // カメラの横幅
        float cameraWidth = cameraHeight * renderingCamera.aspect;
        Vector2 cameraPosition = renderingCamera.transform.position;
        // カメラの左下取得
        Vector2 cameraOriginPosition = cameraPosition - new Vector2(cameraWidth, cameraHeight);
        // カメラの左下を原点としたライトの位置を求める
        Vector2 lightPosition = transform.position;
        Vector2 cameraToLightVector = lightPosition - cameraOriginPosition;

        // 頂点をUV用の座標に変換
        for (int i = 0; i < UVs.Length; i++)
        {
            UVs[i] = new Vector2
            (
                playerScript.lightDirection * (vertices[i].x + cameraToLightVector.x / 4) / cameraWidth,
                (vertices[i].y + cameraToLightVector.y / 4) / cameraHeight
            );
        }

        return UVs;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(DelaunayTriangulationTester))]
    public class DelaunayTriangulationTesterInspector : UnityEditor.Editor
    {
        private int m_triangleToDraw = 0;
        private int m_pointToDraw = 0;
        private string m_triangleInfo;

        public override void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("You can use different colliders of the scene as test cases, then press the Triangulate button. When the triangulation is generated, the mesh will be updated and the Draw Triangle and Draw point will be available for debugging.", UnityEditor.MessageType.Info);

            base.OnInspectorGUI();

            if (GUILayout.Button("Triangulate!"))
            {
                ((DelaunayTriangulationTester)target).RunTestPolygonColliders();
            }

            if (GUILayout.Button("Triangulate tilemap!"))
            {
                ((DelaunayTriangulationTester)target).RunTestTilemapColliders();
            }

            if (((DelaunayTriangulationTester)target).m_triangulation == null)
            {
                return;
            }
        }
    }

    private void DrawPoint(int pointIndex)
    {
        Vector2 point = m_triangulation.TriangleSet.GetPointByIndex(pointIndex);

        Debug.DrawRay(point + Vector2.down * 0.02f * 0.5f, Vector2.up * 0.02f, Color.red, 10.0f);
        Debug.DrawRay(point + Vector2.left * 0.02f * 0.5f, Vector2.right * 0.02f, Color.red, 10.0f);
    }

    private void DrawTriangle(int triangleIndex)
    {
        m_triangulation.TriangleSet.DrawTriangle(triangleIndex, Color.green);
    }

#endif

}
