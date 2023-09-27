using UnityEngine;


public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;
    public bool haveLight = true;                 // ライトの所持状態

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
        // 左右入力取得
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // 重力がある場合、ない場合の処理
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

        // ライトの移動場所を設定
        if (haveLight)
        {
            ChangeSpotLightDirection();
            mouseDelta = Input.GetAxis("Mouse X");
        }

        // スポットライトの方向を調整
        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1) && !chengedDirection)
        {
            // マウスを動かした方へライトを向けれるようにする
            lightDirection = (int)Mathf.Sign(mouseDelta);
            chengedDirection = true;
        }

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
            }
            else
            {
                spotLightSpriteRenderer.sprite = spotLightSprite[0];
            }
        }
    }

    private void FixedUpdate()
    {
        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // プレイヤーの移動量
        Movement();

        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;
    }
}
