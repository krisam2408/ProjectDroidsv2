using Droids.Behaviour;
using System.Collections;
using UnityEngine;

namespace Droids.Model.Skills.Player
{
    [CreateAssetMenu(fileName = "chainSkill.asset", menuName = "Player Skills/Chain Skill")]
    public sealed class ChainSkill : BasePlayerSkill
    {
        [SerializeField] private ChainPointBehaviour m_point;
        [SerializeField] private float m_time;
        [SerializeField] private int m_damage = 2;

        private float m_elapsedTime = 0f;

        public override int AnimationId => 0;

        public override bool IsUsable
        {
            get
            {
                bool locked = !Context.Skills.Locked;
                bool input = Context.Input.Chain.IsPressed;

                return locked && input;
            }
        }

        public override IEnumerator Execute()
        {
            m_elapsedTime = 0f;
            Context.Skills.Locked = true;
            Context.Controller.CanMove = false;
            Vector3 vector = new(Context.Input.Move.X, Context.Input.Move.Y);
            Vector3[] positions = Context.SetChainFiringSpot(vector.x, vector.y);
            ChainPointBehaviour chain = Instantiate<ChainPointBehaviour>(m_point, positions[1], Quaternion.identity);
            chain.Direction = positions[0];
            chain.TargetLayers = TargetMask;
            chain.Damage = m_damage;

            while(m_elapsedTime < m_time && Context.ChainPullVector == null)
            {
                m_elapsedTime += Time.deltaTime;
                Context.AppliedY = 0f;
                yield return null;
            }

            Destroy(chain.gameObject);
            Context.Skills.Locked = false;
            
            if(!Context.WallGripped)
                Context.Controller.CanMove = true;
        }
    }
}
