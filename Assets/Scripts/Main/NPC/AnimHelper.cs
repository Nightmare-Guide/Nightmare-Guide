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
            // �̹� �ش� �ִϸ��̼��� ���� ��
            return false;
        }

        if (HasAnimation(animator, animName))
        {
            animator.CrossFade(animName, time);
            return true;
        }

        Debug.LogWarning($"Animator�� '{animName}' �ִϸ��̼��� �����ϴ�.");
        return false;
    }

    // Animator Controller�� �ش� �̸��� �ִϸ��̼��� �����ϴ��� �˻�
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
