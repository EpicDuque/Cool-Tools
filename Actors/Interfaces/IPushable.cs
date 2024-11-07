using DG.Tweening;
using UnityEngine;

namespace CoolTools.Actors
{
    /// <summary>
    /// Used for knockback, dashing etc.
    /// Your Actor should only have one IPushable instance when implemented.
    /// </summary>
    public interface IPushable
    {
        void DoPush(Vector3 push, float duration, Ease ease);
    }
}