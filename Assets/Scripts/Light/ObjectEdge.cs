using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask indexLayerMask;    // ��ʃt���[���̃��C���[�}�X�N
    [SerializeField]
    private LayerMask objectLayerMask;          // �I�u�W�F�N�g�̃��C���[�}�X�N
    [SerializeField]
    private GameObject enableLight;             // �L���̃��C�g
    [SerializeField] bool debug = true;
    [SerializeField] float radiusAllow;

    private Collider2D objectCollider;          // �e���ł���|�C���g�̌��R���C�_�[
    public bool isEnable { get; private set; }
    SpotLightArea spotLightAreaScript;

    // �e�̏��
    // 0: �e���ł���p�̈ʒu
    // 1: �J�����t���[�����B�_
    // 2: ���ۂɉe�������ʒu
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
        spotLightAreaScript = enableLight.GetComponent<SpotLightArea>();
    }

    public void GetEdgeInformation(GameObject light)
    {
        // ���ɓ������Ă��Ȃ������珈�������s���Ȃ�
        isEnable = IsExposedToLight();
        if (!isEnable) { return; }

        // �e���ł���p�̈ʒu��ݒ�
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            light.transform.position
            - transform.position
            ).normalized * 0.1f;

        //�J�����t���[�����B�_�����߂�
        RaycastHit2D displayFreamHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                indexLayerMask
                );
        shadowSideInfo[1] = displayFreamHit.point;

        // ���ۂ̉e�������ʒu�����߂�
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                objectLayerMask
                );
        shadowSideInfo[2] = objectHit.point;

        if (debug)
        {
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, displayFreamHit.point, color: Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, objectHit.point, color: Color.green);
        }

    }

    /// <summary>
    /// ���C�g�̌����������Ă��邩�𒲂ׂ�
    /// </summary>
    /// <returns>�������Ă�����true</returns>
    private bool IsExposedToLight()
    {
        Vector2 lightPosition = enableLight.transform.position;
        Vector2 thisPosition = transform.position;
        Vector2 direction = (thisPosition - lightPosition).normalized;
        Vector2 distance = thisPosition - lightPosition;
        Vector2 rootObjectDirection = (transform.position - transform.root.position).normalized * 0.01f;


        // ���C�g����Ray���o��
        RaycastHit2D objectHit =
            Physics2D.Raycast
            (
                lightPosition - rootObjectDirection,
                direction,
                distance.magnitude * 2,
                objectLayerMask
            );
        if (debug)
        {
            Debug.DrawLine(lightPosition, objectHit.point);
        }
        // ���������n�_�Ƃ��̃I�u�W�F�N�g�܂ł̋��������߂�
        float hitDistance = (thisPosition - objectHit.point).magnitude;

        // ���������n�_�����e�͈͓��Ȃ�True��Ԃ�
        return hitDistance < radiusAllow;
    }

    // �M�Y���̕\��
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;
        // Ray���������Ă�������~��\������
        Gizmos.DrawWireSphere(Vector2.zero, radiusAllow);
    }
}
