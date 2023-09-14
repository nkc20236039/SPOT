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
        GameObject shadowHitObject = objectHit.transform.gameObject;
        // �ǂ̃I�u�W�F�N�g�ɂ����Ȃ���O���������牼�̒l������
        if (!objectHit)
        {
            shadowSideInfo[1] = shadowSideInfo[0];
            shadowSideInfo[2] = shadowSideInfo[1];
            shadowHitObject = gameObject;
        }

        if (debug)
        {
            Debug.Log($"pos {(shadowSideInfo[0] - shadowSideInfo[2]).magnitude}");
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, displayFreamHit.point, color: Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, objectHit.point, color: Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ���C�g�̌����������Ă��邩�𒲂ׂ�
    /// </summary>
    /// <returns>�������Ă�����true</returns>
    public bool IsExposedToLight(GameObject light)
    {
        Vector2 lightPosition = light.transform.position;
        Vector2 thisPosition = transform.position;
        Vector2 direction = (thisPosition - lightPosition).normalized;
        Vector2 distance = thisPosition - lightPosition;
        Vector2 rootObjectDirection = (transform.position - transform.root.position).normalized * 0.00001f;

        // ���C�g�͈̔͊O���m���߂�
        float lightAngle = light.GetComponent<SpotLightArea>().SpotAngle;
        float thisAngle = Vector2.Angle(transform.right, distance);

        if (lightAngle / 2 < thisAngle)
        {
            // ���C�g�̊p�x���傫�������狭���œ������Ă��Ȃ����Ƃɂ���
            return false;
        }
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
