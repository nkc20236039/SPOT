using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float moveInput;                // �ړ������擾
    private bool isJump;                    // �W�����v������

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ���̓`�F�b�N
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

    void FixedUpdate()
    {
        PlayerMove();
    }
}
