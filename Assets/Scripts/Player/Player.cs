using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;
    public bool haveLight = true;                 // ���C�g�̏������
    public bool canPlayerControl = true;

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
    private Vector2 playerPosition;
    private bool isRightClick;

    [SerializeField] private Sprite[] spotLightSprite;
    [SerializeField] private float detectionRange;
    [SerializeField] private Vector2 distanceToLight;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private LayerMask stageLayer;
    [SerializeField] private MousePointer mousePointerScript;
    [SerializeField] private float deathHeight;

    void Start()
    {
        Time.timeScale = 1f;
        canPlayerControl = true;
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spotLightSpriteRenderer = spotLight.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        SEManager.Instance.ChangeBaseVolume(0.5f);
    }

    void Update()
    {
        playerPosition = transform.position;
        // ����������
        if (playerPosition.y < deathHeight)
        {

        }


        // ������󂯕t���Ȃ��悤�ɂ���
        if (!canPlayerControl)
        {
            return;
        }

        // �J�[�\���̕\��/��\��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }


        // ���E���͎擾
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // �d�͂�����ꍇ�A�Ȃ��ꍇ�̏���
        if (groundStateScript.IsGround())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = true;
                SEManager.Instance.Play(SEPath.JUMP, 1, 0, Random.Range(0.6f, 1.1f));
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
            if (!Isburied(-lightDirection))
            {
                mouseDelta = Input.GetAxis("Mouse X");

            }
        }

        // �X�|�b�g���C�g�̕����𒲐�
        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1) && !chengedDirection)
        {
            // �}�E�X�𓮂��������փ��C�g���������悤�ɂ���
            int oldLightDirection = lightDirection;
            lightDirection = (int)Mathf.Sign(mouseDelta);
            if (oldLightDirection != lightDirection)
            {
                SEManager.Instance.Play(SEPath.LIGHT_SLIDE);
            }
            chengedDirection = true;
        }

        // �J�[�\���̕\����ύX
        isRightClick = Input.GetMouseButton(1);

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
                SEManager.Instance.Play(SEPath.LIGHT_PICK);
            }
            else
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[0];
                SEManager.Instance.Play(SEPath.LIGHT_PLACE);
            }
        }

        // �J�[�\���̃A�j���[�V������ݒ肷��
        if (chengedDirection) { isRightClick = false; }
        mousePointerScript.SetCursorIcon(isRightClick, lightDirection, !Isburied(-lightDirection));
    }

    private void FixedUpdate()
    {
        // ����ł��Ȃ��Ƃ��͎��s���Ȃ�
        if (!canPlayerControl)
        {
            PlayerInit();
        }

        // ���݂�velocity���擾
        velocity = rigidbody2d.velocity;

        // �v���C���[�̈ړ���
        Movement();

        // �ŏI�I�Ȉړ��ʂ�K�p
        rigidbody2d.velocity = velocity;
    }

    private void PlayerInit()
    {
        moveInput = Vector2.zero;
    }
}
