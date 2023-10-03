using System.Collections.Generic;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // �����ł���p�����[�^
    [SerializeField] private bool m_defaultLight = true;    // ���C�g�����̌��̗v�f��

    [Header("���C�g�̐ݒ�")]
    [SerializeField] private MeshRenderer shadowRenderer;
    [SerializeField] private float gravityScale;
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
    [SerializeField] private PolygonCollider2D lightAreaCollider;
    [SerializeField] private float reachColDistance;
    [SerializeField] private Player playerScript;
    public Texture2D shadowTexture;

    private Vector2 oldPosition;        // 1�t���[���O�̈ʒu

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;
    private PolygonCollider2D lightPolygon;
    private DelaunayTriangulationTester meshGenerateScript;
    private BoxCollider2D boxCollider;
    [SerializeField] private GroundState groundStateScript;

    private void Start()
    {
        GameObject lightCollider = transform.Find("LightCollider").gameObject;
        lightPolygon = lightCollider.GetComponent<PolygonCollider2D>();
        // ���C�g�̏����ʒu�Ȃǎ擾
        LightSetting();
        meshGenerateScript = GetComponent<DelaunayTriangulationTester>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void LateUpdate()
    {
        // �R���C�_�[�̍L����ݒ肷��
        GetComponent<EdgeCollider2D>().points =
            SetReachCollider();
        LightSetting();


        GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
        List<Vector2> arrivalPoints = new List<Vector2>();      // �e�̍ŏI�I�ȓ��B�_
        Dictionary<Vector2, Vector2[]>
            shadowPosition =
            new Dictionary<Vector2, Vector2[]>();
        Dictionary<Vector2, GameObject>
            shadowObject =
            new Dictionary<Vector2, GameObject>();
        List<Vector2> plusArrivalPoint = new List<Vector2>();    // �v���X�����̒��_
        List<Vector2> minusArrivalPoint = new List<Vector2>();   // �}�C�i�X�����̒��_
        List<Vector2> completionPoint = new List<Vector2>();    // �����������_�Q


        // �e���W�̏����擾
        foreach (GameObject objectEdge in objectEdges)
        {
            ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

            // �p�����C�g�̌��ɓ����Ă��邩�m���߂�
            if (objectEdgeScript.IsExposedToLight(lightPosition))
            {
                // �������C�g�̑��΍��W�Ƃ��Ď󂯎��
                // �I�u�W�F�N�g�̊p�����擾����
                var edgeInformation = objectEdgeScript.GetEdgeInformation(gameObject.transform);
                Vector2[] shadowSideInfo = edgeInformation.shadowVector;

                // ����鏇�Ԃ��Ǘ�������W������
                arrivalPoints.Add(
                gameObject.transform.InverseTransformPoint(shadowSideInfo[1])
                );
                // ��̂���ɂ������̍��W������
                shadowPosition.TryAdd(
                    arrivalPoints[arrivalPoints.Count - 1],
                    new Vector2[]
                        {
                                gameObject.transform.InverseTransformPoint(shadowSideInfo[0]),
                                gameObject.transform.InverseTransformPoint(shadowSideInfo[2])
                        }
                    );
                shadowObject.TryAdd(
                    arrivalPoints[arrivalPoints.Count - 1],
                    edgeInformation.hitObject
                    );
            }
        }
        // ���X�g���~���Ƀ\�[�g����

        arrivalPoints.Sort((point1, point2) =>
        {
            // Y���W���قړ����ꍇ��X���ŏ����Ƀ\�[�g
            if (Mathf.Approximately(point1.y, point2.y))
            {
                return point1.x.CompareTo(point2.x);
            }
            // Y���W���قȂ�ꍇ��Y���W�ō~���Ƀ\�[�g
            else
            {
                return point2.y.CompareTo(point1.y);
            }
        });

        // ���C�g�̍ŏ��ƍŌ���Ƃ肠���������
        completionPoint.Add(gameObject.transform.InverseTransformPoint(lightPosition));
        completionPoint.Add(gameObject.transform.InverseTransformPoint(hitASide.point));

        // ���ׂĂ̓��B�_���v���X�ƃ}�C�i�X�ɕ�����
        for (int i = 0; i < arrivalPoints.Count; i++)
        {
            if (arrivalPoints[i].y >= 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                // �v���X�̒n�_��ۑ�
                plusArrivalPoint.Add(arrivalPoints[i]);
            }
            else if (shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                // �}�C�i�X�̒n�_��ۑ�
                minusArrivalPoint.Add(arrivalPoints[i]);
            }
        }

        float[] oldPointDistance = { 0f, 0f };
        // �v���X�̍��W�����������_���X�g�ɕ��ѕς���
        if (plusArrivalPoint.Count > 0)
        {
            if (hitASide.transform.gameObject != cameraFrame)
            {
                oldPointDistance[0] = gameObject.transform.InverseTransformPoint(hitASide.point).x - reachColDistance;
            }

            for (int i = 0; i < plusArrivalPoint.Count; i++)
            {
                completionPoint.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[plusArrivalPoint[i]],
                        reachColDistance,
                        ref oldPointDistance[0],
                        shadowObject[plusArrivalPoint[i]]
                    )
                );
            }
        }

        // �}�C�i�X�̍��W�����������_���X�g�ɕ��ѕς���
        if (minusArrivalPoint.Count > 0)
        {
            if (hitBSide.transform.gameObject != cameraFrame)
            {
                oldPointDistance[1] = gameObject.transform.InverseTransformPoint(hitBSide.point).x - reachColDistance;
            }
            minusArrivalPoint.Reverse();
            List<Vector2> arrivalPointStorage = new List<Vector2>();
            for (int i = 0; i < minusArrivalPoint.Count; i++)
            {
                // ��납�����Ă���
                arrivalPointStorage.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[minusArrivalPoint[i]],
                        reachColDistance,
                        ref oldPointDistance[1],
                        shadowObject[minusArrivalPoint[i]]
                    )
                );
            }
            arrivalPointStorage.Reverse();
            completionPoint.AddRange(arrivalPointStorage);
        }
        completionPoint.Add(gameObject.transform.InverseTransformPoint(hitBSide.point));

        // �d�Ȃ��Ă�����W���폜
        Vector2[] checkConflict = completionPoint.ToArray();
        int quantityRemoved = 0;
        Vector2 oldPoint = completionPoint[completionPoint.Count - 1];
        for (int i = 0; i < checkConflict.Length; i++)
        {
            if (checkConflict[i] == oldPoint)
            {
                // ���X�g�̑傫���ɍ��킹�č폜����
                completionPoint.RemoveAt(i - quantityRemoved);
                quantityRemoved++;
            }
            oldPoint = checkConflict[i];
        }


        // �|���S���R���C�_�[�ɔ��f����
        lightPolygon.points = completionPoint.ToArray();

        // ���b�V���\��
        meshGenerateScript.RunTestPolygonColliders();

        // �V�F�[�_�[�����[�h����Material�𐶐�
        Material material = new Material(Shader.Find("Unlit/SimpleTexture"));
        material.SetTexture("_MainTex", shadowTexture);

        // MeshRenderer��Material���Z�b�g
        shadowRenderer.material = material;

        // �Ō�ɍ���̈ʒu��ۑ�
        oldPosition = lightPosition;
        completionPoint.Clear();
    }

    /// <summary>
    /// ���C�g�̐ݒ�A���W�̎擾
    /// </summary>
    private void LightSetting()
    {
        // �d��
        if (!groundStateScript.IsGround() && (!playerScript.haveLight || playerScript.isWall) && m_defaultLight)
        {
            Vector3 position = transform.position;
            position.y -= gravityScale;
            transform.position = position;
        }

        // ���C�g�̈ʒu
        lightPosition = transform.position;

        // ���̏I�_
        m_spotAngle = Mathf.Abs(m_spotAngle);
        m_spotAngle *= playerScript.lightDirection;
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * (transform.right * playerScript.lightDirection);
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * (transform.right * playerScript.lightDirection);

        // ���C�g�̏I�_���擾
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);

        // ���C�g�̃`�F�b�N�R���C�_�[�ݒ�
        lightAreaCollider.points = new Vector2[]
        {
            SetReachCollider()[0],
            SetReachCollider()[1],
            Vector2.zero
        };


        if (debug)
        {
            Debug.DrawLine(lightPosition, hitASide.point, Color.yellow);
            Debug.DrawLine(lightPosition, hitBSide.point, Color.yellow);
        }
    }

    /// <summary>
    /// �z��̗v�f�����Ԃɍ����悤�ɕ��ѕς���
    /// </summary>
    /// <param name="pointArray">�I�u�W�F�N�g�̊p�Ɖe�̏I�_�����Ă���z��</param>
    /// <param name="referencePoint">��r����n�_</param>
    /// <param name="oldDistance">�O��̋���</param>
    private Vector2[] SortImitateShadow(Vector2[] pointArray, float referencePoint, ref float oldDistance, GameObject shadowHitObject)
    {
        // �p�����܂ł̋������r����
        float nowDistance = pointArray[0].x - referencePoint;
        if (oldDistance > nowDistance)
        {
            // �O���荡��̕����傫��������p�Ɖe���t�]����
            Vector2 temp = pointArray[0];
            pointArray[0] = pointArray[1];
            pointArray[1] = temp;
        }

        // ����̍��W��old�ɕۑ�����
        oldDistance = nowDistance;
        // �J�����t���[���̃R���C�_�[�ɉe���ڐG���Ă�����O��̒l�����Z�b�g����
        if (cameraFrame == shadowHitObject)
        {
            oldDistance = 0;
        }

        return pointArray;
    }


    /// <summary>
    /// �e�����B������W���擾���邽�߂̃R���C�_�[�̑傫���𒲐�����
    /// </summary>
    private Vector2[] SetReachCollider()
    {
        float[] pointsCalculation = new float[2];
        Vector2[] points = new Vector2[2];
        points[0] = new Vector2(reachColDistance, 0);
        points[1] = new Vector2(reachColDistance, 0);
        // �x���@����ʓx�@�ɕϊ�
        pointsCalculation[0] = SpotAngle / 2 * Mathf.Deg2Rad;
        pointsCalculation[1] = -SpotAngle / 2 * Mathf.Deg2Rad;

        // Y���W�����߂�
        points[0].y = Mathf.Tan(pointsCalculation[0]) * reachColDistance;
        points[1].y = Mathf.Tan(pointsCalculation[1]) * reachColDistance;

        return points;
    }

    /*private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < lightPolygon.points.Length; i++)
            {
                Handles.Label(lightPolygon.points[i], $"[{i}]");
            }
        }

    }*/
}
