using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class IntParameter : Parameter<int>
    {
        public IntParameter(Animator animator, string name) : base(animator, name) { }

        protected override void SetAction(Animator animator, int value)
        {
            if (animator != null)
                animator.SetInteger(Hashes[0], value);
        }
    }
}
