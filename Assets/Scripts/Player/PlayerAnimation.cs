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
            // 優先度が低かったら終了する
            return;
        }

        // 優先度が高く、
        // 現在再生中のアニメーションと異なった場合
        // 該当のアニメーションを実行する
        animator.Play(animation.ToString());
        playingAnimation = animation;

    }
}
