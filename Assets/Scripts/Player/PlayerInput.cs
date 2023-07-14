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

        // ˆÚ“®•ûŒü“ü—Í
        moveDir = Input.GetAxisRaw("Horizontal");

        // ƒWƒƒƒ“ƒv“ü—Í
        if (Input.GetButtonDown("Jump"))
        {
            isJump = true;
        }
    }
}
