using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // �����ł���p�����[�^
    [SerializeField] private bool m_isSpotLight;
    [SerializeField] private bool m_defaultLight = true;                     // ���C�g�����̌��̗v�f��

    [Header("���C�g�̐ݒ�")]
    [SerializeField] private float m_spotAngle;        // ���C�g�̏Ƃ炷�L��
    [SerializeField] private float m_spotDirection;    // ���C�g�̌���
    [Space]
    [Header("���C�g�ȊO�̎������̐ݒ�")]
    [SerializeField] private Vector2[] m_startPosition;
    [SerializeField] private Vector2[] m_endPosition;

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
        m_spotDirection = Mathf.Clamp(m_spotDirection, 0.0f, 359.9f);
    }

    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_defaultLayerMask;                         // ���C���[�}�X�N

    private Vector2 oldPosition;        // 1�t���[���O�̈ʒu
    private List<Vector2[]> points = new List<Vector2[]>();  // �e���ł���_�̃��X�g

    public bool defaultLight { get { return m_defaultLight; } }
    public float spotAngle { get { return m_spotAngle; } }
    public Vector2 forwardDirection { get; private set; }
    public Vector2 lightPosition { get; private set; }
    public Vector2 lightAngle { get; private set; }
    public RaycastHit2D hitASide { get; private set; }
    public RaycastHit2D hitBSide { get; private set; }
    public Vector2[] startPosition { get; private set; }
    public Vector2[] endPosition { get; private set; }


    void Update()
    {
        if (defaultLight && oldPosition != lightPosition)
        {
            // ���C�g�̈ʒu
            lightPosition = transform.position;
            // ���C�g�̌���
            forwardDirection = Quaternion.Euler(0, 0, m_spotDirection) * Vector2.right;
            // ���̏I�_
            Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * forwardDirection;
            Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * forwardDirection;

            // ���C�g�̏I�_�����߂�
            hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
            hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);
            // ���X�g�ɕۑ�
            points.Add(new Vector2[] { lightPosition, hitASide.point });
            points.Add(new Vector2[] { hitASide.point, lightPosition });

            // �p�ɂ���shadowPoint���擾
            GameObject[] shadowPoints = GameObject.FindGameObjectsWithTag("ShadowPoint");

            // ���C�g�̌����͂������ׂ�
            foreach (GameObject shadowPoint in shadowPoints)
            {
                // shadowPoint�̍��W
                Vector2 shadowPointPosition = shadowPoint.transform.position;
                // ��r����x�N�g���v�Z
                Vector2 standardVector =
                    (lightPosition -
                        (forwardDirection +
                        lightPosition))
                        .normalized;
                Vector2 targetVector =
                    (lightPosition - shadowPointPosition)
                    .normalized;

                // ���C�g�܂ł̊p�x�����߂�
                float angle =
                    Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                    Mathf.Rad2Deg;

                if (angle < spotAngle / 2)
                {
                    // �W���ɂȂ�I�u�W�F�N�g���Ȃ������ׂ�
                    RaycastHit2D hit = Physics2D.Linecast(lightPosition, shadowPointPosition, m_defaultLayerMask);

                    if (!hit)
                    {
                        // ������Ȃ������Ƃ����̃I�u�W�F����L�т�e�̏I�_�����߂�

                    }
                }
            }
        }

        // �Ō�ɍ���̈ʒu��ۑ�
        oldPosition = lightPosition;
    }

    /// <summary>
    /// ���ݒn����߂����ɕ��ѕς���
    /// </summary>
    /// <param name="origin"></param>
    private Vector2[] SortByClosest(Vector2 origin, List<Vector2> pointList)
    {
        return pointList.OrderBy(point => Vector2.Distance(point, origin)).ToArray();
    }
}
