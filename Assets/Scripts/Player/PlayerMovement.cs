using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed = 17;    // プレイヤー速度
    [SerializeField] private LayerMask lMask;       // レイヤーマスク
    [SerializeField] private float maxSpeed;        // プレイヤーの最高速度
    Rigidbody2D rb2D;                               // プレイヤーのRigidbody
    private PlayerInput inputScript;                // PlayerInput
    private Vector3 velocity;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        inputScript = GetComponent<PlayerInput>();
    }

    public void Update()
    {

        // 移動速度を適用
        Vector2 forceDir =
            new Vector2(inputScript.Walk() * m_speed * GroundState() * 1000f, 0);
        rb2D.AddForce(forceDir, ForceMode2D.Impulse);


    }

    private float GroundState()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, lMask);
        Debug.DrawRay(transform.position, Vector2.down);
        if (hit.collider != null)
        {
            // 傾斜した地面に対して補正係数を計算
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return Mathf.Cos(Mathf.Deg2Rad * angle);
        }

        return 1f;
    }
}
