using System.Collections.Generic;
using UnityEngine;
using static DelaunayTriangulationTester;

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
    [SerializeField] private float reachColDistance;

    private Vector2 oldPosition;        // 1フレーム前の位置

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;
    private PolygonCollider2D lightPolygon;
    private DelaunayTriangulationTester meshGenerateScript;

    private void Start()
    {
        GameObject lightCollider = transform.Find("LightCollider").gameObject;
        lightPolygon = lightCollider.GetComponent<PolygonCollider2D>();
        // ライトの初期位置など取得
        LightSetting();
        meshGenerateScript = GetComponent<DelaunayTriangulationTester>();
    }

    void Update()
    {
        // コライダーの広さを設定する
        SetReachCollider();
        LightSetting();

        GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
        List<Vector2> arrivalPoints = new List<Vector2>();
        Dictionary<Vector2, Vector2[]>
            shadowPosition =
            new Dictionary<Vector2, Vector2[]>();
        Dictionary<Vector2, GameObject>
            shadowObject =
            new Dictionary<Vector2, GameObject>();
        List<Vector2> plusEndOfShadow = new List<Vector2>();    // プラス方向の頂点
        List<Vector2> minusEndOfShadow = new List<Vector2>();   // マイナス方向の頂点
        List<Vector2> completionPoint = new List<Vector2>();    // 完成した頂点群
        float[] oldPointDistance = { 0f, 0f };


        // 影座標の情報を取得
        foreach (GameObject objectEdge in objectEdges)
        {
            ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

            // 情報をライトの相対座標として受け取る
            if (objectEdgeScript.IsExposedToLight(gameObject))
            {
                // オブジェクトの角情報を取得する
                var edgeInformation = objectEdgeScript.GetEdgeInformation(gameObject.transform);
                Vector2[] shadowSideInfo = edgeInformation.shadowVector;

                // 入れる順番を管理する座標を入れる
                arrivalPoints.Add(
                gameObject.transform.InverseTransformPoint(shadowSideInfo[1])
                );
                // 上のを基準にした他の座標を入れる
                shadowPosition.TryAdd(
                    arrivalPoints[arrivalPoints.Count - 1],
                    new Vector2[]
                        {
                                gameObject.transform.InverseTransformPoint(shadowSideInfo[0]),
                                gameObject.transform.InverseTransformPoint(shadowSideInfo[2])
                        }
                    );
                shadowObject.TryAdd(
                    arrivalPoints[arrivalPoints.Count - 1],
                    edgeInformation.hitObject
                    );
            }
        }
        // リストを降順にソートする
        arrivalPoints.Sort((a, b) => b.y.CompareTo(a.y));

        // ライトの最初と最後をとりあえず入れる
        plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(lightPosition));
        plusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitASide.point));

        // 影の形を作る
        // プラスの場合の処理
        int plusPointCount = 0;
        for (int i = 0; i < arrivalPoints.Count; i++)
        {
            if (arrivalPoints[i].y >= 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                // 値をそのまま並び変える
                plusEndOfShadow.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[arrivalPoints[i]],
                        arrivalPoints[i].x,
                        ref oldPointDistance[0],
                        shadowObject[arrivalPoints[i]]
                    )
                );

                // プラスの数を数える
                plusPointCount++;
            }
        }
        // マイナスの場合の処理
        for (int i = plusPointCount; i < arrivalPoints.Count; i++)
        {
            Debug.Log($"minus {arrivalPoints[i].y}");
            if (arrivalPoints[i].y < 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                minusEndOfShadow.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[arrivalPoints[i]],
                       arrivalPoints[i].x,
                        ref oldPointDistance[1],
                        shadowObject[arrivalPoints[i]]
                    )
                );
            }

            // マイナスのリストの要素を反転させる
            minusEndOfShadow.Reverse();
            minusEndOfShadow.Add(gameObject.transform.InverseTransformPoint(hitBSide.point));

            // プラスとマイナスを合わせる
            completionPoint.AddRange(plusEndOfShadow);
            completionPoint.AddRange(minusEndOfShadow);

            lightPolygon.points = completionPoint.ToArray();

            Debug.Log(completionPoint[completionPoint.Count - 2]);

            // 最後に今回の位置を保存
            oldPosition = lightPosition;

            // メッシュ表示
            meshGenerateScript.RunTestPolygonColliders();
            completionPoint.Clear();
        }

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
            Debug.DrawLine(lightPosition, hitASide.point, Color.cyan);
            Debug.DrawLine(lightPosition, hitBSide.point, Color.cyan);
        }
    }

    /// <summary>
    /// 配列の要素を順番に合うように並び変える
    /// </summary>
    /// <param name="pointArray"></param>
    /// <param name="referencePoint"></param>
    /// <param name="oldDistance"></param>
    private Vector2[] SortImitateShadow(Vector2[] pointArray, float referencePoint, ref float oldDistance, GameObject shadowHitObject)
    {
        // 角から基準までの距離を比較する
        float nowDistance = referencePoint - pointArray[0].x;
        if (oldDistance < nowDistance)
        {
            // 前回より今回の方が大きかったら角と影を逆転する
            Vector2 temp = pointArray[0];
            pointArray[0] = pointArray[1];
            pointArray[1] = temp;
        }

        // 今回の座標をoldに保存する
        oldDistance = nowDistance;
        // カメラフレームのコライダーに影が接触していたら前回の値をリセットする
        if (cameraFrame == shadowHitObject)
        {
            oldDistance = 0;
        }

        return pointArray;
    }


    /// <summary>
    /// 影が到達する座標を取得するためのコライダーの大きさを調整する
    /// </summary>
    private void SetReachCollider()
    {
        float[] pointsCalculation = new float[2];
        Vector2[] points = new Vector2[2];
        points[0] = new Vector2(reachColDistance, 0);
        points[1] = new Vector2(reachColDistance, 0);
        // 度数法から弧度法に変換
        pointsCalculation[0] = SpotAngle / 2 * Mathf.Deg2Rad;
        pointsCalculation[1] = -SpotAngle / 2 * Mathf.Deg2Rad;

        // Y座標を求める
        points[0].y = Mathf.Tan(pointsCalculation[0]) * (reachColDistance + 1);
        points[1].y = Mathf.Tan(pointsCalculation[1]) * (reachColDistance - 1);

        GetComponent<EdgeCollider2D>().points = points;
    }
}
