using UnityEngine;

namespace Droids.Facade.Animation
{
    public sealed class AnimationHandler
    {
        private readonly Animator m_animator;
        private readonly int m_hash;
        private readonly int m_layer;

        public AnimationHandler(string name, Animator animator, int layer = 0)
        {
            m_hash = Animator.StringToHash(name);
            m_animator = animator;
            m_layer = layer;
        }

        public void Play() => m_animator.Play(m_hash);
    }
}
