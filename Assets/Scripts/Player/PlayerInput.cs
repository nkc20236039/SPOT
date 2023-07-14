using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float moveDir { get; private set; }
    public bool isJump { get; private set; }

    public void InputInit()
    {
        moveDir = 0;
        isJump = false;
    }

    void Update()
    {
        InputInit();

        // �ړ���������
        moveDir = Input.GetAxisRaw("Horizontal");

        // �W�����v����
        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }
    }
}
