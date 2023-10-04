using UnityEngine;
using UnityEngine.Rendering;

public partial class Player
{
    // �p�����[�^�[
    [Header("�v���C���[�ɉe�������")]
    [SerializeField] private float m_speed;         // �v���C���[���x
    [SerializeField] private float m_jumpForce;     // �W�����v��
    [SerializeField] private float m_gravityScale;  //�@�d�͂̑傫��
    [SerializeField] private float airborneTime;    // �؋󎞊�

    [Header("���C�g�Ɋւ��ϐ�")]
    [SerializeField] private float pickReach;       // �E����͈�
    [SerializeField] private float defaultRadius;   // ���C�g�̖��܂�h�~�̕��ʂ͈̔�
    [SerializeField] private Vector2 lightSize;

    private void Movement()
    {
        // �v���C���[�Ɉړ��ʂ����Z
        velocity.x = moveInput.x * m_speed * Time.deltaTime;
        // �Ζʂ������ꍇ�Ƀx�N�g����ύX����
        if (Vector2.Angle(Vector2.right, groundStateScript.Slope(velocity)) < 50)
        {
            velocity = groundStateScript.Slope(velocity);
        }
        else
        {
            isFall = true;
        }

        if (moveInput.x != 0)
        {
            // �X�P�[�����ړ������ɍ��킹�ĕύX����
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

        if (isJump)
        {
            velocity.y = m_jumpForce * Time.deltaTime;
            isJump = false;
        }

        if (isFall)
        {
            velocity.y -= m_gravityScale * Time.deltaTime;
            isFall = false;
        }

        if (!groundStateScript.IsGround())
        {
            if (velocity.y < 0)
            {
                PlayAnimation(animationType.Fall);
            }
            else if (velocity.y > 0)
            {
                PlayAnimation(animationType.Jump);
            }
        }
        else if (velocity.magnitude == 0)
        {
            PlayAnimation(animationType.Idle);
        }
    }

    private void ChangeSpotLightDirection()
    {
        // �͂��擾
        Vector2 changeLightPosition
            = new Vector2(
                Mathf.Abs(distanceToLight.x)
                * lightDirection,
                distanceToLight.y
                );
        Vector2 objectHitPosition
            = playerPosition
            - changeLightPosition;

        // ���C�g������ꏊ�ɃI�u�W�F�N�g���Ȃ���
        RaycastHit2D lightObjectivePosition = Isburied(objectHitPosition);

        if (!lightObjectivePosition)
        {
            isWall = false;
            spotLight.transform.position = objectHitPosition;
        }
        else
        {
            isWall = true;
        }

        // ���C�g�̌�����ύX
        Vector3 spotLightScale = spotLight.transform.localScale;
        // �����l�����߂�
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        // ���͂��ꂽ�����ɐ؂�ւ���
        spotLightScale.x *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
    }

    private RaycastHit2D Isburied(Vector2 position)
    {
        return Physics2D.BoxCast(
                position,
                lightSize,
                0,
                Vector2.zero,
                0,
                stageLayer
                );
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spotLight.transform.position, lightSize);
    }
}
