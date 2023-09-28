using UnityEngine;

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
    [SerializeField] private GameObject[] spotlight;// �V�[���ɑ��݂���X�|�b�g���C�g
    [SerializeField] private float defaultRadius;   // ���C�g�̖��܂�h�~�̕��ʂ͈̔�

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
            velocity.y = m_jumpForce;
            isJump = false;
        }

        if (isFall)
        {
            velocity.y -= m_gravityScale;
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
        Transform lightShadow = spotLight.transform.Find("Shadow");

        Vector2 spotLightPosition = lightShadow.position;

        // ���C�g������ꏊ�ɃI�u�W�F�N�g���Ȃ���


        // ���C�g�̈ʒu��ύX
        if (!Isburied(lightDirection))
        {
            spotLight.transform.position =
            Vector3.MoveTowards(
                spotLight.transform.position,
                playerPosition + -distanceToLight * lightDirection,
                1
                );
        }

        // ���C�g�̌�����ύX
        Vector3 spotLightScale = spotLight.transform.localScale;
        // �����l�����߂�
        distanceToLight.y = Mathf.Abs(distanceToLight.y);
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        // ���͂��ꂽ�����ɐ؂�ւ���
        distanceToLight.y *= -lightDirection;
        spotLightScale.x *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
    }

    private bool Isburied(int direction)
    {
        return Physics2D.CircleCast(
                playerPosition,
                defaultRadius,
                distanceToLight * direction,
                -distanceToLight.magnitude,
                stageLayer
                );
    }



    private void ChangeSpotLight(int lightNumber)
    {
        lightNumber--;
        if (lightNumber < spotlight.Length)
        {
            foreach (GameObject light in spotlight)
            {
                if (light.activeSelf)
                {
                    light.SetActive(false);
                }
            }
            spotlight[lightNumber].SetActive(true);
        }
    }
}
