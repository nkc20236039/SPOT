using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class MainLightFrame : MonoBehaviour
{
    private GameObject enableLight;                 // プレイヤーが動かす有効のライト
    private GameObject[] spotLights;                // フィールド内にある全てのライト
    private List<GameObject> hittingLight           // 光を当ててるライト
        = new List<GameObject>();
    private SpotLightParameter enableLightScript;   // パラメータースクリプト
    private List<Vector2> points                    // 
        = new List<Vector2>();


    void Start()
    {
        // 全てのライト取得
        spotLights = GameObject.FindGameObjectsWithTag("Light");

    }

    void Update()
    {
        // 有効中のライト取得
        enableLight = GameObject.FindWithTag("EnableLight");
        enableLightScript = enableLight.GetComponent<SpotLightParameter>();

        // 光の中にいるかテストする
        foreach (GameObject light in spotLights)
        {
            SpotLightParameter lightScript =
                light.GetComponent<SpotLightParameter>();

            if (IsLightHit(lightScript.forwardDirection, lightScript.lightPosition, lightScript.lightAngle, enableLight.transform.position))
            {
                hittingLight.Add(light);
            }
        }

        /* 外周の交点を記録する */
        // 初期値
        Vector2 lineStart = Vector2.positiveInfinity;                // 交点座標
        Vector2 lineEnd = enableLightScript.upHit.point;
        Vector2 startPosition = enableLight.transform.position;
        // 開始位置と交点座標が同じになるまで繰り返す
        while (!Mathf.Approximately(lineStart.magnitude, startPosition.magnitude))
        {
            if (hittingLight.Count == 0)
            {
                // 現在の位置が他のライトの範囲外のとき
                // currentLocationが初期値の場合有効ライトの座標に設定
                if (lineStart == Vector2.positiveInfinity)
                {
                    lineStart = enableLight.transform.position;
                }
                // 今回の座標を記録
                points.Add(lineStart);

                // 次の交点座標を求める
                // 近くの座標を取得
                var intersection = FindNearestIntersection(lineStart, lineEnd);
                
            }
            else
            {
                // ライトの中なら

            }
        }

    }

    /// <summary>
    /// 指定の座標がライトの光に当たっているか調べる
    /// </summary>
    /// <param name="source">角度の比較元</param>
    /// <param name="lightPosition">このライトの光が当たっているか調べられる</param>
    /// <param name="lightEndPosition">このライトの光がどこまで伸びているか</param>
    /// <param name="targetPosition">この位置がライトの光に当たっているか調べられる</param>
    /// <returns>ライトの光を受けていたらTRUE、受けていなければFALSE</returns>
    private bool IsLightHit(Vector2 source, Vector2 lightPosition, Vector2 lightEndPosition, Vector2 targetPosition)
    {
        Vector2 lightVector = lightEndPosition - lightPosition;
        Vector2 targetVector = targetPosition - lightPosition;

        float lightAngle = Vector2.Angle(source, lightVector);
        float targetAngle = Vector2.Angle(source, targetVector);

        return targetAngle < lightAngle;
    }

    /// <summary>
    /// 交点座標の計算をする
    /// </summary>
    /// <param name="pointA1">1つ目の辺の最初</param>
    /// <param name="pointA2">1つ目の辺の最後</param>
    /// <param name="pointB1">2つ目の辺の最初</param>
    /// <param name="pointB2">2つ目の辺の最後</param>
    private (bool isIntersection, Vector2 intersection, Vector2 normal) Intersection(Vector2 pointA1, Vector2 pointA2, Vector2 pointB1, Vector2 pointB2)
    {
        // 交点座標初期値
        Vector2 intersection = Vector2.zero;

        // 今の進行方向のベクトル
        Vector2 pointAVector = pointA2 - pointA1;
        // 比較したい方のベクトル
        Vector2 pointBVector = pointB2 - pointB1;

        // 交点座標を求める
        float t, s;
        bool isIntersection =
            IsIntersect(pointA1, pointAVector, pointB1, pointBVector, out t, out s);
        if (isIntersection)
        {
            intersection = pointA1 + t * pointAVector;
        }

        Vector2 normal = new Vector2(-pointAVector.y, pointAVector.x).normalized + new Vector2(-pointBVector.y, pointBVector.x).normalized;
        return (isIntersection, intersection, normal);
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

    private (Vector2 nextIntersection, Vector2 normal) FindNearestIntersection(Vector2 lineStart, Vector2 lineEnd)
    {
        List<Vector2> intersections = new List<Vector2>();    // すべての交点座標
        List<Vector2> normals = new List<Vector2>();          // すべての法線ベクトル
        Vector2 nearestIntersection;
        Vector2 nearestNormal;

        // 受け取った線に交わる座標をすべて求める
        foreach (GameObject light in spotLights)
        {
            // 相手のスクリプト取得
            SpotLightParameter lightScript =
                light.GetComponent<SpotLightParameter>();
            // 交差点チェック
            for (int i = 0; i < lightScript.hitPoint.Length; i++)
            {
                var intersectionInfo = Intersection(
                    lineStart,
                    lineEnd,
                    lightScript.lightPosition,
                    lightScript.hitPoint[i]
                    );

                if (intersectionInfo.isIntersection)    // 交点が見つかった時に座標を保存する
                {
                    intersections.Add(intersectionInfo.intersection);
                    normals.Add(intersectionInfo.normal);
                }
            }
        }

        // 一番近い座標を求める
        nearestIntersection = intersections[0];
        nearestNormal = normals[0];
        for (int i = 0; i < intersections.Count; i++)
        {
            Vector2 closerIntersection = nearestIntersection - lineStart;
            Vector2 comparisonIntersection = intersections[i] - lineStart;

            if (closerIntersection.magnitude < comparisonIntersection.magnitude)
            {
                nearestIntersection = intersections[i];
            }
        }

        return (nearestIntersection, nearestNormal);
    }
}
