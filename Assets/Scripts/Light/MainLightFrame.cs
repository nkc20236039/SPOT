/*using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class MainLightFrame : MonoBehaviour
{
    [SerializeField] private bool debug;

    private SpotLightArea enableLightScript;   // パラメータースクリプト
    public ReactiveProperty<GameObject> enableLight =// プレイヤーが動かす有効のライト
        new ReactiveProperty<GameObject>();
    private List<GameObject> exclusionLight =       // 交点を求める時除外するオブジェクト
        new List<GameObject>();

    private GameObject[] spotLights;                // フィールド内にある全てのライト
    private List<GameObject> hittingLight           // 光を当ててるライト
        = new List<GameObject>();
    private List<Vector2> points                    // 順番に影の角になる箇所の座標を記録する
        = new List<Vector2>();
    private Vector2 scheduleLineEnd;               // 次の終了地点になるかもしれない値

    void Start()
    {
        // 全てのライト取得
        spotLights = GameObject.FindGameObjectsWithTag("Light");
        enableLight.Subscribe(value =>
            enableLightScript =
                enableLight.Value.GetComponent<SpotLightArea>());
    }

    void Update()
    {
        enableLightScript = enableLight.Value.GetComponent<SpotLightArea>();

        // ライトに当たっている場合そのライトを取得する
        IsLightHit(enableLight.Value.transform.position, true, true);

        外周の交点を記録する
       // 初期値
       Vector2 lineStart = Vector2.positiveInfinity;                // 交点座標
        Vector2 lineEnd = enableLightScript.hitASide.point;
        Vector2 startPosition = enableLight.Value.transform.position;
        exclusionLight.Clear();
        points.Clear();
        // 開始位置と交点座標が同じになるまで繰り返す
        int safety = 0;
        //while (!Mathf.Approximately(lineStart.magnitude, startPosition.magnitude))
        for (int i = 0; i < 4; i++)
        {
            if (hittingLight.Count == 0)
            {
                // 現在の位置が他のライトの範囲外のとき
                ---1フレーム一回限りの実行-- -
                if (lineStart.magnitude >= Vector2.positiveInfinity.magnitude - 1)
                {
                    // currentLocationが初期値の場合有効ライトの座標に設定
                    lineStart = enableLight.Value.transform.position;
                    exclusionLight.Add(enableLight.Value);
                    points.Add(lineStart);
                }
                -----------------------
                // 近くの座標を取得
                var latestIntersect =
                    FindNearestIntersection(lineStart, lineEnd, exclusionLight.ToArray());


                SpotLightArea lightScript = latestIntersect.intersectObject.GetComponent<SpotLightArea>();
                if (!lightScript.defaultLight)          // 画面の端についたとき
                {

                }
                else if (Mathf.Approximately(latestIntersect.nextIntersection.magnitude, Vector2.positiveInfinity.magnitude))               // ライトの座標とほぼ同じだった時
                {
                    // スタートと終了を入れ替え
                    lineStart = lineEnd;
                    lineEnd = scheduleLineEnd;
                    // リストに2つ以上入っていたら最初を削除
                    if (exclusionLight.Count > 1)
                    {
                        exclusionLight.RemoveAt(0);
                    }
                }
                else                                    // ライトの光の途中のとき
                {
                    // スタート地点を更新
                    lineStart = latestIntersect.nextIntersection;
                    // 終了地点を求める
                    if (IsLightHit(lineStart + latestIntersect.normal * 0.1f))
                    {
                        lineEnd = GetLineEndPoint(
                            lineStart,
                            -latestIntersect.normal,
                            latestIntersect.intersectObject
                            );
                    }
                    else if (IsLightHit(lineStart - latestIntersect.normal * 0.1f))
                    {
                        lineEnd = GetLineEndPoint(
                            lineStart,
                            latestIntersect.normal,
                            latestIntersect.intersectObject
                            );
                    }
                    // 現時点で除外したいオブジェクトを指定
                    exclusionLight.Add(latestIntersect.intersectObject);
                    exclusionLight.RemoveAt(0);
                }

                // 今回の座標を記録
                points.Add(lineStart);
                Debug.Log($"{i} : {lineStart}");
                if (debug) { Debug.DrawLine(lineStart, lineEnd, color: Color.green); }


            }
            else
            {
                // ライトの中なら


                // 今回の位置をとりあえず記録しておく
                //points.Insert(0, lineStart);
            }



            // 非常用措置
            safety++;
            if (safety >= 100) break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(points.Count);
            foreach (var item in points)
            {
                Debug.Log(item);
            }
        }

        // 処理が終了したらオブジェクトを無効化する
        // gameObject.SetActive(false);
    }

    private Vector2 GetLineEndPoint(Vector2 startPoint, Vector2 angle, GameObject targetObject)
    {
        SpotLightArea lightScript = targetObject.GetComponent<SpotLightArea>();
        // 終了地点(めっちゃ遠くに設定しておく)
        Vector2 endPoint = startPoint + angle * 10000;
        // 初期化
        Vector2 pointToStart;
        Vector2 pointToEnd;
        float crossProduct;
        Vector2 lineEnd = endPoint;

        // ライトの位置が直線上にあるか
        if (lightScript.defaultLight)
        {
            // スタートとエンドまでのベクトルを求める
            pointToStart = startPoint - lightScript.lightPosition;
            pointToEnd = endPoint - lightScript.lightPosition;
            // 外積計算
            crossProduct = Vector3.Cross(pointToStart, pointToEnd).z;
            // 外積がだいたい0なら処理を終了
            if (crossProduct < 0.005)
            {
                lineEnd = lightScript.lightPosition;
            }
            else
            {
                scheduleLineEnd = lightScript.lightPosition;
            }
        }
        // ライトの終了地点が直線上にあるか
        foreach (Vector2 endPosition in lightScript.endPosition)
        {
            // スタートとエンドまでのベクトルを求める
            pointToStart = startPoint - endPosition;
            pointToEnd = endPoint - endPosition;
            // 外積計算
            crossProduct = Vector3.Cross(pointToStart, pointToEnd).z;
            // 外積がだいたい0なら処理を終了
            if (crossProduct < 0.005)
            {
                lineEnd = endPosition;
            }
            else
            {
                scheduleLineEnd = endPosition;
            }
        }
        return lineEnd;
    }

    /// <summary>
    /// 指定の座標がライトの光に当たっているか調べる
    /// </summary>
    /// <param name="targetPosition">この位置がライトの光に当たっているか調べられる</param>
    /// <param name="skipEnableLight">有効中のライトの光も調べるか</param>
    /// <returns>ライトの光を受けていたらTRUE、受けていなければFALSE</returns>
    private bool IsLightHit(Vector2 targetPosition, bool skipEnableLight = false, bool addHitLightToLight = false)
    {
        hittingLight.Clear();
        bool isHit = false;
        foreach (GameObject light in spotLights)
        {
            // スクリプト取得
            SpotLightArea lightScript = light.GetComponent<SpotLightArea>();

            if (skipEnableLight && light == enableLight.Value)
            {
                // 有効中のライトをチェックしない場合飛ばす
                continue;
            }
            if (lightScript.defaultLight)
            {
                // 比較するベクトル計算
                Vector2 standardVector =
                    (lightScript.lightPosition -
                        (lightScript.forwardDirection +
                        lightScript.lightPosition))
                        .normalized;
                Vector2 targetVector =
                    (lightScript.lightPosition - targetPosition)
                    .normalized;

                // ライトまでの角度を求める
                float angle =
                    Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                    Mathf.Rad2Deg;

                if (angle < lightScript.spotAngle / 2)
                {
                    isHit = true;
                    if (addHitLightToLight)
                    {
                        // ライトの中なら現在のライトをリストに保存
                        hittingLight.Add(light);
                    }

                }
                if (debug)
                {
                    Debug.DrawRay(lightScript.lightPosition, lightScript.forwardDirection * 100, color: Color.black);
                    Debug.DrawLine(lightScript.lightPosition, targetPosition, color: Color.gray);
                }
            }
        }
        if (debug)
        {
            Debug.Log($"{hittingLight.Count} in hittingLight");
        }
        return isHit;
    }

    /// <summary>
    /// 2辺が交差しているか
    /// </summary>
    /// <param name="pointA">1つ目の辺の始点</param>
    /// <param name="pointAVector">1つ目の辺のベクトル</param>
    /// <param name="pointB">2つ目の辺の始点</param>
    /// <param name="pointBVector">2つ目の辺のベクトル</param>
    /// <returns>交差していればTRUE、していなければFALSE</returns>
    private bool IsIntersect(Vector2 pointA, Vector2 pointAVector, Vector2 pointB, Vector2 pointBVector, out float t, out float s)
    {
        float cross = pointAVector.x * pointBVector.y - pointAVector.y * pointBVector.x;
        // 交差していないか
        if (cross == 0)
        {
            t = 0;
            s = 0;
            return false;
        }

        Vector2 diff = pointB - pointA;
        t = (diff.x * pointBVector.y - diff.y * pointBVector.x) / cross;
        s = (diff.x * pointAVector.y - diff.y * pointAVector.x) / cross;

        return (t >= 0 && t <= 1 && s >= 0 && s <= 1);
    }

    /// <summary>
    /// スタート地点から一番近い交差点を求める
    /// </summary>
    /// <param name="lineStart">測りたい線の始点</param>
    /// <param name="lineEnd">測りたい線の終点</param>
    /// <param name="exclusions">検索を除外するライトオブジェクト</param>
    /// <returns>交差点、交差点の法線ベクトル、交差した相手オブジェクト</returns>
    private (Vector2 nextIntersection, Vector2 normal, GameObject intersectObject) FindNearestIntersection(Vector2 lineStart, Vector2 lineEnd, GameObject[] exclusions)
    {
        Vector2 intersection;      // すべての交点座標
        Vector2 normal;            // すべての法線ベクトル
        Vector2 nearestIntersection = Vector2.positiveInfinity;             // 一番近い交点座標
        Vector2 nearestNormal = Vector2.zero;                   // 一番近い場所の法線ベクトル
        GameObject nearestGameObject = enableLight.Value;                           // 一番近い交点の元のゲームオブジェクト


        // 受け取った線に交わる座標をすべて求める
        foreach (GameObject light in spotLights)
        {
            if (!exclusions.Contains(light))
            {
                // 相手のスクリプト取得
                SpotLightArea lightScript =
                    light.GetComponent<SpotLightArea>();
                // 交差点チェック
                for (int i = 0; i < lightScript.endPosition.Length; i++)
                {
                    // 今の進行方向のベクトル
                    Vector2 lineSegmentAVector = lineEnd - lineStart;
                    // 比較したい方のベクトル
                    Vector2 lineSegmentBVector =
                        lightScript.endPosition[i] - lightScript.startPosition[i];

                    // 2つの辺が交わったかテストする
                    float t, s;
                    bool isIntersection = IsIntersect(
                        lineStart,
                        lineSegmentAVector,
                        lightScript.startPosition[i],
                        lineSegmentBVector,
                        out t,
                        out s
                        );

                    // 2つの辺が交わったとき
                    if (isIntersection)
                    {
                        // 座標を求める
                        intersection = lineStart + t * lineSegmentAVector;
                        normal = lineSegmentBVector.normalized;

                        // 一番近い座標と比較して近い方を保存
                        Vector2 closerIntersection = nearestIntersection - lineStart;   // 現在一番近い
                        Vector2 comparisonIntersection = intersection - lineStart;  // 次の比較対象

                        if (closerIntersection.magnitude > comparisonIntersection.magnitude)
                        {
                            // 一番近い
                            nearestIntersection = intersection;
                            nearestNormal = normal;
                            nearestGameObject = light;
                        }
                    }
                }
            }
        }

        return (nearestIntersection, nearestNormal, nearestGameObject);
    }
}*/