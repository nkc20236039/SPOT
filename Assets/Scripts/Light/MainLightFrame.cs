/*using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class MainLightFrame : MonoBehaviour
{
    [SerializeField] private bool debug;

    private SpotLightArea enableLightScript;   // �p�����[�^�[�X�N���v�g
    public ReactiveProperty<GameObject> enableLight =// �v���C���[���������L���̃��C�g
        new ReactiveProperty<GameObject>();
    private List<GameObject> exclusionLight =       // ��_�����߂鎞���O����I�u�W�F�N�g
        new List<GameObject>();

    private GameObject[] spotLights;                // �t�B�[���h���ɂ���S�Ẵ��C�g
    private List<GameObject> hittingLight           // ���𓖂ĂĂ郉�C�g
        = new List<GameObject>();
    private List<Vector2> points                    // ���Ԃɉe�̊p�ɂȂ�ӏ��̍��W���L�^����
        = new List<Vector2>();
    private Vector2 scheduleLineEnd;               // ���̏I���n�_�ɂȂ邩������Ȃ��l

    void Start()
    {
        // �S�Ẵ��C�g�擾
        spotLights = GameObject.FindGameObjectsWithTag("Light");
        enableLight.Subscribe(value =>
            enableLightScript =
                enableLight.Value.GetComponent<SpotLightArea>());
    }

    void Update()
    {
        enableLightScript = enableLight.Value.GetComponent<SpotLightArea>();

        // ���C�g�ɓ������Ă���ꍇ���̃��C�g���擾����
        IsLightHit(enableLight.Value.transform.position, true, true);

        �O���̌�_���L�^����
       // �����l
       Vector2 lineStart = Vector2.positiveInfinity;                // ��_���W
        Vector2 lineEnd = enableLightScript.hitASide.point;
        Vector2 startPosition = enableLight.Value.transform.position;
        exclusionLight.Clear();
        points.Clear();
        // �J�n�ʒu�ƌ�_���W�������ɂȂ�܂ŌJ��Ԃ�
        int safety = 0;
        //while (!Mathf.Approximately(lineStart.magnitude, startPosition.magnitude))
        for (int i = 0; i < 4; i++)
        {
            if (hittingLight.Count == 0)
            {
                // ���݂̈ʒu�����̃��C�g�͈̔͊O�̂Ƃ�
                ---1�t���[��������̎��s-- -
                if (lineStart.magnitude >= Vector2.positiveInfinity.magnitude - 1)
                {
                    // currentLocation�������l�̏ꍇ�L�����C�g�̍��W�ɐݒ�
                    lineStart = enableLight.Value.transform.position;
                    exclusionLight.Add(enableLight.Value);
                    points.Add(lineStart);
                }
                -----------------------
                // �߂��̍��W���擾
                var latestIntersect =
                    FindNearestIntersection(lineStart, lineEnd, exclusionLight.ToArray());


                SpotLightArea lightScript = latestIntersect.intersectObject.GetComponent<SpotLightArea>();
                if (!lightScript.defaultLight)          // ��ʂ̒[�ɂ����Ƃ�
                {

                }
                else if (Mathf.Approximately(latestIntersect.nextIntersection.magnitude, Vector2.positiveInfinity.magnitude))               // ���C�g�̍��W�Ƃقړ�����������
                {
                    // �X�^�[�g�ƏI�������ւ�
                    lineStart = lineEnd;
                    lineEnd = scheduleLineEnd;
                    // ���X�g��2�ȏ�����Ă�����ŏ����폜
                    if (exclusionLight.Count > 1)
                    {
                        exclusionLight.RemoveAt(0);
                    }
                }
                else                                    // ���C�g�̌��̓r���̂Ƃ�
                {
                    // �X�^�[�g�n�_���X�V
                    lineStart = latestIntersect.nextIntersection;
                    // �I���n�_�����߂�
                    if (IsLightHit(lineStart + latestIntersect.normal * 0.1f))
                    {
                        lineEnd = GetLineEndPoint(
                            lineStart,
                            -latestIntersect.normal,
                            latestIntersect.intersectObject
                            );
                    }
                    else if (IsLightHit(lineStart - latestIntersect.normal * 0.1f))
                    {
                        lineEnd = GetLineEndPoint(
                            lineStart,
                            latestIntersect.normal,
                            latestIntersect.intersectObject
                            );
                    }
                    // �����_�ŏ��O�������I�u�W�F�N�g���w��
                    exclusionLight.Add(latestIntersect.intersectObject);
                    exclusionLight.RemoveAt(0);
                }

                // ����̍��W���L�^
                points.Add(lineStart);
                Debug.Log($"{i} : {lineStart}");
                if (debug) { Debug.DrawLine(lineStart, lineEnd, color: Color.green); }


            }
            else
            {
                // ���C�g�̒��Ȃ�


                // ����̈ʒu���Ƃ肠�����L�^���Ă���
                //points.Insert(0, lineStart);
            }



            // ���p�[�u
            safety++;
            if (safety >= 100) break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(points.Count);
            foreach (var item in points)
            {
                Debug.Log(item);
            }
        }

        // �������I��������I�u�W�F�N�g�𖳌�������
        // gameObject.SetActive(false);
    }

    private Vector2 GetLineEndPoint(Vector2 startPoint, Vector2 angle, GameObject targetObject)
    {
        SpotLightArea lightScript = targetObject.GetComponent<SpotLightArea>();
        // �I���n�_(�߂����ቓ���ɐݒ肵�Ă���)
        Vector2 endPoint = startPoint + angle * 10000;
        // ������
        Vector2 pointToStart;
        Vector2 pointToEnd;
        float crossProduct;
        Vector2 lineEnd = endPoint;

        // ���C�g�̈ʒu��������ɂ��邩
        if (lightScript.defaultLight)
        {
            // �X�^�[�g�ƃG���h�܂ł̃x�N�g�������߂�
            pointToStart = startPoint - lightScript.lightPosition;
            pointToEnd = endPoint - lightScript.lightPosition;
            // �O�όv�Z
            crossProduct = Vector3.Cross(pointToStart, pointToEnd).z;
            // �O�ς���������0�Ȃ珈�����I��
            if (crossProduct < 0.005)
            {
                lineEnd = lightScript.lightPosition;
            }
            else
            {
                scheduleLineEnd = lightScript.lightPosition;
            }
        }
        // ���C�g�̏I���n�_��������ɂ��邩
        foreach (Vector2 endPosition in lightScript.endPosition)
        {
            // �X�^�[�g�ƃG���h�܂ł̃x�N�g�������߂�
            pointToStart = startPoint - endPosition;
            pointToEnd = endPoint - endPosition;
            // �O�όv�Z
            crossProduct = Vector3.Cross(pointToStart, pointToEnd).z;
            // �O�ς���������0�Ȃ珈�����I��
            if (crossProduct < 0.005)
            {
                lineEnd = endPosition;
            }
            else
            {
                scheduleLineEnd = endPosition;
            }
        }
        return lineEnd;
    }

    /// <summary>
    /// �w��̍��W�����C�g�̌��ɓ������Ă��邩���ׂ�
    /// </summary>
    /// <param name="targetPosition">���̈ʒu�����C�g�̌��ɓ������Ă��邩���ׂ���</param>
    /// <param name="skipEnableLight">�L�����̃��C�g�̌������ׂ邩</param>
    /// <returns>���C�g�̌����󂯂Ă�����TRUE�A�󂯂Ă��Ȃ����FALSE</returns>
    private bool IsLightHit(Vector2 targetPosition, bool skipEnableLight = false, bool addHitLightToLight = false)
    {
        hittingLight.Clear();
        bool isHit = false;
        foreach (GameObject light in spotLights)
        {
            // �X�N���v�g�擾
            SpotLightArea lightScript = light.GetComponent<SpotLightArea>();

            if (skipEnableLight && light == enableLight.Value)
            {
                // �L�����̃��C�g���`�F�b�N���Ȃ��ꍇ��΂�
                continue;
            }
            if (lightScript.defaultLight)
            {
                // ��r����x�N�g���v�Z
                Vector2 standardVector =
                    (lightScript.lightPosition -
                        (lightScript.forwardDirection +
                        lightScript.lightPosition))
                        .normalized;
                Vector2 targetVector =
                    (lightScript.lightPosition - targetPosition)
                    .normalized;

                // ���C�g�܂ł̊p�x�����߂�
                float angle =
                    Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                    Mathf.Rad2Deg;

                if (angle < lightScript.spotAngle / 2)
                {
                    isHit = true;
                    if (addHitLightToLight)
                    {
                        // ���C�g�̒��Ȃ猻�݂̃��C�g�����X�g�ɕۑ�
                        hittingLight.Add(light);
                    }

                }
                if (debug)
                {
                    Debug.DrawRay(lightScript.lightPosition, lightScript.forwardDirection * 100, color: Color.black);
                    Debug.DrawLine(lightScript.lightPosition, targetPosition, color: Color.gray);
                }
            }
        }
        if (debug)
        {
            Debug.Log($"{hittingLight.Count} in hittingLight");
        }
        return isHit;
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

    /// <summary>
    /// �X�^�[�g�n�_�����ԋ߂������_�����߂�
    /// </summary>
    /// <param name="lineStart">���肽�����̎n�_</param>
    /// <param name="lineEnd">���肽�����̏I�_</param>
    /// <param name="exclusions">���������O���郉�C�g�I�u�W�F�N�g</param>
    /// <returns>�����_�A�����_�̖@���x�N�g���A������������I�u�W�F�N�g</returns>
    private (Vector2 nextIntersection, Vector2 normal, GameObject intersectObject) FindNearestIntersection(Vector2 lineStart, Vector2 lineEnd, GameObject[] exclusions)
    {
        Vector2 intersection;      // ���ׂĂ̌�_���W
        Vector2 normal;            // ���ׂĂ̖@���x�N�g��
        Vector2 nearestIntersection = Vector2.positiveInfinity;             // ��ԋ߂���_���W
        Vector2 nearestNormal = Vector2.zero;                   // ��ԋ߂��ꏊ�̖@���x�N�g��
        GameObject nearestGameObject = enableLight.Value;                           // ��ԋ߂���_�̌��̃Q�[���I�u�W�F�N�g


        // �󂯎�������Ɍ������W�����ׂċ��߂�
        foreach (GameObject light in spotLights)
        {
            if (!exclusions.Contains(light))
            {
                // ����̃X�N���v�g�擾
                SpotLightArea lightScript =
                    light.GetComponent<SpotLightArea>();
                // �����_�`�F�b�N
                for (int i = 0; i < lightScript.endPosition.Length; i++)
                {
                    // ���̐i�s�����̃x�N�g��
                    Vector2 lineSegmentAVector = lineEnd - lineStart;
                    // ��r���������̃x�N�g��
                    Vector2 lineSegmentBVector =
                        lightScript.endPosition[i] - lightScript.startPosition[i];

                    // 2�̕ӂ�����������e�X�g����
                    float t, s;
                    bool isIntersection = IsIntersect(
                        lineStart,
                        lineSegmentAVector,
                        lightScript.startPosition[i],
                        lineSegmentBVector,
                        out t,
                        out s
                        );

                    // 2�̕ӂ���������Ƃ�
                    if (isIntersection)
                    {
                        // ���W�����߂�
                        intersection = lineStart + t * lineSegmentAVector;
                        normal = lineSegmentBVector.normalized;

                        // ��ԋ߂����W�Ɣ�r���ċ߂�����ۑ�
                        Vector2 closerIntersection = nearestIntersection - lineStart;   // ���݈�ԋ߂�
                        Vector2 comparisonIntersection = intersection - lineStart;  // ���̔�r�Ώ�

                        if (closerIntersection.magnitude > comparisonIntersection.magnitude)
                        {
                            // ��ԋ߂�
                            nearestIntersection = intersection;
                            nearestNormal = normal;
                            nearestGameObject = light;
                        }
                    }
                }
            }
        }

        return (nearestIntersection, nearestNormal, nearestGameObject);
    }
}*/