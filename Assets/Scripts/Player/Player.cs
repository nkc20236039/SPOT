using UnityEngine;


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
    private bool isFall;
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
        // ���E���͎擾
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // �d�͂�����ꍇ�A�Ȃ��ꍇ�̏���
        if (groundStateScript.IsGround())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = true;
            }

            if (moveInput.x != 0)
            {
                PlayAnimation(animationType.Run);
            }
            else
            {
                PlayAnimation(animationType.Idle);
            }
        }
        else
        {
            isFall = true;
        }

        // ���C�g�̈ړ��ꏊ��ݒ�
        if (haveLight)
        {
            ChangeSpotLightDirection();
        }

        // �X�|�b�g���C�g�̕����𒲐�
        mouseDelta = Input.GetAxis("Mouse X");

        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1))
        {
            // �}�E�X�𓮂��������փ��C�g���������悤�ɂ���
            lightDirection = (int)Mathf.Sign(mouseDelta);
        }

        // ���C�g�̎����ւ�
        if (Input.GetKeyDown(KeyCode.F))
        {
            haveLight = !haveLight;
        }


    }

    private void FixedUpdate()
    {
        // ���݂�velocity���擾
        velocity = rigidbody2d.velocity;

        // �v���C���[�̈ړ���
        Movement();

        // �ŏI�I�Ȉړ��ʂ�K�p
        rigidbody2d.velocity = velocity;
    }
}
