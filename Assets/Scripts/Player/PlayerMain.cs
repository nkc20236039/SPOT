using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    private PlayerInput inputScript;
    private PlayerMovement movementScript;

    void Start()
    {
        inputScript = GetComponent<PlayerInput>();
        movementScript = GetComponent <PlayerMovement>();
    }

    void Update()
    {
        // プレイヤー左右移動
        movementScript.WalkSpeed(inputScript.moveDir);
    }
}
