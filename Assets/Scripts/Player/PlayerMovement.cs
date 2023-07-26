using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed = 17;    // �v���C���[���x
    [SerializeField] private LayerMask lMask;       // ���C���[�}�X�N
    [SerializeField] private float maxSpeed;        // �v���C���[�̍ō����x
    Rigidbody2D rb2D;                               // �v���C���[��Rigidbody
    private PlayerInput inputScript;                // PlayerInput
    private Vector3 velocity;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        inputScript = GetComponent<PlayerInput>();
    }

    public void Update()
    {

        // �ړ����x��K�p
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
            // �X�΂����n�ʂɑ΂��ĕ␳�W�����v�Z
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return Mathf.Cos(Mathf.Deg2Rad * angle);
        }

        return 1f;
    }
}
