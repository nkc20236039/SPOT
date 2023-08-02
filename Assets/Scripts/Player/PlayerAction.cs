using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Vector2 velocity;
    [Header("�v���C���[�ɉe�������")]
    [SerializeField] private float m_speed;         // �v���C���[���x
    [SerializeField] private float m_jumpForce;     // �W�����v��
    [SerializeField] private float m_gravityScale;  //�@�d�͂̑傫��

    /// <summary>
    /// �v���C���[�̓���
    /// </summary>
    private void PlayerMove()
    {
        // ���݂�velocity���擾
        velocity = rigidbody2d.velocity;

        // ���E�ړ�
        velocity.x = moveInput * m_speed * Time.deltaTime;

        // �Ζʂ������ꍇ�Ƀx�N�g����ύX����
        velocity = groundStateScript.Slope(velocity);

        // �n�ʂɂ���Ƃ�/���Ȃ��Ƃ��̏���
        if (groundStateScript.isGround())
        {
            // �W�����v
            if (isJump)
            {
                velocity.y += m_jumpForce;
                isJump = false;
            }
        }
        else
        {
            //�d�͂�t����
            velocity.y -= m_gravityScale * Time.deltaTime;
        }

        // �ŏI�I�Ȉړ��ʂ�K�p
        rigidbody2d.velocity = velocity;
    }


}
