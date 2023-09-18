using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField] private Vector3 parentPos;     // �����Ă��鎞�̃v���C���[����̋���
    [SerializeField] private GameObject[] spotlight;// �V�[���ɑ��݂���X�|�b�g���C�g

    private void PlayerMove()
    {
        // �v���C���[�Ɉړ��ʂ����Z
        velocity.x = moveInput.x * m_speed * Time.deltaTime;
        // �Ζʂ������ꍇ�Ƀx�N�g����ύX����
        velocity = groundStateScript.Slope(velocity);

        // �W�����v
        if (state == PlayerState.Jump)
        {
            velocity.y = m_jumpForce;
            // �W�����v��]�̓�����J�n����
            state = PlayerState.JumpTurn;
            StartCoroutine("JumpTurn");
        }
        if(state == PlayerState.JumpTurn)
        {
            velocity.y *= 0.8f;
        }


        if (moveInput.x != 0)
        {
            // �X�P�[�����ړ������ɍ��킹�ĕύX����
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

    }

    private IEnumerator JumpTurn()
    {
        yield return new WaitForSeconds(airborneTime);
        animator.SetBool("Jump", false);
        // �؋󎞊Ԃ𒴂������]���ė�������
        if (!groundStateScript.IsGround())
        {
            animator.SetTrigger("JumpTurn");
        }
    }

    /// <summary>
    /// ���C�g���E��/�u������̐؂�ւ�
    /// </summary>
    private void SwitchSpotLight()
    {
        // ���C�g�������Ă��邩�`�F�b�N
        bool haveLight = GameObject.Find("Light").transform.IsChildOf(transform);

        if (haveLight)
        {
            // ���C�g�������Ă��鎞���C�g��u��
            this.gameObject.transform.DetachChildren();
            haveLight = false;
        }
        else
        {
            // ���C�g�������Ă��Ȃ����̏���
            // �S�Ẵ��C�g���A���C�ɓ����
            GameObject[] light = GameObject.FindGameObjectsWithTag("Light");
            GameObject nearestObject = null;
            float nearestDistance = pickReach;

            // ���C�g�ƃv���C���[�̈ʒu�̋��������߂�
            foreach (GameObject thisObject in light)
            {
                Vector3 distance = thisObject.transform.position - transform.position;
                // �E����͈͓��Ƀ��C�g�����邩���ׂ�
                if (distance.magnitude <= pickReach && distance.magnitude < nearestDistance)
                {
                    nearestObject = thisObject;
                    nearestDistance = distance.magnitude;
                }
            }

            // �E����͈͓��Ƀ��C�g����������
            if (nearestObject != null)
            {
                // �e�q�֌W�ɓo�^����
                nearestObject.transform.parent = this.transform;
                // �i�s�����ɂȂ�悤�ɕ␳
                parentPos.x = Mathf.Abs(parentPos.x) * Mathf.Sign(transform.localScale.x) * -1;
                nearestObject.transform.localScale = Vector3.one;
                // ���W��ύX
                nearestObject.transform.position = transform.position + parentPos;


                haveLight = true;
            }
            else
            {
                // ���������ꍇ�̃��b�Z�[�W�𑗂�
                Debug.Log("�߂��Ƀ��C�g������\n--UI�쐬���ɉ�ʂɕ\������悤�ɂ���");
            }
        }
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
