using UnityEngine;
using UnityEngine.InputSystem;



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
        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // ジャンプ中に地面についた時
        if (groundStateScript.IsGround())
        {
            // 再生中のコルーチンを止める
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

        // プレイヤーの移動量
        Movement();

        // 重力をつける
        if (onGravity)
        {
            velocity.y -= m_gravityScale;
            PlayAnimation(animationType.Fall, -1);
        }

        // カメラの移動場所を設定
        if (haveLight)
        {
            ChangeSpotLightDirection();
        }
        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;

        // スポットライトの方向を調整
        var mouse = Mouse.current;
        if (mouse != null && isRightClicking)
        {
            mouseDelta = mouse.delta.ReadValue().x;
            if (Mathf.Abs(mouseDelta) >= detectionRange)
            {
                // マウスを動かした方へライトを向けれるようにする
                lightDirection = (int)Mathf.Sign(mouseDelta);
            }
        }
    }

    /// <summary>
    /// 左右の入力取得
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力方向を取得
        moveInput = context.ReadValue<Vector2>();
        PlayAnimation(animationType.Run);
    }

    /// <summary>
    /// ジャンプの入力取得
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (groundStateScript.IsGround())
        {
            // 地上にいればジャンプ
            isJump = context.performed;
        }
    }

    /// <summary>
    /// 右クリック検知
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
