using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;
using static UnityEditor.Experimental.GraphView.GraphView;

public static class AnimHelper
{
    public static bool TryPlay(this Animator animator, string animName, float time)
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName(animName))
        {
            // 이미 해당 애니메이션이 실행 중
            return false;
        }

        if (HasAnimation(animator, animName))
        {
            animator.CrossFade(animName, time);
            return true;
        }

        Debug.LogWarning($"Animator에 '{animName}' 애니메이션이 없습니다.");
        return false;
    }

    // Animator Controller에 해당 이름의 애니메이션이 존재하는지 검사
    private static bool HasAnimation(Animator animator, string animName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
            {
                return true;
            }
        }
        return false;
    }
}
