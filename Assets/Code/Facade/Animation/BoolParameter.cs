using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class BoolParameter : Parameter<bool>
    {
        public BoolParameter(Animator animator, string name) : base(animator, name) { }

        protected override void SetAction(Animator animator, bool value)
        {
            if (animator != null)
                animator.SetBool(Hashes[0], value);
        }
    }
}
