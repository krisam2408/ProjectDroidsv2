using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class FloatParameter : Parameter<float>
    {
        public FloatParameter(Animator[] animators, string boolean) : base(animators, boolean) { }

        protected override void SetAction(Animator animator, float value)
        {
            if (animator != null)
                animator.SetFloat(Hashes[0], value);
        }
    }
}
