using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float moveInput;                // �ړ������擾
    private bool isJump;                    // �W�����v������
    private bool isPlayerOperation = true;          // �v���C���[�𑀍�ł��邩

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���[�h�؂�ւ�
        if (Input.GetButtonDown("Player Mode"))
        {
            isPlayerOperation = (isPlayerOperation) ? false : true;
            moveInput = 0;
        }
        // ���̓`�F�b�N
        if (isPlayerOperation)
        {
            //�v���C���[����
            moveInput = Input.GetAxisRaw("Horizontal");                         // ���E�L�[
            if (Input.GetButtonDown("Jump") && groundStateScript.isGround())    // �W�����v�L�[
            {
                isJump = true;
            }
            if (Input.GetButtonDown("Switch Spot Light"))                       // ���C�g�̐؂�ւ��L�[
            {
                SwitchSpotLight();
            }
        }
        else
        {
            // �I�u�W�F�N�g���e�̉e�����󂯂邩�̐؂�ւ�
        }

        // ���C�g�؂�ւ�
        if (Input.GetButtonUp("Point Light 1"))
        {
            SwitchSpotLight(1);
        }
        if (Input.GetButtonUp("Point Light 2"))
        {
            SwitchSpotLight(2);
        }
        if (Input.GetButtonUp("Point Light 3"))
        {
            SwitchSpotLight(3);
        }
        if (Input.GetButtonUp("Point Light 4"))
        {
            SwitchSpotLight(4);
        }
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
}
