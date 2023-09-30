using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;
    public bool haveLight = true;                 // ライトの所持状態
    public bool canPlayerControl = true;

    private Vector2 moveInput;                // 移動方向取得

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private bool onGravity;                 // 重力をつける
    private bool isJump;                    // ジャンプ
    private bool isFall;
    private float mouseDelta;               // マウスの移動量
    private Coroutine jumpCoroutine;
    private SpriteRenderer spotLightSpriteRenderer; // ライトの設置/持っているときのスプライト
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
        // 落下した時
        if (playerPosition.y < deathHeight)
        {

        }


        // 操作を受け付けないようにする
        if (!canPlayerControl)
        {
            return;
        }

        // カーソルの表示/非表示
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.visible = false;
        }


        // 左右入力取得
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // 重力がある場合、ない場合の処理
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

        // ライトの移動場所を設定
        if (haveLight)
        {
            ChangeSpotLightDirection();
            if (!Isburied(-lightDirection))
            {
                mouseDelta = Input.GetAxis("Mouse X");

            }
        }

        // スポットライトの方向を調整
        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1) && !chengedDirection)
        {
            // マウスを動かした方へライトを向けれるようにする
            int oldLightDirection = lightDirection;
            lightDirection = (int)Mathf.Sign(mouseDelta);
            if (oldLightDirection != lightDirection)
            {
                SEManager.Instance.Play(SEPath.LIGHT_SLIDE);
            }
            chengedDirection = true;
        }

        // カーソルの表示を変更
        isRightClick = Input.GetMouseButton(1);

        if (Input.GetMouseButtonUp(1))
        {
            // ボタンを離したら再度使えるようにする
            chengedDirection = false;
        }

        // ライトの持ち替え
        if (Input.GetKeyDown(KeyCode.F))
        {
            haveLight = !haveLight;

            // 切り替えた後のスプライトの状態
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

        // カーソルのアニメーションを設定する
        if (chengedDirection) { isRightClick = false; }
        mousePointerScript.SetCursorIcon(isRightClick, lightDirection, !Isburied(-lightDirection));
    }

    private void FixedUpdate()
    {
        // 操作できないときは実行しない
        if (!canPlayerControl)
        {
            PlayerInit();
        }

        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // プレイヤーの移動量
        Movement();

        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;
    }

    private void PlayerInit()
    {
        moveInput = Vector2.zero;
    }
}
