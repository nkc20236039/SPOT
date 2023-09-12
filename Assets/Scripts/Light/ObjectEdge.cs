using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask indexLayerMask;    // ��ʃt���[���̃��C���[�}�X�N
    [SerializeField]
    private LayerMask objectLayerMask;          // �I�u�W�F�N�g�̃��C���[�}�X�N
    [SerializeField]
    private GameObject enableLight;             // �L���̃��C�g
    private Collider2D objectCollider;          // �e���ł���|�C���g�̌��R���C�_�[
    public bool isEnable { get; private set; }
    [SerializeField] bool debug = true;
    // �e�̏��
    // 0: �e���ł���p�̈ʒu
    // 1: �J�����t���[�����B�_
    // 2: ���ۂɉe�������ʒu
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
    }

    void Update()
    {
        // ���ɓ������Ă��Ȃ������珈�������s���Ȃ�
        if (debug)
        {
            Debug.Log(!IsExposedToLight());
        }
        isEnable = IsExposedToLight();
        if (!isEnable) { return; }

        // �e���ł���p�̈ʒu��ݒ�
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            enableLight.transform.position
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

    private bool IsExposedToLight()
    {
        // ���C�g�̕��֏���������ꏊ�̍��W�����߂�
        Vector3 lightSidePosition = shadowSideInfo[0];
        lightSidePosition +=
            (enableLight.transform.position
            - lightSidePosition).normalized * 0.1f;
        // TODO: �������e�̌`�����Ȃ��̂𒼂�
        Debug.DrawLine(lightSidePosition, enableLight.transform.position);
        return !Physics2D.Linecast(lightSidePosition, enableLight.transform.position, objectLayerMask);
    }

}
