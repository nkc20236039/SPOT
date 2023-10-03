using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField] private ShadowEdgeAsset shadowEdgeDate;
    [SerializeField] private float test;
    Player playerScript;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }


    // �e�̏��
    // 0: �e���ł���p�̈ʒu
    // 1: ���o�p�R���C�_�[���B�_
    // 2: ���ۂɉe�������ʒu
    public (Vector2[] shadowVector, GameObject hitObject) GetEdgeInformation(Transform light)
    {
        Vector2[] shadowSideInfo = new Vector2[3];

        // �e���ł���p�̈ʒu��ݒ�
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            light.position
            - this.transform.position
            ).normalized * 0.1f;

        //�J�����t���[�����B�_�����߂�
        RaycastHit2D displayFreamHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                shadowEdgeDate.indexLayerMask
                );

        shadowSideInfo[1] = displayFreamHit.point;
        shadowSideInfo[1].x += displayFreamHit.distance;

        // ���ۂ̉e�������ʒu�����߂�
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                shadowEdgeDate.objectLayerMask
                );
        shadowSideInfo[2] = objectHit.point;
        GameObject shadowHitObject;
        if (objectHit.transform != null)
        {
            // �I�u�W�F�N�g�ɓ��������瓖�������I�u�W�F�N�g������
            shadowHitObject = objectHit.transform.gameObject;
        }
        else
        {
            // �I�u�W�F�N�g�ɓ�����Ȃ�������e�I�u�W�F�N�g�𓖂������I�u�W�F�ɂ���
            shadowHitObject = transform.root.gameObject;
        }

        // �ǂ̃I�u�W�F�N�g�ɂ����Ȃ���O����������
        // �p�̍��W�ɑ�����
        if (Mathf.Approximately(objectHit.distance, 0))
        {
            shadowSideInfo[2] = shadowSideInfo[0];
        }

        if (shadowEdgeDate.debug)
        {
            Vector2 debugFreamHit = new Vector2(shadowSideInfo[1].x - displayFreamHit.distance, shadowSideInfo[1].y);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, debugFreamHit, Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[2], Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ���C�g�̌����������Ă��邩�𒲂ׂ�
    /// </summary>
    /// <returns>�������Ă�����true</returns>
    public bool IsExposedToLight(Vector2 lightPosition)
    {
        Vector2 edgePosition = transform.position;
        Vector2 edgeDirection = edgePosition - lightPosition;
        float edgeDistance = edgeDirection.magnitude;
        // ���ݒn��lightArea���C���[�̒��ɑ��݂��Ȃ����
        // false��Ԃ�
        if (!Physics2D.OverlapPoint(transform.position, shadowEdgeDate.lightAreaLayerMask)) { return false; }
        Debug.DrawLine(lightPosition + edgeDirection.normalized * 0.001f,
            edgePosition - edgeDirection.normalized * 0.001f, Color.yellow);

        RaycastHit2D lineHit = Physics2D.Linecast(
            lightPosition + edgeDirection.normalized * 0.001f,
            edgePosition - edgeDirection.normalized * 0.001f,
            shadowEdgeDate.groundLayerMask
            );

        if (lineHit)
        {
            Debug.DrawLine((lightPosition + edgeDirection.normalized) * 0.001f,
            (edgePosition - edgeDirection.normalized) * 0.001f, Color.yellow);

            Vector2 lightNormal = edgeDirection.normalized;
            if (lineHit.transform.gameObject != gameObject && lightNormal.y > -0.002f)
            {
                // �����ȊO�ɓ����������̏���
                // ������Ƃ��ꂽ�ʒu����
                // ���C�g�Ɍq����܂łɃI�u�W�F�N�g�����݂��邩
                float gap = 0.01f;
                int direction = -1;
                for (int i = 0; i < 2; i++)
                {
                    direction *= -1;
                    Vector2 startPosition = new Vector2(lightPosition.x, lightPosition.y + (gap * direction));
                    Vector2 endPosition = new Vector2(edgePosition.x, lightPosition.y + (gap * direction));
                    if (!Physics2D.Linecast(startPosition, endPosition, shadowEdgeDate.groundLayerMask))
                    {
                        Debug.DrawLine(startPosition, endPosition, Color.red);
                        return true;
                    }
                }

            }
        }
        else
        {
            Debug.DrawLine((lightPosition + edgeDirection.normalized) * 0.001f,
            (edgePosition - edgeDirection.normalized) * 0.001f, Color.red);
            return true;
        }



        return false;
    }

}
