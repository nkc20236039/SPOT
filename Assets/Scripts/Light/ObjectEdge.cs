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
            - transform.position
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
        GameObject shadowHitObject = objectHit.transform.gameObject;
        // どのオブジェクトにもつかない例外が生じたら仮の値を入れる
        if (!objectHit)
        {
            shadowSideInfo[1] = shadowSideInfo[0];
            shadowSideInfo[2] = shadowSideInfo[1];
            shadowHitObject = gameObject;
        }

        if (debug)
        {
            Debug.Log($"pos {(shadowSideInfo[0] - shadowSideInfo[2]).magnitude}");
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, displayFreamHit.point, color: Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, objectHit.point, color: Color.green);
        }

        return (shadowSideInfo, shadowHitObject);
    }

    /// <summary>
    /// ライトの光が当たっているかを調べる
    /// </summary>
    /// <returns>当たっていたらtrue</returns>
    public bool IsExposedToLight(GameObject light)
    {
        Vector2 lightPosition = light.transform.position;
        Vector2 thisPosition = transform.position;
        Vector2 direction = (thisPosition - lightPosition).normalized;
        Vector2 distance = thisPosition - lightPosition;
        Vector2 rootObjectDirection = (transform.position - transform.root.position).normalized * 0.00001f;

        // ライトの範囲外か確かめる
        float lightAngle = light.GetComponent<SpotLightArea>().SpotAngle;
        float thisAngle = Vector2.Angle(transform.right, distance);

        if (lightAngle / 2 < thisAngle)
        {
            // ライトの角度より大きかったら強制で当たっていないことにする
            return false;
        }
        // ライトからRayを出す
        RaycastHit2D objectHit =
            Physics2D.Raycast
            (
                lightPosition - rootObjectDirection,
                direction,
                distance.magnitude * 2,
                objectLayerMask
            );
        if (debug)
        {
            Debug.DrawLine(lightPosition, objectHit.point);
        }
        // 当たった地点とこのオブジェクトまでの距離を求める
        float hitDistance = (thisPosition - objectHit.point).magnitude;

        // 当たった地点が許容範囲内ならTrueを返す
        return hitDistance < radiusAllow;
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
