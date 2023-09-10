using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

// ���C�g����x���߂����Ay���������ɕ��ѕς���
// �V���h�[�|�C���g����L�т�e�̏I�_�����߂�
// �J�����t���[���ɂ����炻�����m���Ȃ���

public class LightCollider : MonoBehaviour
{
    [SerializeField] LayerMask m_LayerMask;     // ���C���[�}�X�N

    void Start()
    {

    }

    /// <summary>
    /// ���ɓ������Ă���p�̍��W���擾����
    /// </summary>
    /// <param name="lightPosition"></param>
    /// <param name="forwardDirection"></param>
    /// <param name="spotAngle"></param>
    /// <returns></returns>
    public List<GameObject> GetLightHitPoint(Vector2 lightPosition, Vector2 forwardDirection, float spotAngle)
    {
        List<GameObject> brightPoint = new List<GameObject>();
        // shadowPoint�����ɓ������Ă��邩�`�F�b�N����
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject shadowPoint = transform.GetChild(i).gameObject;

            Vector2 shadowCornerPosition = shadowPoint.transform.position;
            // ��r����x�N�g���v�Z
            Vector2 standardVector =
                (lightPosition -
                    (forwardDirection +
                    lightPosition))
                    .normalized;
            Vector2 targetVector =
                (lightPosition - shadowCornerPosition)
                .normalized;

            // ���C�g�܂ł̊p�x�����߂�
            float angle =
                Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                Mathf.Rad2Deg;

            if (angle < spotAngle / 2)
            {
                // �W���ɂȂ�I�u�W�F�N�g���Ȃ������ׂ�
                RaycastHit2D hit = Physics2D.Linecast(lightPosition, shadowCornerPosition, m_LayerMask);

                if (!hit)
                {
                    brightPoint.Add(shadowPoint);
                }
            }
        }

        return brightPoint;
    }

    private void SortShadowPoint(GameObject[] shadowPoint)
    {
        List<Vector2> cornerPosition = new List<Vector2>();

        for (int i = 0; i < shadowPoint.Length; i++)
        {
            // �e�I�u�W�F�N�g�����W�ɕϊ�
            cornerPosition.Add(shadowPoint[i].transform.position);
        }

        // x���W��
        cornerPosition.Sort((a, b) =>
        {
            if (a.x != b.x)
            {
                return a.x.CompareTo(b.x);
            }
            else
            {
                return -a.y.CompareTo(b.y);
            }
        });
    }
}
