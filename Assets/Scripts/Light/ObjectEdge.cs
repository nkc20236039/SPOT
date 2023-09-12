using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask indexLayerMask;    // 画面フレームのレイヤーマスク
    [SerializeField]
    private LayerMask objectLayerMask;          // オブジェクトのレイヤーマスク
    [SerializeField]
    private GameObject enableLight;             // 有効のライト
    private Collider2D objectCollider;          // 影ができるポイントの元コライダー
    public bool isEnable { get; private set; }
    [SerializeField] bool debug = true;
    // 影の情報
    // 0: 影ができる角の位置
    // 1: カメラフレーム到達点
    // 2: 実際に影がついた位置
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
    }

    void Update()
    {
        // 光に当たっていなかったら処理を実行しない
        if (debug)
        {
            Debug.Log(!IsExposedToLight());
        }
        isEnable = IsExposedToLight();
        if (!isEnable) { return; }

        // 影ができる角の位置を設定
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            enableLight.transform.position
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

    private bool IsExposedToLight()
    {
        // ライトの方へ少し寄った場所の座標を求める
        Vector3 lightSidePosition = shadowSideInfo[0];
        lightSidePosition +=
            (enableLight.transform.position
            - lightSidePosition).normalized * 0.1f;
        // TODO: 正しく影の形を作れないのを直す
        Debug.DrawLine(lightSidePosition, enableLight.transform.position);
        return !Physics2D.Linecast(lightSidePosition, enableLight.transform.position, objectLayerMask);
    }

}
