using UnityEngine;


public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
    public Vector2 lightCallPosition;

    private Vector2 moveInput;                // 移動方向取得

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private bool haveLight = true;                 // ライトの所持状態
    private bool onGravity;                 // 重力をつける
    private bool isJump;                    // ジャンプ
    private bool isFall;
    private float mouseDelta;               // マウスの移動量
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
            else
            {
                PlayAnimation(animationType.Idle);
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
        }

        // スポットライトの方向を調整
        mouseDelta = Input.GetAxis("Mouse X");

        if (Mathf.Abs(mouseDelta) >= detectionRange && Input.GetMouseButton(1))
        {
            // マウスを動かした方へライトを向けれるようにする
            lightDirection = (int)Mathf.Sign(mouseDelta);
        }

        // ライトの持ち替え
        if (Input.GetKeyDown(KeyCode.F))
        {
            haveLight = !haveLight;
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
