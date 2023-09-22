using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

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
        if (isJumping)
        {
            velocity.y = m_jumpForce;
            // ジャンプ回転の動作を開始する
            StartCoroutine("JumpTurn");
        }

        if (moveInput.x != 0)
        {
            // スケールを移動方向に合わせて変更する
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

        // ジャンプ中じゃなければ重力をつける
        if (!isJumpTurn && !isJumping)
        {
            velocity.y -= m_gravityScale;
            isFalling = true;
        }
    }

    private IEnumerator JumpTurn()
    {
        yield return new WaitForSeconds(airborneTime);

        // 移動していなければ回転する
        if (!isRunning)
        {
            isJumpTurn = true;
            isJumping = false;
        }
    }

    private void ChangeSpotLightDirection()
    {
        Transform lightShadow = spotLight.transform.Find("Shadow");
        Vector2 playerPosition = transform.position;
        Vector2 spotLightPosition = lightShadow.position;
        // ライトの位置を変更
        spotLight.transform.position = playerPosition + -distanceToLight * lightDirection;

        // ライトの向きを変更
        Vector3 spotLightScale = spotLight.transform.localScale;
        Vector3 shadowScale = spotLight.transform.Find("Shadow").localScale;
        // 実効値を求める
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        spotLightScale.y = Mathf.Abs(spotLightScale.y);
        spotLightScale.z = Mathf.Abs(spotLightScale.z);
        shadowScale.z = Mathf.Abs(shadowScale.z);
        // 入力された方向に切り替える
        spotLightScale *= lightDirection;
        shadowScale.z *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
        lightShadow.localScale = shadowScale;
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
