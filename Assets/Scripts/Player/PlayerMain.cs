using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Ray�̐ݒ荀�ڂ̃X�g���N�g
/// </summary>
[System.Serializable]
public struct RayInfo
{
    public Vector3 startPos;
    public Vector3 endPos;
    public LayerMask layerMask;

    public RayInfo(Vector3 startPos, Vector3 endPos, LayerMask layerMask)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        this.layerMask = layerMask;
    }
}

public class PlayerMain : MonoBehaviour
{
    private PlayerInput inputScript;
    private PlayerMovement movementScript;

    [SerializeField] private bool debugMode;
    private RayInfo touchGround;
    private RayInfo groundTilt;

    void Start()
    {
        inputScript = GetComponent<PlayerInput>();
        movementScript = GetComponent <PlayerMovement>();
    }

    void FixedUpdate()
    {
        // �v���C���[���E�ړ�
        movementScript.WalkSpeed(inputScript.moveDir);


    }

    /// <summary>
    /// �n�ʂɐڐG���Ă��邩
    /// </summary>
    /// <returns>bool</returns>
    public bool IsGround()
    {

        return true;
    }
}
