using System.Collections.Generic;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // 調整できるパラメータ
    [SerializeField] private bool m_defaultLight = true;    // ライトか他の光の要素か

    [Header("ライトの設定")]
    [SerializeField] private float m_spotAngle;        // ライトの照らす広さ
    public float SpotAngle
    {
        get { return m_spotAngle; }
        private set { m_spotAngle = value; }
    }

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
    }

    [Space]
    [SerializeField] private bool debug;               // デバッグ用
    [SerializeField] private LayerMask m_defaultLayerMask;  //レイヤーマスク
    [SerializeField] private LayerMask m_frameBetweenLayerMask;  // レイヤーマスク
    [SerializeField] private GameObject cameraFrame;        // カメラフレームのコライダーを取得する用

    private Vector2 oldPosition;        // 1フレーム前の位置

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;
    private PolygonCollider2D cameraCollider;

    private void Start()
    {
        cameraCollider = GetComponent<PolygonCollider2D>();
        // ライトの初期位置など取得
        LightSetting();
    }

    void Update()
    {
        LightSetting();
        if (oldPosition != lightPosition)
        {
            GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
            List<Vector2> arrivalPoints = new List<Vector2>();
            Dictionary<Vector2, Vector2[]>
                shadowPosition =
                new Dictionary<Vector2, Vector2[]>();
            List<Vector2> allEndOfShadow = new List<Vector2>();
            List<Vector2> plusEndOfShadow = new List<Vector2>();
            List<Vector2> minusEndOfShadow = new List<Vector2>();
            List<Vector2> completionPoint = new List<Vector2>();

            // 影座標の情報を取得
            foreach (GameObject objectEdge in objectEdges)
            {
                ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

                // 情報をライトの相対座標として受け取る
                if (objectEdgeScript.isEnable)
                {
                    arrivalPoints.Add(
                    gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[1])
                    );
                    shadowPosition.TryAdd(
                        gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[1]),
                        new Vector2[]
                            {
                                gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[0]),
                                gameObject.transform.InverseTransformPoint(objectEdgeScript.shadowSideInfo[2])
                            }
                        );
                }

            }
            // リストを降順にソートする
            arrivalPoints.Sort((a, b) => b.y.CompareTo(a.y));

            // ライトの最初と最後をとりあえず入れる
            plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(lightPosition));
            plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitASide.point));
            minusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitBSide.point));

            // 影の形を作る
            for (int i = 0; i < arrivalPoints.Count; i++)
            {
                // プラスの場合の処理
                if (arrivalPoints[i].y >= 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
                {
                    //SortImitateShadow(shadowPosition[arrivalPoints[i]]);
                }
            }
        }

        // 最後に今回の位置を保存
        oldPosition = lightPosition;
    }

    /// <summary>
    /// ライトの設定、座標の取得
    /// </summary>
    private void LightSetting()
    {
        // ライトの位置
        lightPosition = transform.position;

        // 光の終点
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * transform.right;
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * transform.right;

        // ライトの終点を取得
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);


        if (debug)
        {
            Debug.DrawLine(lightPosition, hitASide.point, color: Color.cyan);
            Debug.DrawLine(lightPosition, hitBSide.point, color: Color.cyan);
        }
    }

    //private Vector2 SortImitateShadow(Vector2[] pointArray)


    private Vector2[] FrameEdgeLightHit()
    {
        List<Vector2> lightHitPoint = new List<Vector2>();
        foreach (Vector2 polygonPoint in cameraCollider.points)
        {
            if (!Physics2D.Linecast(polygonPoint, lightPosition, m_frameBetweenLayerMask))
            {
                // ベクトルを求める
                Vector2 pointDirection = lightPosition - polygonPoint;
                Vector2 lightDirection = lightPosition - hitASide.point;

                // アングル計算
                float pointAngle = Vector2.Angle(transform.right, pointDirection);
                float lightAngle = Vector2.Angle(transform.right, lightDirection);

                // ライトの大きさより小さいか
                if (pointAngle < lightAngle)
                {
                    // 小さかったらリストに入れる
                    lightHitPoint.Add(polygonPoint);
                }
            }
        }

        return lightHitPoint.ToArray();
    }
}
