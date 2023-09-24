using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;



public partial class Player
{
    private enum animationType
    {
        Idle,
        Run,
        Jump,
        JumpTurn,
        Fall,
    }

    private animationType playingAnimation;
    private int nowPriority;

    private void PlayAnimation(animationType animation, int priority = 0)
    {
        if (priority < nowPriority || animation == playingAnimation || priority != -1)
        {
            // �D��x���Ⴉ������I������
            return;
        }

        // �D��x�������A
        // ���ݍĐ����̃A�j���[�V�����ƈقȂ����ꍇ
        // �Y���̃A�j���[�V���������s����
        animator.Play(animation.ToString());
        playingAnimation = animation;

    }
}
