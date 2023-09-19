using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class Player
{

    // パラメーター
    [Header("プレイヤーに影響する力")]
    [SerializeField] private float m_speed;         // プレイヤー測度
    [SerializeField] private float m_jumpForce;     // ジャンプ力
    [SerializeField] private float m_gravityScale;  //　重力の大きさ
    [SerializeField] private float airborneTime;    // 滞空時間

    [Header("ライトに関わる変数")]
    [SerializeField] private float pickReach;       // 拾える範囲
    [SerializeField] private Vector3 parentPos;     // 持っている時のプレイヤーからの距離
    [SerializeField] private GameObject[] spotlight;// シーンに存在するスポットライト

    private void PlayerMove()
    {
        // プレイヤーに移動量を加算
        velocity.x = moveInput.x * m_speed * Time.deltaTime;
        // 斜面だった場合にベクトルを変更する
        velocity = groundStateScript.Slope(velocity);

        // ジャンプ
        if (state == PlayerState.Jump)
        {
            velocity.y = m_jumpForce;
            // ジャンプ回転の動作を開始する
            state = PlayerState.JumpTurn;
            StartCoroutine("JumpTurn");
        }
        if(state == PlayerState.JumpTurn)
        {
            velocity.y *= 0.8f;
        }


        if (moveInput.x != 0)
        {
            // スケールを移動方向に合わせて変更する
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

    }

    private IEnumerator JumpTurn()
    {
        yield return new WaitForSeconds(airborneTime);
        animator.SetBool("Jump", false);
        // 滞空時間を超えたら回転して落下する
        if (!groundStateScript.IsGround())
        {
            animator.SetTrigger("JumpTurn");
        }
    }

    private void ChangeSpotLight(int lightNumber)
    {
        lightNumber--;
        if (lightNumber < spotlight.Length)
        {
            foreach (GameObject light in spotlight)
            {
                if (light.activeSelf)
                {
                    light.SetActive(false);
                }
            }
            spotlight[lightNumber].SetActive(true);
        }
    }

}
