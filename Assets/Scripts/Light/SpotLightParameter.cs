using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightParameter : MonoBehaviour
{
    // 調整できるパラメータ
    [SerializeField, Range(0.0f, 180.0f)] private float m_spotAngle;        // ライトの照らす広さ
    [SerializeField, Range(0.0f, 359.9f)] private float m_spotDirection;    // ライトの向き
    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_layerMask;                         // レイヤーマスク

    // 受け渡し用
    public Vector2 forwardDirection { get;private set; }
    public Vector2 lightPosition { get; private set; }
    public Vector2 lightAngle { get; private set; }
    public RaycastHit2D upHit { get; private set; }
    public RaycastHit2D underHit { get; private set; }
    public Vector2[] hitPoint { get; private set; }

    void Start()
    {
        
    }

    void Update()
    {
        // ライトの位置
        lightPosition = transform.position;
        // ライトの向き
        forwardDirection = Quaternion.Euler(0, 0, m_spotDirection) * transform.right;
        // 光の終点
        Vector2 upDirection = Quaternion.Euler(0, 0, -m_spotAngle / 2) * forwardDirection;
        Vector2 underDirection = Quaternion.Euler(0, 0, m_spotAngle / 2) * forwardDirection;

        upHit = Physics2D.Raycast(lightPosition, upDirection, Mathf.Infinity, m_layerMask);
        underHit = Physics2D.Raycast(lightPosition, underDirection, Mathf.Infinity, m_layerMask);
        // 光の角度
        lightAngle = upDirection;

        // 光が当たった場所格納
        hitPoint = new Vector2[]{ upHit.point, underHit.point};

        if (rayVisible)
        {
            Debug.DrawRay(lightPosition, upDirection * 100);
            Debug.DrawRay(lightPosition, underDirection * 100);
        }
    }
}
