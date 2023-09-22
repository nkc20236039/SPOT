using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public partial class Player
{
    private bool isRunning;     // 地面にいるとき
    private bool isJumping;     // ジャンプしている
    private bool isJumpTurn;    // 止まってジャンプ
    private bool isIdle;        // 地面で止まっている
    private bool isFalling;     // 空中で落下

    private void SetAnimation()
    {
        if (groundStateScript.IsGround())
        {
            // 動いているとき
            animator.SetBool("Run", isRunning && !isJumping);
            // アニメーターでアイドル時は実行される
            // animator.SetBool("Idle", isIdle);
            // ジャンプ
            animator.SetBool("Jump", isJumping);
        }
        else
        {
            // 空中でジャンプ回転
            animator.SetBool("JumpTurn", isJumpTurn);
            // 空中でジャンプ状態じゃない
            animator.SetBool("Fall", isFalling);
        }
    }
}
