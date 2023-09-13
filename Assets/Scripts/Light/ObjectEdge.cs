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

    private Collider2D objectCollider;          // 影ができるポイントの元コライダー
    public bool isEnable { get; private set; }
    SpotLightArea spotLightAreaScript;

    // 影の情報
    // 0: 影ができる角の位置
    // 1: カメラフレーム到達点
    // 2: 実際に影がついた位置
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
        spotLightAreaScript = enableLight.GetComponent<SpotLightArea>();
    }

    public void GetEdgeInformation(GameObject light)
    {
        // 光に当たっていなかったら処理を実行しない
        isEnable = IsExposedToLight();
        if (!isEnable) { return; }

        // 影ができる角の位置を設定
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            light.transform.position
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

        if (debug)
        {
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, displayFreamHit.point, color: Color.cyan);
            Debug.DrawLine(shadowSideInfo[0] - lightDirection, objectHit.point, color: Color.green);
        }

    }

    /// <summary>
    /// ライトの光が当たっているかを調べる
    /// </summary>
    /// <returns>当たっていたらtrue</returns>
    private bool IsExposedToLight()
    {
        Vector2 lightPosition = enableLight.transform.position;
        Vector2 thisPosition = transform.position;
        Vector2 direction = (thisPosition - lightPosition).normalized;
        Vector2 distance = thisPosition - lightPosition;
        Vector2 rootObjectDirection = (transform.position - transform.root.position).normalized * 0.01f;


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
