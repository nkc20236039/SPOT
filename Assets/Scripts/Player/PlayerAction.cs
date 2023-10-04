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
    [SerializeField] private float defaultRadius;   // ライトの埋まり防止の普通の範囲
    [SerializeField] private Vector2 lightSize;

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
            velocity.y = m_jumpForce * Time.deltaTime;
            isJump = false;
        }

        if (isFall)
        {
            velocity.y -= m_gravityScale * Time.deltaTime;
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
        // 力を取得
        Vector2 changeLightPosition
            = new Vector2(
                Mathf.Abs(distanceToLight.x)
                * lightDirection,
                distanceToLight.y
                );
        Vector2 objectHitPosition
            = playerPosition
            - changeLightPosition;

        // ライトがある場所にオブジェクトがないか
        RaycastHit2D lightObjectivePosition = Isburied(objectHitPosition);

        if (!lightObjectivePosition)
        {
            isWall = false;
            spotLight.transform.position = objectHitPosition;
        }
        else
        {
            isWall = true;
        }

        // ライトの向きを変更
        Vector3 spotLightScale = spotLight.transform.localScale;
        // 実効値を求める
        spotLightScale.x = Mathf.Abs(spotLightScale.x);
        // 入力された方向に切り替える
        spotLightScale.x *= lightDirection;
        spotLight.transform.localScale = spotLightScale;
    }

    private RaycastHit2D Isburied(Vector2 position)
    {
        return Physics2D.BoxCast(
                position,
                lightSize,
                0,
                Vector2.zero,
                0,
                stageLayer
                );
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spotLight.transform.position, lightSize);
    }
}
