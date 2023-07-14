using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_speed = 17;
    Rigidbody2D rb2D;
    Vector2 vel;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ˆÚ“®‘¬“x‚ð“K—p
        rb2D.velocity = vel;
    }


    public void WarkSpeed(float dir)
    {
        vel = rb2D.velocity;
        vel.x = m_speed * dir;

    }
}
