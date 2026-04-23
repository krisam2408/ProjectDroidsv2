using System.Linq;
using UnityEngine;

namespace Droids.Facade.Animation
{
    public abstract class Parameter<T>
    {
        protected readonly Animator[] m_animators;

        private readonly int[] m_hashes;
        protected int[] Hashes => m_hashes;

        private T m_value;
        public T Value 
        {
            get => m_value;
            set 
            {
                m_value = value;
                foreach (Animator anim in m_animators)
                    SetAction(anim, value);
            }
        }

        protected Parameter(Animator[] animators, params string[] parameters)
        {
            m_animators = animators;

            m_hashes = parameters
                .Select(p => Animator.StringToHash(p))
                .ToArray();
        }

        protected abstract void SetAction(Animator animator, T value);
    }
}
