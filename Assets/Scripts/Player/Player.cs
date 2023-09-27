using UnityEngine;


public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;
    public bool haveLight = true;                 // ���C�g�̏������

    private Vector2 moveInput;                // �ړ������擾

    private GroundState groundStateScript;  // �n�ʃ`�F�b�Nscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private bool onGravity;                 // �d�͂�����
    private bool isJump;                    // �W�����v
    private bool isFall;
    private float mouseDelta;               // �}�E�X�̈ړ���
    private Coroutine jumpCoroutine;
    private SpriteRenderer spotLightSpriteRenderer; // ���C�g�̐ݒu/�����Ă���Ƃ��̃X�v���C�g
    private bool chengedDirection = false;

    [SerializeField] private Sprite[] spotLightSprite;
    [SerializeField] private float detectionRange;
    [SerializeField] private Vector2 distanceToLight;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private LayerMask stageLayer;

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spotLightSpriteRenderer = spotLight.GetComponent<SpriteRenderer>();
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
        }
        else
        {
            isFall = true;
        }

        // ���C�g�̈ړ��ꏊ��ݒ�
        if (haveLight)
        {
            ChangeSpotLightDirection();
            mouseDelta = Input.GetAxis("Mouse X");
        }

        // �X�|�b�g���C�g�̕����𒲐�
        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1) && !chengedDirection)
        {
            // �}�E�X�𓮂��������փ��C�g���������悤�ɂ���
            lightDirection = (int)Mathf.Sign(mouseDelta);
            chengedDirection = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            // �{�^���𗣂�����ēx�g����悤�ɂ���
            chengedDirection = false;
        }

        // ���C�g�̎����ւ�
        if (Input.GetKeyDown(KeyCode.F))
        {
            haveLight = !haveLight;

            // �؂�ւ�����̃X�v���C�g�̏��
            if (haveLight)
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[1];
            }
            else
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[0];
            }
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
