using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : MonoBehaviour
{
    [SerializeField] private bool debug = false;        // Ray�Ȃǂ̕\��
    [SerializeField] private LayerMask layerMask;       // ���C���[�}�X�N
    [SerializeField] private Vector3 rayRelativePos;    // �ŏ��̃v���C���[����̑��΃|�W�V����
    private Vector3 rayOrigin;

    public bool isGround()
    {
        // �����X�^�[�g�ʒu
        rayOrigin = transform.position + rayRelativePos;


        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(rayOrigin, Vector3.down, 0.1f, layerMask);

            // �f�o�b�O
            if (debug) { Debug.DrawRay(rayOrigin, Vector3.down * 0.1f, Color.yellow); }

            // ���̌����ʒu��
            rayOrigin.x += -rayRelativePos.x;

            if (hit.collider != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ���o�鎞�̏���
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Vector3</returns>
    public Vector3 SlopeGoUp(RaycastHit2D hit)
    {


        return new Vector3(0, 0, 0);
    }

    /// <summary>
    /// ������鎞�̏���
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Vector3</returns>
    public Vector3 SlopeGoDown(RaycastHit2D hit)
    {


        return new Vector3(0, 0, 0);
    }

}
