using KanKikuchi.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; } = 1;
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
    private SpriteRenderer spotLightSpriteRenderer; // ライトの設置
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
        // BGM再生

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
        // 死亡演出
        if ((isLightOver || isfallOut) && !isPlayed)
        {
            cameraShakeScript.Shake(0.25f, 0.1f);
            SEManager.Instance.Play(SEPath.DEATH);
            systemButtonScript.Reload(1f);
            isPlayed = true;
        }

        // コマンド
        if (Input.GetKey(KeyCode.Backspace) && Input.GetKey(KeyCode.RightShift))
        {
            SceneManager.LoadScene("Stage1");
            BGMManager.Instance.Stop();
            BGMManager.Instance.Play(BGMPath.PEEKABOO, 0.25f);
            TouchClearFlag.Stage = 1;
        }


        // 操作を受け付けないようにする
        if (!canPlayerControl)
        {
            return;
        }

        // 左右入力取得
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // 重力がある場合、ない場合の処理
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

        // ライトの移動場所を設定
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
            // スポットライトの方向を変更
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

                // SE再生
                SEManager.Instance.Play(SEPath.FLIP_ERROR, 0.7f);
            }

            // ライトの向きや位置を決定
            ChangeSpotLightDirection();
        }
        // ライトの持ち替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            haveLight = !haveLight;

            // 切り替えた後のスプライトの状態
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

        // Rキーでリロード
        if (Input.GetKeyUp(KeyCode.R))
        {
            systemButtonScript.Reload(0);
        }
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

    private bool IsLightInside()
    {
        // 現在地がlightAreaレイヤーの中に存在しなければ
        // falseを返す
        if (!Physics2D.OverlapPoint(transform.position, lightAreaLayerMask)) { return false; }

        return !Physics2D.Linecast(transform.position, spotLight.transform.position, stageLayer);

    }
}
