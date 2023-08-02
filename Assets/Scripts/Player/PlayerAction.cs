using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Vector2 velocity;
    [Header("プレイヤーに影響する力")]
    [SerializeField] private float m_speed;         // プレイヤー測度
    [SerializeField] private float m_jumpForce;     // ジャンプ力
    [SerializeField] private float m_gravityScale;  //　重力の大きさ

    /// <summary>
    /// プレイヤーの動く
    /// </summary>
    private void PlayerMove()
    {
        // 現在のvelocityを取得
        velocity = rigidbody2d.velocity;

        // 左右移動
        velocity.x = moveInput * m_speed * Time.deltaTime;

        // 斜面だった場合にベクトルを変更する
        velocity = groundStateScript.Slope(velocity);

        // 地面にいるとき/いないときの処理
        if (groundStateScript.isGround())
        {
            // ジャンプ
            if (isJump)
            {
                velocity.y += m_jumpForce;
                isJump = false;
            }
        }
        else
        {
            //重力を付ける
            velocity.y -= m_gravityScale * Time.deltaTime;
        }

        // 最終的な移動量を適用
        rigidbody2d.velocity = velocity;
    }


}
