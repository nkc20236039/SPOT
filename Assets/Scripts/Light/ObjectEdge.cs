using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask indexLayerMask;    // 画面フレームのレイヤーマスク
    [SerializeField]
    private LayerMask objectLayerMask;          // オブジェクトのレイヤーマスク
    [SerializeField]
    private GameObject enableLight;             // 有効のライト
    [SerializeField] bool debug = true;
    [SerializeField] float radiusAllow;

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
                indexLayerMask
                );

        shadowSideInfo[1] = displayFreamHit.point;

        // 実際の影がついた位置を求める
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                shadowSideInfo[0] - lightDirection,
                -lightDirection,
                Mathf.Infinity,
                objectLayerMask
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

        if (debug)
        {
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[1], Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, shadowSideInfo[2], Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ライトの光が当たっているかを調べる
    /// </summary>
    /// <returns>当たっていたらtrue</returns>
    public bool IsExposedToLight(Vector2 lightPoint, Vector2 pointA, Vector2 pointB)
    {
        // Debug.Log($"{lightPoint}\n{pointA}\n{pointB}");

        Vector2 edgePoint = transform.position;
        Vector2 direction = (edgePoint - lightPoint).normalized * 0.0001f;
        Vector2 distance = edgePoint - lightPoint;

        // 各辺のベクトルを計算
        Vector2 pointAToLight = pointA - lightPoint;
        Vector2 pointBToPointA = pointB - pointA;
        Vector2 lightToPointB = lightPoint - pointB;

        // 各頂点から指定した座標へのベクトルを計算
        Vector2 edgeToLight = edgePoint - lightPoint;
        Vector2 edgeToPointA = edgePoint - pointA;
        Vector2 edgeToPointB = edgePoint - pointB;

        // 各辺の外積を計算
        float crossPointAToLight = Vector3.Cross(pointAToLight, edgeToLight).z;
        float crossPointBToPointA = Vector3.Cross(pointBToPointA, edgeToPointA).z;
        float crossLightToPointB = Vector3.Cross(lightToPointB, edgeToPointB).z;

        bool plusCross = crossPointAToLight >= 0 && crossPointBToPointA >= 0 && crossLightToPointB >= 0;
        bool minusCross = crossPointAToLight <= 0 && crossPointBToPointA <= 0 && crossLightToPointB <= 0;
        if (!(plusCross || minusCross))
        {
            // すべての外積が同じ符号でなければ強制で当たっていないことにする
            return false;
        }
        // ライトからRayを出す
        RaycastHit2D objectHit =
            Physics2D.Raycast
            (
                lightPoint + direction,
                direction,
                distance.magnitude * 2,
                objectLayerMask
            );
        if (debug)
        {
            Debug.DrawRay(lightPoint + direction, direction * 1000, Color.red);
            Debug.DrawLine(lightPoint, objectHit.point);
        }

        // 当たったオブジェクトが同じオブジェクトだったらtrue
        return objectHit.transform.gameObject == gameObject;
    }

    // ギズモの表示
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = Color.red;
        // Rayが当たっても許可する円を表示する
        Gizmos.DrawWireSphere(Vector2.zero, radiusAllow);
    }
}
