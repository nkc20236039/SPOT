using System.Collections.Generic;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // �����ł���p�����[�^
    [SerializeField] private bool m_defaultLight = true;    // ���C�g�����̌��̗v�f��

    [Header("���C�g�̐ݒ�")]
    [SerializeField] private float m_spotAngle;        // ���C�g�̏Ƃ炷�L��
    public float SpotAngle
    {
        get { return m_spotAngle; }
        private set { m_spotAngle = value; }
    }

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
    }

    [Space]
    [SerializeField] private bool debug;               // �f�o�b�O�p
    [SerializeField] private LayerMask m_defaultLayerMask;  //���C���[�}�X�N
    [SerializeField] private LayerMask m_frameBetweenLayerMask;  // ���C���[�}�X�N
    [SerializeField] private GameObject cameraFrame;        // �J�����t���[���̃R���C�_�[���擾����p

    private Vector2 oldPosition;        // 1�t���[���O�̈ʒu

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;
    private PolygonCollider2D cameraCollider;

    private void Start()
    {
        cameraCollider = GetComponent<PolygonCollider2D>();
        // ���C�g�̏����ʒu�Ȃǎ擾
        LightSetting();
    }

    void Update()
    {
        LightSetting();
        if (oldPosition != lightPosition)
        {
            GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
            List<Vector2> arrivalPoints = new List<Vector2>();
            Dictionary<Vector2, Vector2[]>
                shadowPosition =
                new Dictionary<Vector2, Vector2[]>();
            List<Vector2> allEndOfShadow = new List<Vector2>();
            List<Vector2> plusEndOfShadow = new List<Vector2>();
            List<Vector2> minusEndOfShadow = new List<Vector2>();
            List<Vector2> completionPoint = new List<Vector2>();

            // �e���W�̏����擾
            foreach (GameObject objectEdge in objectEdges)
            {
                ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

                // �������C�g�̑��΍��W�Ƃ��Ď󂯎��
                if (objectEdgeScript.isEnable)
                {
                    arrivalPoints.Add(
                    gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[1])
                    );
                    shadowPosition.TryAdd(
                        gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[1]),
                        new Vector2[]
                            {
                                gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[0]),
                                gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[2])
                            }
                        );
                }

            }
            // ���X�g���~���Ƀ\�[�g����
            arrivalPoints.Sort((a, b) => b.y.CompareTo(a.y));

            // ���C�g�̍ŏ��ƍŌ���Ƃ肠���������
            plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(lightPosition));
            plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitASide.point));
            minusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitBSide.point));

            // �e�̌`�����
            for (int i = 0; i < arrivalPoints.Count; i++)
            {
                // �v���X�̏ꍇ�̏���
                if (arrivalPoints[i].y >= 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
                {
                    //SortImitateShadow(shadowPosition[arrivalPoints[i]]);
                }
            }
        }

        // �Ō�ɍ���̈ʒu��ۑ�
        oldPosition = lightPosition;
    }

    /// <summary>
    /// ���C�g�̐ݒ�A���W�̎擾
    /// </summary>
    private void LightSetting()
    {
        // ���C�g�̈ʒu
        lightPosition = transform.position;

        // ���̏I�_
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * transform.right;
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * transform.right;

        // ���C�g�̏I�_���擾
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);


        if (debug)
        {
            Debug.DrawLine(lightPosition, hitASide.point, color: Color.cyan);
            Debug.DrawLine(lightPosition, hitBSide.point, color: Color.cyan);
        }
    }

    //private Vector2 SortImitateShadow(Vector2[] pointArray)


    private Vector2[] FrameEdgeLightHit()
    {
        List<Vector2> lightHitPoint = new List<Vector2>();
        foreach (Vector2 polygonPoint in cameraCollider.points)
        {
            if (!Physics2D.Linecast(polygonPoint, lightPosition, m_frameBetweenLayerMask))
            {
                // �x�N�g�������߂�
                Vector2 pointDirection = lightPosition - polygonPoint;
                Vector2 lightDirection = lightPosition - hitASide.point;

                // �A���O���v�Z
                float pointAngle = Vector2.Angle(transform.right, pointDirection);
                float lightAngle = Vector2.Angle(transform.right, lightDirection);

                // ���C�g�̑傫����菬������
                if (pointAngle < lightAngle)
                {
                    // �����������烊�X�g�ɓ����
                    lightHitPoint.Add(polygonPoint);
                }
            }
        }

        return lightHitPoint.ToArray();
    }
}
