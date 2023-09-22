using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private Vector3 parentPos;     // �����Ă��鎞�̃v���C���[����̋���
    [SerializeField] private GameObject[] spotlight;// �V�[���ɑ��݂���X�|�b�g���C�g

    private void PlayerMove()
    {
        // �v���C���[�Ɉړ��ʂ����Z
        velocity.x = moveInput.x * m_speed * Time.deltaTime;
        // �Ζʂ������ꍇ�Ƀx�N�g����ύX����
        velocity = groundStateScript.Slope(velocity);

        // �W�����v
        if (isJumping)
        {
            velocity.y = m_jumpForce;
            // �W�����v��]�̓�����J�n����
            StartCoroutine("JumpTurn");
        }

        if (moveInput.x != 0)
        {
            // �X�P�[�����ړ������ɍ��킹�ĕύX����
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

        // �W�����v������Ȃ���Ώd�͂�����
        if (!isJumpTurn && !isJumping)
        {
            velocity.y -= m_gravityScale;
            isFalling = true;
        }
    }

    private IEnumerator JumpTurn()
    {
        yield return new WaitForSeconds(airborneTime);

        // �ړ����Ă��Ȃ���Ή�]����
        if (!isRunning)
        {
            isJumpTurn = true;
            isJumping = false;
        }
    }

    private void ChangeSpotLightDirection()
    {
        Transform lightShadow = spotLight.transform.Find("Shadow");
        Vector2 playerPosition = transform.position;
        Vector2 spotLightPosition = lightShadow.position;
        // ���C�g�̈ʒu��ύX
        spotLight.transform.position = playerPosition + -distanceToLight * lightDirection;

        // ���C�g�̌�����ύX
        Vector3 spotLightScale = spotLight.transform.localScale;
        Vector3 shadowScale = spotLight.transform.Find("Shadow").localScale;
        // �����l�����߂�
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        spotLightScale.y = Mathf.Abs(spotLightScale.y);
        spotLightScale.z = Mathf.Abs(spotLightScale.z);
        shadowScale.z = Mathf.Abs(shadowScale.z);
        // ���͂��ꂽ�����ɐ؂�ւ���
        spotLightScale *= lightDirection;
        shadowScale.z *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
        lightShadow.localScale = shadowScale;
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
