using UnityEngine;

public class ObjectEdge : MonoBehaviour
{
    [SerializeField]
    private LayerMask cameraFrameLayerMask;    // 画面フレームのレイヤーマスク
    [SerializeField]
    private LayerMask objectLayerMask;          // オブジェクトのレイヤーマスク
    [SerializeField]
    private GameObject enableLight;             // 有効のライト
    private Collider2D objectCollider;          // 影ができるポイントの元コライダー

    // 影の情報
    // 0: 影ができる角の位置
    // 1: カメラフレームについた場所の位置
    // 2: 実際に影がついた位置
    public Vector2[] shadowSideInfo { get; private set; } = new Vector2[3];

    private void Start()
    {
        objectCollider = transform.root.GetComponent<Collider2D>();
    }

    void Update()
    {
        // 光に当たっていなかったら無効にする
        gameObject.SetActive(!IsExposedToLight());
        if (!IsExposedToLight()) { return; }

        // 影ができる角の位置を設定
        shadowSideInfo[0] = transform.position;

        Vector2 lightDirection = (
            enableLight.transform.position
            - transform.position
            ).normalized;

        //カメラフレームについた場所の位置を求める
        RaycastHit2D displayFreamHit =
            Physics2D.Raycast(
                transform.position,
                -lightDirection,
                cameraFrameLayerMask
                );
        shadowSideInfo[1] = displayFreamHit.point;

        // 実際の影がついた位置を求める
        RaycastHit2D objectHit =
            Physics2D.Raycast(
                transform.position,
                -lightDirection,
                objectLayerMask
                );
        shadowSideInfo[2] = objectHit.point;
    }

    private bool IsExposedToLight()
    {
        // ライトの方へ少し寄った場所の座標を求める
        Vector3 lightSidePosition = transform.position;
        lightSidePosition =
            (enableLight.transform.position
            - lightSidePosition).normalized * 0.1f;

        return !objectCollider.OverlapPoint(lightSidePosition);
    }
}
