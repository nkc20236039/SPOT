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
    /// �ړ�����L�[�����̎擾
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
