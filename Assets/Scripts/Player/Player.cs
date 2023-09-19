using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerState
{
    Idle,
    Run,
    Jump,
    JumpTurn,
    Fall,
}

public partial class Player : MonoBehaviour
{
    public int lightDirection { get; private set; }

    private Vector2 moveInput;                // 移動方向取得
    private PlayerState state;

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;
    private bool isRightClicking;
    private float mouseDelta;

    [SerializeField] float detectionRange;

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // 地上判定
        if (groundStateScript.IsGround())
        {
            animator.SetBool("JumpTurn", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
            if (velocity.x != 0 && state != PlayerState.JumpTurn && state != PlayerState.Jump)
            {
                // ジャンプしていない&
                // 移動してたら歩きアニメーションをつける
                animator.SetBool("Run", true);
            }
            else
            {
                // 移動してなかったら
                animator.SetBool("Run", false);
            }
        }
        else
        {
            if (state != PlayerState.JumpTurn)
            {
                // 落下状態のとき重力を設定
                velocity.y = -m_gravityScale;
                animator.SetBool("Fall", true);
            }
        }

        PlayerMove();

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
                Debug.Log(lightDirection);
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
    }

    /// <summary>
    /// ジャンプの入力取得
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (groundStateScript.IsGround())
        {
            // 地上にいればジャンプ
            state = PlayerState.Jump;
            animator.SetBool("Jump", true);
        }
    }

    public void LightFacing(InputAction.CallbackContext context)
    {
        isRightClicking = context.performed;
    }


    /// <summary>
    /// 落下状態に設定
    /// </summary>
    public void SetFallState()
    {
        animator.SetBool("Jump", false);
        animator.SetBool("JumpTurn", false);
        state = PlayerState.Fall;
    }

}
