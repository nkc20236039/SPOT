using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
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
    private SpriteRenderer spotLightSpriteRenderer; // ���C�g�̐ݒu
    private Vector2 playerPosition;
    private bool isRightClick;
    public bool isPlayed;
    private Rigidbody2D lightRigidbody;
    public bool isWall;

    [SerializeField] private Sprite[] spotLightSprite;
    [SerializeField] private float detectionRange;
    [SerializeField] private Vector2 distanceToLight;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private LayerMask stageLayer;
    [SerializeField] private float deathHeight;
    [SerializeField] private CameraShake cameraShakeScript;
    [SerializeField] private SystemButton systemButtonScript;
    [SerializeField] private LayerMask lightAreaLayerMask;
    [SerializeField] private GameObject errorCameraPrefab;

    void Start()
    {
        // BGM�Đ�

        BGMManager.Instance.UnPause();

        Time.timeScale = 1f;
        canPlayerControl = true;
        isPlayed = false;

        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spotLightSpriteRenderer = spotLight.GetComponent<SpriteRenderer>();
        lightRigidbody = spotLight.GetComponent<Rigidbody2D>();

        SEManager.Instance.ChangeBaseVolume(0.5f);
    }

    void Update()
    {
        playerPosition = transform.position;

        bool isLightOver = !IsLightInside();
        bool isfallOut = playerPosition.y < deathHeight;
        // ���S���o
        if ((isLightOver || isfallOut) && !isPlayed)
        {
            cameraShakeScript.Shake(0.25f, 0.1f);
            SEManager.Instance.Play(SEPath.DEATH);
            systemButtonScript.Reload(1f);
            isPlayed = true;
        }

        // �R�}���h
        if (Input.GetKey(KeyCode.Backspace) && Input.GetKey(KeyCode.RightShift))
        {
            SceneManager.LoadScene("Stage1");
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.PEEKABOO, 0.25f);
            TouchClearFlag.Stage = 1;
        }


        // ������󂯕t���Ȃ��悤�ɂ���
        if (!canPlayerControl)
        {
            return;
        }

        // ���E���͎擾
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // �d�͂�����ꍇ�A�Ȃ��ꍇ�̏���
        if (groundStateScript.IsGround())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isJump = true;
                SEManager.Instance.Play(SEPath.JUMP, pitch: Random.Range(0.8f, 1.1f));
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
            Vector2 checkLightPosition
            = new Vector2(
                Mathf.Abs(distanceToLight.x)
                * -lightDirection,
                distanceToLight.y
                );
            Vector2 objectHitPosition
            = playerPosition
            - checkLightPosition;
            // �X�|�b�g���C�g�̕�����ύX
            if (Input.GetKeyDown(KeyCode.F) && !Isburied(objectHitPosition))
            {
                lightDirection *= -1;
                SEManager.Instance.Play(SEPath.LIGHT_SLIDE);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject summonedPrefab = Instantiate(errorCameraPrefab, objectHitPosition, Quaternion.identity);
                Vector3 lightScale = spotLight.transform.localScale;
                lightScale.x = Mathf.Abs(lightScale.x);
                lightScale.x *= -lightDirection;
                summonedPrefab.transform.localScale = lightScale;
                Destroy(summonedPrefab, 1);

                // SE�Đ�
                SEManager.Instance.Play(SEPath.FLIP_ERROR, 0.7f);
            }

            // ���C�g�̌�����ʒu������
            ChangeSpotLightDirection();
        }
        // ���C�g�̎����ւ�
        if (Input.GetKeyDown(KeyCode.C))
        {
            haveLight = !haveLight;

            // �؂�ւ�����̃X�v���C�g�̏��
            if (haveLight)
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[1];
                SEManager.Instance.Play(SEPath.LIGHT_PICK, pitch: 1.25f);
            }
            else
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[0];
                SEManager.Instance.Play(SEPath.LIGHT_PLACE);
            }
        }

        // R�L�[�Ń����[�h
        if (Input.GetKeyUp(KeyCode.R))
        {
            systemButtonScript.Reload(0);
        }
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

    private bool IsLightInside()
    {
        // ���ݒn��lightArea���C���[�̒��ɑ��݂��Ȃ����
        // false��Ԃ�
        if (!Physics2D.OverlapPoint(transform.position, lightAreaLayerMask)) { return false; }

        return !Physics2D.Linecast(transform.position, spotLight.transform.position, stageLayer);

    }
}
