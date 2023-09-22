using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public partial class Player
{
    private bool isRunning;     // �n�ʂɂ���Ƃ�
    private bool isJumping;     // �W�����v���Ă���
    private bool isJumpTurn;    // �~�܂��ăW�����v
    private bool isIdle;        // �n�ʂŎ~�܂��Ă���
    private bool isFalling;     // �󒆂ŗ���

    private void SetAnimation()
    {
        if (groundStateScript.IsGround())
        {
            // �����Ă���Ƃ�
            animator.SetBool("Run", isRunning && !isJumping);
            // �A�j���[�^�[�ŃA�C�h�����͎��s�����
            // animator.SetBool("Idle", isIdle);
            // �W�����v
            animator.SetBool("Jump", isJumping);
        }
        else
        {
            // �󒆂ŃW�����v��]
            animator.SetBool("JumpTurn", isJumpTurn);
            // �󒆂ŃW�����v��Ԃ���Ȃ�
            animator.SetBool("Fall", isFalling);
        }
    }
}
