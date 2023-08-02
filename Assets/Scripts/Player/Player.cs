using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float moveInput;                // 移動方向取得
    private bool isJump;                    // ジャンプしたか

    private GroundState groundStateScript;  // 地面チェックscript
    private Rigidbody2D rigidbody2d;        // rigidbody

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 入力チェック
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) { isJump = true; }
    }

    void FixedUpdate()
    {
        PlayerMove();
    }
}
