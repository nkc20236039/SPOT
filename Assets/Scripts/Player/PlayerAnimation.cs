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
            // 現在再生中のアニメーションと異なった場合
            // 該当のアニメーションを実行する
            animator.Play(animation.ToString());
            playingAnimation = animation;
        }

    }
}
