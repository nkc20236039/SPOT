using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : MonoBehaviour
{
    [SerializeField] private bool debug = false;        // Ray�Ȃǂ̕\��
    [SerializeField] private LayerMask layerMask;       // ���C���[�}�X�N
    [SerializeField] private Vector3 rayRelativePos;    // �ŏ��̃v���C���[����̑��΃|�W�V����
    private Vector3 rayOrigin;
    private Vector3 groundNormal;

    /// <summary>
    /// �n�ʔ���
    /// </summary>
    /// <returns>bool</returns>
    public bool isGround()
    {
        // �����X�^�[�g�ʒu
        rayOrigin = transform.position + rayRelativePos;


        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(rayOrigin, Vector3.down, 0.01f, layerMask);

            // �f�o�b�O
            if (debug) { Debug.DrawRay(rayOrigin, Vector3.down * 0.01f, Color.yellow); }

            // ���̌����ʒu��
            rayOrigin.x += -rayRelativePos.x;

            // �n�ʂɂ��邩
            if (hit.collider != null)
            {
                // �@���x�N�g�����
                groundNormal = hit.normal;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ��̈ړ�����
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector3 Slope(Vector3 vector)
    {
        if(isGround())
        {
            // �Ζʂ̃x�N�g�������߂�
            return Vector3.ProjectOnPlane(vector, groundNormal);
        }

        return vector;
    }

}
