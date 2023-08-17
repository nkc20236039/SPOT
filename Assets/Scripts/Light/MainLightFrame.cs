using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class MainLightFrame : MonoBehaviour
{
    private GameObject enableLight;                 // �v���C���[���������L���̃��C�g
    private GameObject[] spotLights;                // �t�B�[���h���ɂ���S�Ẵ��C�g
    private List<GameObject> hittingLight           // ���𓖂ĂĂ郉�C�g
        = new List<GameObject>();
    private SpotLightParameter enableLightScript;   // �p�����[�^�[�X�N���v�g
    private List<Vector2> points                    // 
        = new List<Vector2>();


    void Start()
    {
        // �S�Ẵ��C�g�擾
        spotLights = GameObject.FindGameObjectsWithTag("Light");

    }

    void Update()
    {
        // �L�����̃��C�g�擾
        enableLight = GameObject.FindWithTag("EnableLight");
        enableLightScript = enableLight.GetComponent<SpotLightParameter>();

        // ���̒��ɂ��邩�e�X�g����
        foreach (GameObject light in spotLights)
        {
            SpotLightParameter lightScript =
                light.GetComponent<SpotLightParameter>();

            if (IsLightHit(lightScript.forwardDirection, lightScript.lightPosition, lightScript.lightAngle, enableLight.transform.position))
            {
                hittingLight.Add(light);
            }
        }

        /* �O���̌�_���L�^���� */
        // �����l
        Vector2 lineStart = Vector2.positiveInfinity;                // ��_���W
        Vector2 lineEnd = enableLightScript.upHit.point;
        Vector2 startPosition = enableLight.transform.position;
        // �J�n�ʒu�ƌ�_���W�������ɂȂ�܂ŌJ��Ԃ�
        while (!Mathf.Approximately(lineStart.magnitude, startPosition.magnitude))
        {
            if (hittingLight.Count == 0)
            {
                // ���݂̈ʒu�����̃��C�g�͈̔͊O�̂Ƃ�
                // currentLocation�������l�̏ꍇ�L�����C�g�̍��W�ɐݒ�
                if (lineStart == Vector2.positiveInfinity)
                {
                    lineStart = enableLight.transform.position;
                }
                // ����̍��W���L�^
                points.Add(lineStart);

                // ���̌�_���W�����߂�
                // �߂��̍��W���擾
                var intersection = FindNearestIntersection(lineStart, lineEnd);
                
            }
            else
            {
                // ���C�g�̒��Ȃ�

            }
        }

    }

    /// <summary>
    /// �w��̍��W�����C�g�̌��ɓ������Ă��邩���ׂ�
    /// </summary>
    /// <param name="source">�p�x�̔�r��</param>
    /// <param name="lightPosition">���̃��C�g�̌����������Ă��邩���ׂ���</param>
    /// <param name="lightEndPosition">���̃��C�g�̌����ǂ��܂ŐL�тĂ��邩</param>
    /// <param name="targetPosition">���̈ʒu�����C�g�̌��ɓ������Ă��邩���ׂ���</param>
    /// <returns>���C�g�̌����󂯂Ă�����TRUE�A�󂯂Ă��Ȃ����FALSE</returns>
    private bool IsLightHit(Vector2 source, Vector2 lightPosition, Vector2 lightEndPosition, Vector2 targetPosition)
    {
        Vector2 lightVector = lightEndPosition - lightPosition;
        Vector2 targetVector = targetPosition - lightPosition;

        float lightAngle = Vector2.Angle(source, lightVector);
        float targetAngle = Vector2.Angle(source, targetVector);

        return targetAngle < lightAngle;
    }

    /// <summary>
    /// ��_���W�̌v�Z������
    /// </summary>
    /// <param name="pointA1">1�ڂ̕ӂ̍ŏ�</param>
    /// <param name="pointA2">1�ڂ̕ӂ̍Ō�</param>
    /// <param name="pointB1">2�ڂ̕ӂ̍ŏ�</param>
    /// <param name="pointB2">2�ڂ̕ӂ̍Ō�</param>
    private (bool isIntersection, Vector2 intersection, Vector2 normal) Intersection(Vector2 pointA1, Vector2 pointA2, Vector2 pointB1, Vector2 pointB2)
    {
        // ��_���W�����l
        Vector2 intersection = Vector2.zero;

        // ���̐i�s�����̃x�N�g��
        Vector2 pointAVector = pointA2 - pointA1;
        // ��r���������̃x�N�g��
        Vector2 pointBVector = pointB2 - pointB1;

        // ��_���W�����߂�
        float t, s;
        bool isIntersection =
            IsIntersect(pointA1, pointAVector, pointB1, pointBVector, out t, out s);
        if (isIntersection)
        {
            intersection = pointA1 + t * pointAVector;
        }

        Vector2 normal = new Vector2(-pointAVector.y, pointAVector.x).normalized + new Vector2(-pointBVector.y, pointBVector.x).normalized;
        return (isIntersection, intersection, normal);
    }

    /// <summary>
    /// 2�ӂ��������Ă��邩
    /// </summary>
    /// <param name="pointA">1�ڂ̕ӂ̎n�_</param>
    /// <param name="pointAVector">1�ڂ̕ӂ̃x�N�g��</param>
    /// <param name="pointB">2�ڂ̕ӂ̎n�_</param>
    /// <param name="pointBVector">2�ڂ̕ӂ̃x�N�g��</param>
    /// <returns>�������Ă����TRUE�A���Ă��Ȃ����FALSE</returns>
    private bool IsIntersect(Vector2 pointA, Vector2 pointAVector, Vector2 pointB, Vector2 pointBVector, out float t, out float s)
    {
        float cross = pointAVector.x * pointBVector.y - pointAVector.y * pointBVector.x;
        // �������Ă��Ȃ���
        if (cross == 0)
        {
            t = 0;
            s = 0;
            return false;
        }

        Vector2 diff = pointB - pointA;
        t = (diff.x * pointBVector.y - diff.y * pointBVector.x) / cross;
        s = (diff.x * pointAVector.y - diff.y * pointAVector.x) / cross;

        return (t >= 0 && t <= 1 && s >= 0 && s <= 1);
    }

    private (Vector2 nextIntersection, Vector2 normal) FindNearestIntersection(Vector2 lineStart, Vector2 lineEnd)
    {
        List<Vector2> intersections = new List<Vector2>();    // ���ׂĂ̌�_���W
        List<Vector2> normals = new List<Vector2>();          // ���ׂĂ̖@���x�N�g��
        Vector2 nearestIntersection;
        Vector2 nearestNormal;

        // �󂯎�������Ɍ������W�����ׂċ��߂�
        foreach (GameObject light in spotLights)
        {
            // ����̃X�N���v�g�擾
            SpotLightParameter lightScript =
                light.GetComponent<SpotLightParameter>();
            // �����_�`�F�b�N
            for (int i = 0; i < lightScript.hitPoint.Length; i++)
            {
                var intersectionInfo = Intersection(
                    lineStart,
                    lineEnd,
                    lightScript.lightPosition,
                    lightScript.hitPoint[i]
                    );

                if (intersectionInfo.isIntersection)    // ��_�������������ɍ��W��ۑ�����
                {
                    intersections.Add(intersectionInfo.intersection);
                    normals.Add(intersectionInfo.normal);
                }
            }
        }

        // ��ԋ߂����W�����߂�
        nearestIntersection = intersections[0];
        nearestNormal = normals[0];
        for (int i = 0; i < intersections.Count; i++)
        {
            Vector2 closerIntersection = nearestIntersection - lineStart;
            Vector2 comparisonIntersection = intersections[i] - lineStart;

            if (closerIntersection.magnitude < comparisonIntersection.magnitude)
            {
                nearestIntersection = intersections[i];
            }
        }

        return (nearestIntersection, nearestNormal);
    }
}
