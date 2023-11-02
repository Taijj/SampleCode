
using UnityEngine;

public static class AnimatorAddons
{
    /// <summary>
    /// Resets an Animator to its initial state.
    /// </summary>
    public static void Reset(this Animator @this)
    {
        @this.Rebind();
    }

    /// <summary>
    /// Checks if the Animator has a Parameter with the given nameHash.
    ///
    /// Note: Getting the array of Parameters from an Animator allocates a lot of memory,
    /// so the array should be cached, if used frequently!
    /// </summary>
    public static bool HasParameter(this Animator _, int nameHash, AnimatorControllerParameter[] parameters)
    {
        foreach(AnimatorControllerParameter param in parameters)
        {
            if (param.nameHash == nameHash)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Resets all the Animator's Parameters.
    ///
    /// Note: Getting the array of Parameters from an Animator allocates a lot of memory,
    /// so the array should be cached, if used frequently!
    /// </summary>
    public static void ResetParameters(this Animator @this, AnimatorControllerParameter[] parameters)
    {
        foreach (AnimatorControllerParameter param in parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                @this.ResetTrigger(param.nameHash);

            if (param.type == AnimatorControllerParameterType.Bool)
                @this.SetBool(param.nameHash, false);

            if (param.type == AnimatorControllerParameterType.Float)
                @this.SetFloat(param.nameHash, 0f);

            if (param.type == AnimatorControllerParameterType.Int)
                @this.SetInteger(param.nameHash, 0);
        }
    }

    /// <summary>
    /// Sets the trigger with the given hash and updates the Animator afterwards.
    /// Outputs the length in seconds of the AnimationClip the trigger is transitioning to.
    /// </summary>
    public static void TriggerInstantly(this Animator @this, int hash, out float clipLength)
    {
        @this.SetTrigger(hash);
        @this.Update(Time.deltaTime);

		AnimatorClipInfo info;
        if (@this.IsInTransition(0))
            info = @this.GetNextAnimatorClipInfo(0)[0];
        else
            info = @this.GetCurrentAnimatorClipInfo(0)[0];

		clipLength = info.clip.length;
    }
}