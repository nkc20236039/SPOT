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

    private void PlayAnimation(animationType animation)
    {
        if (animation != playingAnimation)
        {
            // ���ݍĐ����̃A�j���[�V�����ƈقȂ����ꍇ
            // �Y���̃A�j���[�V���������s����
            animator.Play(animation.ToString());
            playingAnimation = animation;
        }

    }
}
