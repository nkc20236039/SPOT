using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float moveDir;


    public enum getButton
    {
        Jump,

    }

    /// <summary>
    /// ˆÚ“®‚·‚éƒL[•ûŒü‚Ìæ“¾
    /// </summary>
    /// <returns></returns>
    public float Walk()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public bool IsPressingButton(getButton inputKey)
    {
        return Input.GetButton(inputKey.ToString());
    }
}
