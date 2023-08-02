using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float moveInput;
    GroundState groundStateScript;

    void Start()
    {
        groundStateScript = GetComponent<GroundState>();
    }

    void Update()
    {
        // 入力チェック
        moveInput = Input.GetAxisRaw("Horizontal");

    }
}
