using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // 調整できるパラメータ
    [SerializeField] private bool m_isSpotLight;
    [SerializeField] private bool m_defaultLight = true;                     // ライトか他の光の要素か

    [Header("ライトの設定")]
    [SerializeField] private float m_spotAngle;        // ライトの照らす広さ
    [SerializeField] private float m_spotDirection;    // ライトの向き
    [Space]
    [Header("ライト以外の時だけの設定")]
    [SerializeField] private Vector2[] m_startPosition;
    [SerializeField] private Vector2[] m_endPosition;

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
        m_spotDirection = Mathf.Clamp(m_spotDirection, 0.0f, 359.9f);
    }

    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_defaultLayerMask;                         // レイヤーマスク

    private Vector2 oldPosition;        // 1フレーム前の位置
    private List<Vector2[]> points = new List<Vector2[]>();  // 影ができる点のリスト

    public bool defaultLight { get { return m_defaultLight; } }
    public float spotAngle { get { return m_spotAngle; } }
    public Vector2 forwardDirection { get; private set; }
    public Vector2 lightPosition { get; private set; }
    public Vector2 lightAngle { get; private set; }
    public RaycastHit2D hitASide { get; private set; }
    public RaycastHit2D hitBSide { get; private set; }
    public Vector2[] startPosition { get; private set; }
    public Vector2[] endPosition { get; private set; }


    void Update()
    {
        if (defaultLight && oldPosition != lightPosition)
        {
            // ライトの位置
            lightPosition = transform.position;
            // ライトの向き
            forwardDirection = Quaternion.Euler(0, 0, m_spotDirection) * Vector2.right;
            // 光の終点
            Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * forwardDirection;
            Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * forwardDirection;

            // ライトの終点を求める
            hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
            hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);
            // リストに保存
            points.Add(new Vector2[] { lightPosition, hitASide.point });
            points.Add(new Vector2[] { hitASide.point, lightPosition });

            // 角にあるshadowPointを取得
            GameObject[] shadowPoints = GameObject.FindGameObjectsWithTag("ShadowPoint");

            // ライトの光が届くか調べる
            foreach (GameObject shadowPoint in shadowPoints)
            {
                // shadowPointの座標
                Vector2 shadowPointPosition = shadowPoint.transform.position;
                // 比較するベクトル計算
                Vector2 standardVector =
                    (lightPosition -
                        (forwardDirection +
                        lightPosition))
                        .normalized;
                Vector2 targetVector =
                    (lightPosition - shadowPointPosition)
                    .normalized;

                // ライトまでの角度を求める
                float angle =
                    Mathf.Acos(Vector2.Dot(standardVector, targetVector)) *
                    Mathf.Rad2Deg;

                if (angle < spotAngle / 2)
                {
                    // 妨げになるオブジェクトがないか調べる
                    RaycastHit2D hit = Physics2D.Linecast(lightPosition, shadowPointPosition, m_defaultLayerMask);

                    if (!hit)
                    {
                        // 当たらなかったときそのオブジェから伸びる影の終点を求める

                    }
                }
            }
        }

        // 最後に今回の位置を保存
        oldPosition = lightPosition;
    }

    /// <summary>
    /// 現在地から近い順に並び変える
    /// </summary>
    /// <param name="origin"></param>
    private Vector2[] SortByClosest(Vector2 origin, List<Vector2> pointList)
    {
        return pointList.OrderBy(point => Vector2.Distance(point, origin)).ToArray();
    }
}
