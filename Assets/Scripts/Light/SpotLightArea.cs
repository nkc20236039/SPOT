using System.Collections.Generic;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // 調整できるパラメータ
    [SerializeField] private bool m_defaultLight = true;    // ライトか他の光の要素か

    [Header("ライトの設定")]
    [SerializeField] private MeshRenderer shadowRenderer;
    [SerializeField] private float gravityScale;
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
    [SerializeField] private PolygonCollider2D lightAreaCollider;
    [SerializeField] private float reachColDistance;
    [SerializeField] private Player playerScript;
    public Texture2D shadowTexture;

    private Vector2 oldPosition;        // 1フレーム前の位置

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;
    private PolygonCollider2D lightPolygon;
    private DelaunayTriangulationTester meshGenerateScript;
    private BoxCollider2D boxCollider;
    [SerializeField] private GroundState groundStateScript;

    private void Start()
    {
        GameObject lightCollider = transform.Find("LightCollider").gameObject;
        lightPolygon = lightCollider.GetComponent<PolygonCollider2D>();
        // ライトの初期位置など取得
        LightSetting();
        meshGenerateScript = GetComponent<DelaunayTriangulationTester>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void LateUpdate()
    {
        // コライダーの広さを設定する
        GetComponent<EdgeCollider2D>().points =
            SetReachCollider();
        LightSetting();


        GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
        List<Vector2> arrivalPoints = new List<Vector2>();      // 影の最終的な到達点
        Dictionary<Vector2, Vector2[]>
            shadowPosition =
            new Dictionary<Vector2, Vector2[]>();
        Dictionary<Vector2, GameObject>
            shadowObject =
            new Dictionary<Vector2, GameObject>();
        List<Vector2> plusArrivalPoint = new List<Vector2>();    // プラス方向の頂点
        List<Vector2> minusArrivalPoint = new List<Vector2>();   // マイナス方向の頂点
        List<Vector2> completionPoint = new List<Vector2>();    // 完成した頂点群


        // 影座標の情報を取得
        foreach (GameObject objectEdge in objectEdges)
        {
            ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

            // 角がライトの光に入っているか確かめる
            if (objectEdgeScript.IsExposedToLight(lightPosition))
            {
                // 情報をライトの相対座標として受け取る
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

        arrivalPoints.Sort((point1, point2) =>
        {
            // Y座標がほぼ同じ場合にX軸で昇順にソート
            if (Mathf.Approximately(point1.y, point2.y))
            {
                return point1.x.CompareTo(point2.x);
            }
            // Y座標が異なる場合にY座標で降順にソート
            else
            {
                return point2.y.CompareTo(point1.y);
            }
        });

        // ライトの最初と最後をとりあえず入れる
        completionPoint.Add(gameObject.transform.InverseTransformPoint(lightPosition));
        completionPoint.Add(gameObject.transform.InverseTransformPoint(hitASide.point));

        // すべての到達点をプラスとマイナスに分ける
        for (int i = 0; i < arrivalPoints.Count; i++)
        {
            if (arrivalPoints[i].y >= 0 && shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                // プラスの地点を保存
                plusArrivalPoint.Add(arrivalPoints[i]);
            }
            else if (shadowPosition.ContainsKey(arrivalPoints[i]))
            {
                // マイナスの地点を保存
                minusArrivalPoint.Add(arrivalPoints[i]);
            }
        }

        float[] oldPointDistance = { 0f, 0f };
        // プラスの座標情報を完成頂点リストに並び変える
        if (plusArrivalPoint.Count > 0)
        {
            if (hitASide.transform.gameObject != cameraFrame)
            {
                oldPointDistance[0] = gameObject.transform.InverseTransformPoint(hitASide.point).x - reachColDistance;
            }

            for (int i = 0; i < plusArrivalPoint.Count; i++)
            {
                completionPoint.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[plusArrivalPoint[i]],
                        reachColDistance,
                        ref oldPointDistance[0],
                        shadowObject[plusArrivalPoint[i]]
                    )
                );
            }
        }

        // マイナスの座標情報を完成頂点リストに並び変える
        if (minusArrivalPoint.Count > 0)
        {
            if (hitBSide.transform.gameObject != cameraFrame)
            {
                oldPointDistance[1] = gameObject.transform.InverseTransformPoint(hitBSide.point).x - reachColDistance;
            }
            minusArrivalPoint.Reverse();
            List<Vector2> arrivalPointStorage = new List<Vector2>();
            for (int i = 0; i < minusArrivalPoint.Count; i++)
            {
                // 後ろから入れていく
                arrivalPointStorage.AddRange
                (
                    SortImitateShadow
                    (
                        shadowPosition[minusArrivalPoint[i]],
                        reachColDistance,
                        ref oldPointDistance[1],
                        shadowObject[minusArrivalPoint[i]]
                    )
                );
            }
            arrivalPointStorage.Reverse();
            completionPoint.AddRange(arrivalPointStorage);
        }
        completionPoint.Add(gameObject.transform.InverseTransformPoint(hitBSide.point));

        // 重なっている座標を削除
        Vector2[] checkConflict = completionPoint.ToArray();
        int quantityRemoved = 0;
        Vector2 oldPoint = completionPoint[completionPoint.Count - 1];
        for (int i = 0; i < checkConflict.Length; i++)
        {
            if (checkConflict[i] == oldPoint)
            {
                // リストの大きさに合わせて削除する
                completionPoint.RemoveAt(i - quantityRemoved);
                quantityRemoved++;
            }
            oldPoint = checkConflict[i];
        }


        // ポリゴンコライダーに反映する
        lightPolygon.points = completionPoint.ToArray();

        // メッシュ表示
        meshGenerateScript.RunTestPolygonColliders();

        // シェーダーをロードしてMaterialを生成
        Material material = new Material(Shader.Find("Unlit/SimpleTexture"));
        material.SetTexture("_MainTex", shadowTexture);

        // MeshRendererにMaterialをセット
        shadowRenderer.material = material;

        // 最後に今回の位置を保存
        oldPosition = lightPosition;
        completionPoint.Clear();
    }

    /// <summary>
    /// ライトの設定、座標の取得
    /// </summary>
    private void LightSetting()
    {
        // 重力
        if (!groundStateScript.IsGround() && (!playerScript.haveLight || playerScript.isWall) && m_defaultLight)
        {
            Vector3 position = transform.position;
            position.y -= gravityScale;
            transform.position = position;
        }

        // ライトの位置
        lightPosition = transform.position;

        // 光の終点
        m_spotAngle = Mathf.Abs(m_spotAngle);
        m_spotAngle *= playerScript.lightDirection;
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * (transform.right * playerScript.lightDirection);
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * (transform.right * playerScript.lightDirection);

        // ライトの終点を取得
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);

        // ライトのチェックコライダー設定
        lightAreaCollider.points = new Vector2[]
        {
            SetReachCollider()[0],
            SetReachCollider()[1],
            Vector2.zero
        };


        if (debug)
        {
            Debug.DrawLine(lightPosition, hitASide.point, Color.yellow);
            Debug.DrawLine(lightPosition, hitBSide.point, Color.yellow);
        }
    }

    /// <summary>
    /// 配列の要素を順番に合うように並び変える
    /// </summary>
    /// <param name="pointArray">オブジェクトの角と影の終点を入れている配列</param>
    /// <param name="referencePoint">比較する地点</param>
    /// <param name="oldDistance">前回の距離</param>
    private Vector2[] SortImitateShadow(Vector2[] pointArray, float referencePoint, ref float oldDistance, GameObject shadowHitObject)
    {
        // 角から基準までの距離を比較する
        float nowDistance = pointArray[0].x - referencePoint;
        if (oldDistance > nowDistance)
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
    private Vector2[] SetReachCollider()
    {
        float[] pointsCalculation = new float[2];
        Vector2[] points = new Vector2[2];
        points[0] = new Vector2(reachColDistance, 0);
        points[1] = new Vector2(reachColDistance, 0);
        // 度数法から弧度法に変換
        pointsCalculation[0] = SpotAngle / 2 * Mathf.Deg2Rad;
        pointsCalculation[1] = -SpotAngle / 2 * Mathf.Deg2Rad;

        // Y座標を求める
        points[0].y = Mathf.Tan(pointsCalculation[0]) * reachColDistance;
        points[1].y = Mathf.Tan(pointsCalculation[1]) * reachColDistance;

        return points;
    }

    /*private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < lightPolygon.points.Length; i++)
            {
                Handles.Label(lightPolygon.points[i], $"[{i}]");
            }
        }

    }*/
}
