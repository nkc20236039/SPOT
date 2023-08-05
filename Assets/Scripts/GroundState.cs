using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : MonoBehaviour
{
    [SerializeField] private bool debug = false;        // Rayなどの表示
    [SerializeField] private LayerMask layerMask;       // レイヤーマスク
    [SerializeField] private Vector3 rayRelativePos;    // 最初のプレイヤーからの相対ポジション
    private Vector3 rayOrigin;
    private Vector3 groundNormal;

    /// <summary>
    /// 地面判定
    /// </summary>
    /// <returns>bool</returns>
    public bool isGround()
    {
        // 初期スタート位置
        rayOrigin = transform.position + rayRelativePos;


        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(rayOrigin, Vector3.down, 0.01f, layerMask);

            // デバッグ
            if (debug) { Debug.DrawRay(rayOrigin, Vector3.down * 0.01f, Color.yellow); }

            // 次の検索位置へ
            rayOrigin.x += -rayRelativePos.x;

            // 地面にいるか
            if (hit.collider != null)
            {
                // 法線ベクトル代入
                groundNormal = hit.normal;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 坂の移動制限
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public Vector3 Slope(Vector3 vector)
    {
        if(isGround())
        {
            // 斜面のベクトルを求める
            return Vector3.ProjectOnPlane(vector, groundNormal);
        }

        return vector;
    }

}
