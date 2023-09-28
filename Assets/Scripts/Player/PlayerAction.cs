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
    [SerializeField] private GameObject[] spotlight;// シーンに存在するスポットライト
    [SerializeField] private float defaultRadius;   // ライトの埋まり防止の普通の範囲

    private void Movement()
    {
        // プレイヤーに移動量を加算
        velocity.x = moveInput.x * m_speed * Time.deltaTime;
        // 斜面だった場合にベクトルを変更する
        if (Vector2.Angle(Vector2.right, groundStateScript.Slope(velocity)) < 50)
        {
            velocity = groundStateScript.Slope(velocity);
        }
        else
        {
            isFall = true;
        }

        if (moveInput.x != 0)
        {
            // スケールを移動方向に合わせて変更する
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.x = (0 < moveInput.x) ? scale.x : -scale.x;
            transform.localScale = scale;
        }

        if (isJump)
        {
            velocity.y = m_jumpForce;
            isJump = false;
        }

        if (isFall)
        {
            velocity.y -= m_gravityScale;
            isFall = false;
        }

        if (!groundStateScript.IsGround())
        {
            if (velocity.y < 0)
            {
                PlayAnimation(animationType.Fall);
            }
            else if (velocity.y > 0)
            {
                PlayAnimation(animationType.Jump);
            }
        }
        else if (velocity.magnitude == 0)
        {
            PlayAnimation(animationType.Idle);
        }
    }

    private void ChangeSpotLightDirection()
    {
        Transform lightShadow = spotLight.transform.Find("Shadow");

        Vector2 spotLightPosition = lightShadow.position;

        // ライトがある場所にオブジェクトがないか


        // ライトの位置を変更
        if (!Isburied(lightDirection))
        {
            spotLight.transform.position =
            Vector3.MoveTowards(
                spotLight.transform.position,
                playerPosition + -distanceToLight * lightDirection,
                1
                );
        }

        // ライトの向きを変更
        Vector3 spotLightScale = spotLight.transform.localScale;
        // 実効値を求める
        distanceToLight.y = Mathf.Abs(distanceToLight.y);
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        // 入力された方向に切り替える
        distanceToLight.y *= -lightDirection;
        spotLightScale.x *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
    }

    private bool Isburied(int direction)
    {
        return Physics2D.CircleCast(
                playerPosition,
                defaultRadius,
                distanceToLight * direction,
                -distanceToLight.magnitude,
                stageLayer
                );
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
