using UnityEngine;
using UnityEngine.InputSystem;



public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;

    private Vector2 moveInput;                // �ړ������擾

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private bool haveLight = true;                 // ���C�g�̏������
    private bool onGravity;                 // �d�͂�����
    private bool isJump;                    // �W�����v
    private float mouseDelta;               // �}�E�X�̈ړ���
    private Coroutine jumpCoroutine;

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

        // �W�����v���ɒn�ʂɂ�����
        if (groundStateScript.IsGround())
        {
            // �Đ����̃R���[�`�����~�߂�
            if (jumpCoroutine != null)
            {
                StopCoroutine(jumpCoroutine);
            }

            if (velocity.x == 0f)
            {
                PlayAnimation(animationType.Idle);
            }
        }
        else if (jumpCoroutine == null)
        {
            onGravity = true;
        }

        // �v���C���[�̈ړ���
        Movement();

        // �d�͂�����
        if (onGravity)
        {
            velocity.y -= m_gravityScale;
            PlayAnimation(animationType.Fall, -1);
        }

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
        PlayAnimation(animationType.Run);
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
            isJump = context.performed;
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
        haveLight = !haveLight;
    }


}
