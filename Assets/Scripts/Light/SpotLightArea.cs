using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private LayerMask m_layerMask;                         // レイヤーマスク

    private Vector2 oldPosition;        // 1フレーム前の位置

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
        if (defaultLight)
        {
            // ライトの位置
            lightPosition = transform.position;
            // ライトの向き
            forwardDirection = Quaternion.Euler(0, 0, m_spotDirection) * Vector2.right;
            // 光の終点
            Vector2 directionASide = Quaternion.Euler(0, 0, m_spotAngle / 2) * forwardDirection;
            Vector2 directionBSide = Quaternion.Euler(0, 0, -m_spotAngle / 2) * forwardDirection;

            hitASide = Physics2D.Raycast(lightPosition, directionASide, Mathf.Infinity, m_layerMask);
            hitBSide = Physics2D.Raycast(lightPosition, directionBSide, Mathf.Infinity, m_layerMask);
            // 光の角度
            lightAngle = directionASide;

            // 光が当たった場所格納
            startPosition = new Vector2[2] { lightPosition, lightPosition };
            endPosition = new Vector2[2] { hitASide.point, hitBSide.point };
            if (rayVisible)
            {
                Debug.DrawLine(lightPosition, hitASide.point);
                Debug.DrawLine(lightPosition, hitBSide.point);
            }
        }
        else
        {
            startPosition = m_startPosition;
            endPosition = m_endPosition;
        }

        if(oldPosition != lightPosition)    // 位置が違ったら実行する
        {

        }

        // 最後に今回の位置を保存
        oldPosition = lightPosition;
    }

}
