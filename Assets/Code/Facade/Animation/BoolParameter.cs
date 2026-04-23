using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class BoolParameter : Parameter<bool>
    {
        public BoolParameter(Animator[] animators, string boolean) : base(animators, boolean) { }

        protected override void SetAction(Animator animator, bool value)
        {
            if (animator != null)
                animator.SetBool(Hashes[0], value);
        }
    }
}
