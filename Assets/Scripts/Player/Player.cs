using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Rendering.Universal;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Idle,
    Run,
    Jump,
    JumpTurn,
    Fall,
}

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;

    private Vector2 moveInput;                // �ړ������擾
    private PlayerState state;

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private bool haveLight = true;                 // ���C�g�̏������
    private float mouseDelta;               // �}�E�X�̈ړ���

    [SerializeField] float detectionRange;
    [SerializeField] Vector2 distanceToLight;
    [SerializeField] GameObject spotLight;
    [SerializeField] LayerMask stageLayer;

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ���݂�velocity���擾
        velocity = rigidbody2d.velocity;

        // �n�㔻��
        if (groundStateScript.IsGround())
        {
            animator.SetBool("JumpTurn", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            if (velocity.x != 0 && state != PlayerState.JumpTurn && state != PlayerState.Jump)
            {
                // �W�����v���Ă��Ȃ�&
                // �ړ����Ă�������A�j���[�V����������
                animator.SetBool("Run", true);
            }
            else
            {
                // �ړ����ĂȂ�������
                animator.SetBool("Run", false);
            }
        }
        else
        {
            if (state != PlayerState.JumpTurn)
            {
                // ������Ԃ̂Ƃ��d�͂�ݒ�
                velocity.y = -m_gravityScale;
                animator.SetBool("Fall", true);
            }
        }

        // �v���C���[�̈ړ���
        PlayerMove();

        // �J�����̈ړ��ꏊ��ݒ�
        if (haveLight)
        {
            ChangeSpotLightDirection();
        }
        // �ŏI�I�Ȉړ��ʂ�K�p
        rigidbody2d.velocity = velocity;

        // �X�|�b�g���C�g�̕����𒲐�
        var mouse = Mouse.current;
        if (mouse != null && isRightClicking)
        {
            mouseDelta = mouse.delta.ReadValue().x;
            if (Mathf.Abs(mouseDelta) >= detectionRange)
            {
                // �}�E�X�𓮂��������փ��C�g���������悤�ɂ���
                lightDirection = (int)Mathf.Sign(mouseDelta);
                Debug.Log(lightDirection);
            }
        }
    }

    /// <summary>
    /// ���E�̓��͎擾
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // ���͕������擾
        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// �W�����v�̓��͎擾
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        if (groundStateScript.IsGround())
        {
            // �n��ɂ���΃W�����v
            state = PlayerState.Jump;
            animator.SetBool("Jump", true);
        }

    }

    /// <summary>
    /// �E�N���b�N���m
    /// </summary>
    /// <param name="context"></param>
    public void LightFacing(InputAction.CallbackContext context)
    {
        isRightClicking = context.performed;
    }

    public void InteractLight(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        haveLight = haveLight ? false : true;
    }

    /// <summary>
    /// ������Ԃɐݒ�
    /// </summary>
    public void SetFallState()
    {
        animator.SetBool("Jump", false);
        animator.SetBool("JumpTurn", false);
        state = PlayerState.Fall;
    }
}
