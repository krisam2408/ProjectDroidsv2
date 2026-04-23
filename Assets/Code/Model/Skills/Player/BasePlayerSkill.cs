using Droids.Behaviour;
using Droids.Code.Extension;
using Droids.Model.DataTransfer;
using System.Collections;
using UnityEngine;

namespace Droids.Model.Skills.Player
{
    public abstract class BasePlayerSkill : ScriptableObject
    {
        [SerializeField] private Vector2 m_areaExtents;
        [SerializeField] private Vector2 m_areaOffset;
        [SerializeField] private LayerMask m_targetMask;

        public Square EffectArea(Vector3 position, bool flipped)
        {
            if(!flipped)
                return new(position.ToVector2() + m_areaOffset, m_areaExtents);
            return new(position.ToVector2() + new Vector2(-m_areaOffset.x, m_areaOffset.y), m_areaExtents);
        }

        protected PlayerBehaviour Context { get; private set; }
        protected LayerMask TargetMask => m_targetMask;

        public abstract int AnimationId { get; }
        public abstract bool IsUsable { get; }

        public void Initialize(PlayerBehaviour context) => Context = context;

        public abstract IEnumerator Execute();
        
    }
}
