using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // �����ł���p�����[�^
    [SerializeField] private bool m_isSpotLight;
    [SerializeField] private bool m_defaultLight = true;                     // ���C�g�����̌��̗v�f��

    [Header("���C�g�̐ݒ�")]
    [SerializeField] private float m_spotAngle;        // ���C�g�̏Ƃ炷�L��
    [Space]
    [Header("���C�g�ȊO�̎������̐ݒ�")]
    [SerializeField] private Vector2[] m_startPosition;
    [SerializeField] private Vector2[] m_endPosition;

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
    }

    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_defaultLayerMask;                         // ���C���[�}�X�N

    private Vector2 oldPosition;        // 1�t���[���O�̈ʒu

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;

    private void Start()
    {
        // ���C�g�̏����ʒu�Ȃǎ擾
        LightSetting();
    }

    void Update()
    {
        LightSetting();
        if (oldPosition != lightPosition)
        {
            GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
            List<Vector2> shadowPositionIndex = new List<Vector2>();
            Dictionary<Vector2, Vector2[]>
                shadowPosition =
                new Dictionary<Vector2, Vector2[]>();
            List<Vector2> allEndOfShadow = new List<Vector2>();
            List<Vector2> plusendOfShadow = new List<Vector2>();
            List<Vector2> minusEndOfShadow = new List<Vector2>();
            List<Vector2> completionPoint = new List<Vector2>();

            // �e���W�̏����擾
            foreach (GameObject objectEdge in objectEdges)
            {
                ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

                // �������C�g�̑��΍��W�Ƃ��Ď󂯎��
                shadowPositionIndex.Add(
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
            // ���X�g���~���Ƀ\�[�g����
            shadowPositionIndex.Sort((a, b) => b.y.CompareTo(a.y));

            // �ォ�珇�ԂɃ��X�g�ɓ���Ă���

        }

        // �Ō�ɍ���̈ʒu��ۑ�
        oldPosition = lightPosition;
    }

    private void LightSetting()
    {
        // ���C�g�̈ʒu
        lightPosition = transform.position;

        // ���̏I�_
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * transform.right;
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * transform.right;

        // ���C�g�̏I�_�����߂�
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);

        if (rayVisible)
        {
            Debug.DrawLine(lightPosition, hitASide.point, color: Color.cyan);
            Debug.DrawLine(lightPosition, hitBSide.point, color: Color.cyan);
        }
    }
}
