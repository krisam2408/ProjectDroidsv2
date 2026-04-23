using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class TriggerParameter : Parameter<bool>
    {
        public TriggerParameter(Animator[] animators, string boolean) : base(animators, boolean) { }

        protected override void SetAction(Animator animator, bool value)
        {
            if (value)
            {
                animator.SetTrigger(Hashes[0]);
                return;
            }

            animator.ResetTrigger(Hashes[0]);
        }

        public void Trigger() => Value = true;
        public void Reset() => Value = false;
    }
}
