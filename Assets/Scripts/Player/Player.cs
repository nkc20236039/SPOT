using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    private Vector2 moveInput;                // �ړ������擾
    private bool isJump;                    // �W�����v������
    private bool isJumping;                 // �W�����v���̃��[�������O
    private bool isPlayerOperation = true;          // �v���C���[�𑀍�ł��邩

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // ���݂�velocity���擾
        velocity = rigidbody2d.velocity;

        PlayerMove();

        // �ŏI�I�Ȉړ��ʂ�K�p
        rigidbody2d.velocity = velocity;
    }

    /// <summary>
    /// ���E�̓��͎擾
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // ���͕������擾
        moveInput = context.ReadValue<Vector2>();

        // �A�j���[�V�����Đ�
        animator.SetBool("IsRun", context.performed);
    }

    /// <summary>
    /// �W�����v�̓��͎擾
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (groundStateScript.IsGround())
        {
            // �n��ɂ���΃W�����v
            isJump = context.performed;

        }
    }


    /// <summary>
    /// �d�͂�ݒ�
    /// </summary>
    public void SetGravity()
    {
        
        isJump = false;
        isJumping = false;
        // �A�j���[�V����
        animator.SetBool("Fall", true);
    }

    private void KeyBoardOperation()
    {
        // ���[�h�؂�ւ�
        if (Input.GetButtonDown("Player Mode"))
        {
            isPlayerOperation = (isPlayerOperation) ? false : true;
            
        }
        // ���̓`�F�b�N

        //�v���C���[����                         // ���E�L�[
        if (Input.GetButtonDown("Jump") && groundStateScript.IsGround())    // �W�����v�L�[
        {
            isJump = true;
        }
        /*if (Input.GetButtonDown("Switch Spot Light"))                       // ���C�g�̐؂�ւ��L�[
        {
            SwitchSpotLight();
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
        }*/
    }
}
