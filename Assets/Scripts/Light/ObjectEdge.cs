using UnityEngine;
using UnityEngine.UIElements;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField] ShadowEdgeAsset shadowEdgeDate;

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
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[1], Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[2], Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ���C�g�̌����������Ă��邩�𒲂ׂ�
    /// </summary>
    /// <returns>�������Ă�����true</returns>
    public bool IsExposedToLight(Vector2 lightPoint, Vector2 pointA, Vector2 pointB)
    {
        // Debug.Log($"{lightPoint}\n{pointA}\n{pointB}");

        Vector2 edgePoint = transform.position;
        Vector2 direction = (edgePoint - lightPoint).normalized * 0.0001f;
        Vector2 distance = edgePoint - lightPoint;

        // �e�ӂ̃x�N�g�����v�Z
        Vector2 pointAToLight = pointA - lightPoint;
        Vector2 pointBToPointA = pointB - pointA;
        Vector2 lightToPointB = lightPoint - pointB;

        // �e���_����w�肵�����W�ւ̃x�N�g�����v�Z
        Vector2 edgeToLight = edgePoint - lightPoint;
        Vector2 edgeToPointA = edgePoint - pointA;
        Vector2 edgeToPointB = edgePoint - pointB;

        // �e�ӂ̊O�ς��v�Z
        float crossPointAToLight = Vector3.Cross(pointAToLight, edgeToLight).z;
        float crossPointBToPointA = Vector3.Cross(pointBToPointA, edgeToPointA).z;
        float crossLightToPointB = Vector3.Cross(lightToPointB, edgeToPointB).z;

        bool plusCross = crossPointAToLight >= 0 && crossPointBToPointA >= 0 && crossLightToPointB >= 0;
        bool minusCross = crossPointAToLight <= 0 && crossPointBToPointA <= 0 && crossLightToPointB <= 0;
        if (!(plusCross || minusCross))
        {
            // ���ׂĂ̊O�ς����������łȂ���΋����œ������Ă��Ȃ����Ƃɂ���
            return false;
        }

        // ���C�g����Ray���o��
        RaycastHit2D[] objectHitAll =
            Physics2D.RaycastAll
            (
                lightPoint + direction,
                direction,
                distance.magnitude * 2,
                shadowEdgeDate.objectLayerMask
            );



        foreach (RaycastHit2D objectHit in objectHitAll)
        {
            if (shadowEdgeDate.debug)
            {
                Debug.DrawLine(lightPoint, objectHit.point, Color.red);
            }

            // ���g�ɓ����������_�ŏI������
            if (objectHit.transform.gameObject == this.gameObject)
            {
                return true;
            }


            // ���C�g�ƕ��s�ȏꍇ�̓��ʏ���
            Vector2 pointNormal = distance.normalized;
            if (Mathf.Approximately(0, pointNormal.x))
            {
                float gaps = 0.1f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 start = objectHit.point;
                    Vector2 end = edgePoint;
                    Vector2 hitNormal = (start - end).normalized;
                    start.x += gaps;
                    start.y -= hitNormal.y * 0.01f;
                    end.x += gaps;
                    end.y += hitNormal.y * 0.01f;

                    RaycastHit2D lineHit = Physics2D.Linecast(start, end, shadowEdgeDate.objectLayerMask);

                    if (!lineHit)
                    {
                        return true;
                    }

                    gaps *= -1;
                }
            }
            else if (Mathf.Approximately(0, pointNormal.y))
            {
                float gaps = 0.1f;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 start = objectHit.point;
                    Vector2 end = edgePoint;
                    Vector2 hitNormal = (start - end).normalized;
                    start.y += gaps;
                    start.x -= hitNormal.x * 0.01f;
                    end.y += gaps;
                    end.x += hitNormal.x * 0.01f;

                    RaycastHit2D lineHit = Physics2D.Linecast(start, end, shadowEdgeDate.objectLayerMask);

                    if (!lineHit)
                    {
                        return true;
                    }

                    gaps *= -1;
                }
            }


            // 

            // �q�b�g�����I�u�W�F�N�g�̒���
            // ObjectEdge�ȊO�����݂���
            // false��Ԃ�
            if (objectHit.transform.gameObject.tag != gameObject.tag)
            {
                return false;
            }

        }


        // ���������I�u�W�F�N�g�������I�u�W�F�N�g��������true
        return true;
    }

}
