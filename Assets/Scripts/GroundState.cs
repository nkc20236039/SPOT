using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundState : MonoBehaviour
{
    [SerializeField] private bool debug = false;        // Rayなどの表示
    [SerializeField] private LayerMask layerMask;       // レイヤーマスク
    [SerializeField] private Vector3 rayRelativePos;    // 最初のプレイヤーからの相対ポジション
    private Vector3 rayOrigin;

    public bool isGround()
    {
        // 初期スタート位置
        rayOrigin = transform.position + rayRelativePos;


        for (int i = 0; i < 3; i++)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(rayOrigin, Vector3.down, 0.1f, layerMask);

            // デバッグ
            if (debug) { Debug.DrawRay(rayOrigin, Vector3.down * 0.1f, Color.yellow); }

            // 次の検索位置へ
            rayOrigin.x += -rayRelativePos.x;

            if (hit.collider != null)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 坂を登る時の処理
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Vector3</returns>
    public Vector3 SlopeGoUp(RaycastHit2D hit)
    {


        return new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 坂を下る時の処理
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Vector3</returns>
    public Vector3 SlopeGoDown(RaycastHit2D hit)
    {


        return new Vector3(0, 0, 0);
    }

}
