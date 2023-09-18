using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    private Vector2 moveInput;                // 移動方向取得
    private bool isJump;                    // ジャンプしたか
    private bool isJumping;                 // ジャンプ中のロールリング
    private bool isPlayerOperation = true;          // プレイヤーを操作できるか

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody
    private Vector2 velocity;
    private Animator animator;

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

        PlayerMove();

        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;
    }

    /// <summary>
    /// 左右の入力取得
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力方向を取得
        moveInput = context.ReadValue<Vector2>();

        // アニメーション再生
        animator.SetBool("IsRun", context.performed);
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
            isJump = context.performed;

        }
    }


    /// <summary>
    /// 重力を設定
    /// </summary>
    public void SetGravity()
    {
        
        isJump = false;
        isJumping = false;
        // アニメーション
        animator.SetBool("Fall", true);
    }

    private void KeyBoardOperation()
    {
        // モード切り替え
        if (Input.GetButtonDown("Player Mode"))
        {
            isPlayerOperation = (isPlayerOperation) ? false : true;
            
        }
        // 入力チェック

        //プレイヤー操作                         // 左右キー
        if (Input.GetButtonDown("Jump") && groundStateScript.IsGround())    // ジャンプキー
        {
            isJump = true;
        }
        /*if (Input.GetButtonDown("Switch Spot Light"))                       // ライトの切り替えキー
        {
            SwitchSpotLight();
        }


        // ライト切り替え
        if (Input.GetButtonUp("Point Light 1"))
        {
            SwitchSpotLight(1);
        }
        if (Input.GetButtonUp("Point Light 2"))
        {
            SwitchSpotLight(2);
        }
        if (Input.GetButtonUp("Point Light 3"))
        {
            SwitchSpotLight(3);
        }
        if (Input.GetButtonUp("Point Light 4"))
        {
            SwitchSpotLight(4);
        }*/
    }
}
