using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField] private ShadowEdgeAsset shadowEdgeDate;
    [SerializeField] private float test;
    Player playerScript;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
    }


    // 影の情報
    // 0: 影ができる角の位置
    // 1: 検出用コライダー到達点
    // 2: 実際に影がついた位置
    public (Vector2[] shadowVector, GameObject hitObject) GetEdgeInformation(Transform light)
    {
        Vector2[] shadowSideInfo = new Vector2[3];

        // 影ができる角の位置を設定
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            light.position
            - this.transform.position
            ).normalized * 0.1f;

        //カメラフレーム到達点を求める
        RaycastHit2D displayFreamHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                shadowEdgeDate.indexLayerMask
                );

        shadowSideInfo[1] = displayFreamHit.point;
        shadowSideInfo[1].x += displayFreamHit.distance;

        // 実際の影がついた位置を求める
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                shadowEdgeDate.objectLayerMask
                );
        shadowSideInfo[2] = objectHit.point;
        GameObject shadowHitObject;
        if (objectHit.transform != null)
        {
            // オブジェクトに当たったら当たったオブジェクトを入れる
            shadowHitObject = objectHit.transform.gameObject;
        }
        else
        {
            // オブジェクトに当たらなかったら親オブジェクトを当たったオブジェにする
            shadowHitObject = transform.root.gameObject;
        }

        // どのオブジェクトにもつかない例外が生じたら
        // 角の座標に揃える
        if (Mathf.Approximately(objectHit.distance, 0))
        {
            shadowSideInfo[2] = shadowSideInfo[0];
        }

        if (shadowEdgeDate.debug)
        {
            Vector2 debugFreamHit = new Vector2(shadowSideInfo[1].x - displayFreamHit.distance, shadowSideInfo[1].y);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, debugFreamHit, Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[2], Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ライトの光が当たっているかを調べる
    /// </summary>
    /// <returns>当たっていたらtrue</returns>
    public bool IsExposedToLight(Vector2 lightPosition)
    {
        Vector2 edgePosition = transform.position;
        Vector2 edgeDirection = edgePosition - lightPosition;
        float edgeDistance = edgeDirection.magnitude;
        // 現在地がlightAreaレイヤーの中に存在しなければ
        // falseを返す
        if (!Physics2D.OverlapPoint(transform.position, shadowEdgeDate.lightAreaLayerMask)) { return false; }

        // ライトからこのオブジェクトに向かってRayを射出
        RaycastHit2D[] hitPoints = Physics2D.RaycastAll(lightPosition, edgeDirection, edgeDistance, shadowEdgeDate.objectLayerMask);

        // 反対を向いていたら反転する
        if(playerScript.lightDirection < 0)
        {
            hitPoints.Reverse();
        }


        // 最初が自分と同じオブジェクトならtrue
        if (hitPoints[0].transform.gameObject == gameObject && hitPoints[0].transform.gameObject.layer == gameObject.layer) { return true; }

        List<int> untilHit = new List<int>();

        foreach (RaycastHit2D hit in hitPoints)
        {
            if (shadowEdgeDate.debug)
            {
                Debug.DrawLine(lightPosition, hit.point);
            }

            Vector2 lightNormal = edgeDirection.normalized;
            untilHit.Add(hit.transform.gameObject.layer);

            if (hit.transform.gameObject != gameObject && lightNormal.y > -0.002f)
            {
                // 自分以外に当たった時の処理
                // ちょっとずれた位置から
                // ライトに繋がるまでにオブジェクトが存在するか
                float gap = 0.01f;
                int direction = -1;
                for (int i = 0; i < 2; i++)
                {
                    direction *= -1;
                    Vector2 startPosition = new Vector2(lightPosition.x, lightPosition.y + (gap * direction));
                    Vector2 endPosition = new Vector2(edgePosition.x, lightPosition.y + (gap * direction));
                    if (!Physics2D.Linecast(startPosition, endPosition, shadowEdgeDate.groundLayerMask))
                    {
                        Debug.DrawLine(startPosition, endPosition, Color.red);
                        return true;
                    }
                }
            }
            else
            {
                // これまでに当たったオブジェクトの中に
                // グランドがなければtrue
                return !untilHit.Contains(12);
            }
        }

        return false;
    }

}
