using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawShadow : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        // メッシュの作成
        var mesh = new Mesh();

        // 頂点座標配列をメッシュにセット
        mesh.SetVertices(new Vector3[] {
            new Vector3 (-8.889f, -5),
            new Vector3 (-8.889f, 5f),
            new Vector3 (8.889f, 5f),
            new Vector3 (8.889f, -5f),
        });

        // インデックス配列をメッシュにセット
        mesh.SetTriangles(new int[] {
            0, 1, 2, 0, 2, 3
        }, 0);

        // MeshFilterを通してメッシュをMeshRendererにセット
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
    }
}
