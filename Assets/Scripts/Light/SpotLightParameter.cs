using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightParameter : MonoBehaviour
{
    // �����ł���p�����[�^
    [SerializeField, Range(0.0f, 180.0f)] private float m_spotAngle;        // ���C�g�̏Ƃ炷�L��
    [SerializeField, Range(0.0f, 359.9f)] private float m_spotDirection;    // ���C�g�̌���
    [Space]
    [SerializeField] private bool rayVisible;
    [SerializeField] private LayerMask m_layerMask;                         // ���C���[�}�X�N

    // �󂯓n���p
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
        // ���C�g�̈ʒu
        lightPosition = transform.position;
        // ���C�g�̌���
        forwardDirection = Quaternion.Euler(0, 0, m_spotDirection) * transform.right;
        // ���̏I�_
        Vector2 upDirection = Quaternion.Euler(0, 0, -m_spotAngle / 2) * forwardDirection;
        Vector2 underDirection = Quaternion.Euler(0, 0, m_spotAngle / 2) * forwardDirection;

        upHit = Physics2D.Raycast(lightPosition, upDirection, Mathf.Infinity, m_layerMask);
        underHit = Physics2D.Raycast(lightPosition, underDirection, Mathf.Infinity, m_layerMask);
        // ���̊p�x
        lightAngle = upDirection;

        // �������������ꏊ�i�[
        hitPoint = new Vector2[]{ upHit.point, underHit.point};

        if (rayVisible)
        {
            Debug.DrawRay(lightPosition, upDirection * 100);
            Debug.DrawRay(lightPosition, underDirection * 100);
        }
    }
}
