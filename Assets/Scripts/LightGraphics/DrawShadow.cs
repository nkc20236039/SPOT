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
        // ���b�V���̍쐬
        var mesh = new Mesh();

        // ���_���W�z������b�V���ɃZ�b�g
        mesh.SetVertices(new Vector3[] {
            new Vector3 (-8.889f, -5),
            new Vector3 (-8.889f, 5f),
            new Vector3 (8.889f, 5f),
            new Vector3 (8.889f, -5f),
        });

        // �C���f�b�N�X�z������b�V���ɃZ�b�g
        mesh.SetTriangles(new int[] {
            0, 1, 2, 0, 2, 3
        }, 0);

        // MeshFilter��ʂ��ă��b�V����MeshRenderer�ɃZ�b�g
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
    }
}
