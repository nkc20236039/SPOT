using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float moveInput;                // 移動方向取得
    private bool isJump;                    // ジャンプしたか
    private bool isPlayerOperation;          // プレイヤーを操作できるか

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // モード切り替え
        if (Input.GetButtonDown("Player Mode")) {
            isPlayerOperation = (isPlayerOperation) ? false : true;
            moveInput = 0;
        }
        // 入力チェック
        if (isPlayerOperation)
        {
            //プレイヤー操作
            moveInput = Input.GetAxisRaw("Horizontal");                         // 左右キー
            if (Input.GetButtonDown("Jump") && groundStateScript.isGround())    // ジャンプキー
            {
                isJump = true;
            }
            if (Input.GetButtonDown("Switch Spot Light"))                       // ライトの切り替えキー
            {
                SwitchSpotLight();
            }
        }
        else
        {
            // オブジェクトが影の影響を受けるかの切り替え
        }
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
}
