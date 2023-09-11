using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpotLightArea : MonoBehaviour
{
    // 調整できるパラメータ
    [SerializeField] private bool m_isSpotLight;
    [SerializeField] private bool m_defaultLight = true;                     // ライトか他の光の要素か

    [Header("ライトの設定")]
    [SerializeField] private float m_spotAngle;        // ライトの照らす広さ
    [Space]
    [Header("ライト以外の時だけの設定")]
    [SerializeField] private Vector2[] m_startPosition;
    [SerializeField] private Vector2[] m_endPosition;

    private void OnValidate()
    {
        m_spotAngle = Mathf.Clamp(m_spotAngle, 0.0f, 180.0f);
    }

    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_defaultLayerMask;                         // レイヤーマスク

    private Vector2 oldPosition;        // 1フレーム前の位置

    private Vector2 lightPosition;
    private Vector2 lightAngle;
    private RaycastHit2D hitASide;
    private RaycastHit2D hitBSide;

    private void Start()
    {
        // ライトの初期位置など取得
        LightSetting();
    }

    void Update()
    {
        LightSetting();
        if (oldPosition != lightPosition)
        {
            GameObject[] objectEdges = GameObject.FindGameObjectsWithTag("ObjectEdge");
            List<Vector2> shadowPositionIndex = new List<Vector2>();
            Dictionary<Vector2, Vector2[]>
                shadowPosition =
                new Dictionary<Vector2, Vector2[]>();
            List<Vector2> allEndOfShadow = new List<Vector2>();
            List<Vector2> plusendOfShadow = new List<Vector2>();
            List<Vector2> minusEndOfShadow = new List<Vector2>();
            List<Vector2> completionPoint = new List<Vector2>();

            // 影座標の情報を取得
            foreach (GameObject objectEdge in objectEdges)
            {
                ObjectEdge objectEdgeScript = objectEdge.GetComponent<ObjectEdge>();

                // 情報をライトの相対座標として受け取る
                shadowPositionIndex.Add(
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
            // リストを降順にソートする
            shadowPositionIndex.Sort((a, b) => b.y.CompareTo(a.y));

            // 上から順番にリストに入れていく

        }

        // 最後に今回の位置を保存
        oldPosition = lightPosition;
    }

    private void LightSetting()
    {
        // ライトの位置
        lightPosition = transform.position;

        // 光の終点
        Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * transform.right;
        Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * transform.right;

        // ライトの終点を求める
        hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_defaultLayerMask);
        hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_defaultLayerMask);

        if (rayVisible)
        {
            Debug.DrawLine(lightPosition, hitASide.point, color: Color.cyan);
            Debug.DrawLine(lightPosition, hitBSide.point, color: Color.cyan);
        }
    }
}
