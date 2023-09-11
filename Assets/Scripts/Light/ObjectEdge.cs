using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask cameraFrameLayerMask;    // ��ʃt���[���̃��C���[�}�X�N
    [SerializeField]
    private LayerMask objectLayerMask;          // �I�u�W�F�N�g�̃��C���[�}�X�N
    [SerializeField]
    private GameObject enableLight;             // �L���̃��C�g
    private Collider2D objectCollider;          // �e���ł���|�C���g�̌��R���C�_�[

    // �e�̏��
    // 0: �e���ł���p�̈ʒu
    // 1: �J�����t���[���ɂ����ꏊ�̈ʒu
    // 2: ���ۂɉe�������ʒu
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
    }

    void Update()
    {
        // ���ɓ������Ă��Ȃ������疳���ɂ���
        gameObject.SetActive(!IsExposedToLight());
        if (!IsExposedToLight()) { return; }

        // �e���ł���p�̈ʒu��ݒ�
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            enableLight.transform.position
            - transform.position
            ).normalized;

        //�J�����t���[���ɂ����ꏊ�̈ʒu�����߂�
        RaycastHit2D displayFreamHit =
            Physics2D.Raycast(
                transform.position,
                -lightDirection,
                cameraFrameLayerMask
                );
        shadowSideInfo[1] = displayFreamHit.point;

        // ���ۂ̉e�������ʒu�����߂�
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                transform.position,
                -lightDirection,
                objectLayerMask
                );
        shadowSideInfo[2] = objectHit.point;
    }

    private bool IsExposedToLight()
    {
        // ���C�g�̕��֏���������ꏊ�̍��W�����߂�
        Vector3 lightSidePosition = transform.position;
        lightSidePosition =
            (enableLight.transform.position
            - lightSidePosition).normalized * 0.1f;

        return !objectCollider.OverlapPoint(lightSidePosition);
    }
}
