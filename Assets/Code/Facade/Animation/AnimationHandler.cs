using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class AnimationHandler
    {
        private readonly Animator m_animator;
        private readonly int m_hash;

        public AnimationHandler(string name, Animator animator)
        {
            m_hash = Animator.StringToHash(name);
            m_animator = animator;
        }

        public void Play() => m_animator.Play(m_hash);
    }
}
