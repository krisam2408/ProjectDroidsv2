using Droids.Code.Extension;
using Droids.Model;
using Droids.Model.DataTransfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Droids.Component
{
    public class CharacterControllerComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CapsuleCollider2D m_collider;

        [Header("Detection")]
        [SerializeField] private LayerMask m_groundDetection;
        [SerializeField] private LayerMask m_passthroughDetection;
        [SerializeField] private float m_hangAltitude;

        [Header("Collider Parameters")]
        [SerializeField] private Vector2 m_rect;
        [SerializeField] private Vector2 m_origin;

        [Header("Horizontal")]
        [SerializeField] private float m_sideWidth;
        [SerializeField] private float m_bottomHeight;
        [SerializeField] private float m_verticalPadding;
        [SerializeField, Range(1f, 3f)] private float m_topWidthFactor;
        [SerializeField] private bool m_xFlipped;

        [Header("Bottom")]
        [SerializeField] private Vector2 m_bottomExtents;
        [SerializeField] private Vector2 m_bottomOffset;
        [SerializeField] private float m_bottomFixFactor = 10f;

        public bool CanMove { get; set; } = true;
        public bool XFlipped 
        {
            get => m_xFlipped;
            set => m_xFlipped = value; 
        }

        private CollisionChecker m_collision = CollisionChecker.None;
        private CollisionChecker m_collisionToggle = CollisionChecker.Bottom | CollisionChecker.Left | CollisionChecker.Right | CollisionChecker.Top;
        private CollisionChecker m_passThrough = CollisionChecker.None;

        public CollisionChecker Collisions => m_collision;
        public CollisionChecker PassThrough => m_passThrough;

        private bool CanWallGripLeft { get; set; }
        private bool CanWallGripRight { get; set; }
        public bool CanWallGrip => (CanWallGripLeft && XFlipped) || (CanWallGripRight && !XFlipped);

        public bool CanHangLeft { get; private set; }
        public bool CanHangRight { get; private set; }

        public void DisableCollisionToggle(params CollisionChecker[] sides)
        {
            foreach (CollisionChecker side in sides)
                m_collisionToggle &= ~side;
        }

        private WaitForSeconds m_enableToggleWait = new(0.3f);

        #region Boxes
        private Square BottomBox => new(transform.position.ToVector2() + m_bottomOffset, m_bottomExtents);

        private Square TopLeftBox => new(
            transform.position.ToVector2() + new Vector2(
                -(m_rect.x * 0.5f + m_sideWidth * 2f * m_topWidthFactor),
                m_origin.y + m_bottomHeight + ((m_rect.y * 0.5f) - m_bottomHeight - m_verticalPadding) * 0.5f),
            new Vector2(
                m_sideWidth * 2f * m_topWidthFactor,
                ((m_rect.y * 0.5f) - m_bottomHeight - m_verticalPadding) * 0.5f
            ));

        private Square TopRightBox => new(
            transform.position.ToVector2() + new Vector2(
                m_rect.x * 0.5f + m_sideWidth * 2f * m_topWidthFactor,
                m_origin.y + m_bottomHeight + ((m_rect.y * 0.5f) - m_bottomHeight - m_verticalPadding) * 0.5f),
            new Vector2(
                m_sideWidth * 2f * m_topWidthFactor,
                ((m_rect.y * 0.5f) - m_bottomHeight - m_verticalPadding) * 0.5f
            ));

        private Square MiddleLeftBox => new(
            transform.position.ToVector2() + new Vector2(
                -(m_rect.x * 0.5f + m_sideWidth * 2f),
                m_origin.y + m_bottomHeight - ((m_rect.y * 0.5f) - m_bottomHeight) * 0.5f),
            new Vector2(
                m_sideWidth * 2f,
                ((m_rect.y * 0.5f) - m_bottomHeight) * 0.5f
            ));

        private Square MiddleRightBox => new(
            transform.position.ToVector2() + new Vector2(
                m_rect.x * 0.5f + m_sideWidth * 2f,
                m_origin.y + m_bottomHeight - ((m_rect.y * 0.5f) - m_bottomHeight) * 0.5f),
            new Vector2(
                m_sideWidth * 2f,
                ((m_rect.y * 0.5f) - m_bottomHeight) * 0.5f
            ));

        private Square BottomLeftBox => new(
            transform.position.ToVector2() + new Vector2(
                -(m_rect.x * 0.5f + m_sideWidth * 2f),
                m_bottomHeight + m_origin.y - m_rect.y * 0.5f + m_verticalPadding
            ),
            new Vector2(
                m_sideWidth * 2f,
                m_bottomHeight - m_verticalPadding
            ));

        private Square BottomRightBox => new(
            transform.position.ToVector2() + new Vector2(
                m_rect.x * 0.5f + m_sideWidth * 2f,
                m_bottomHeight + m_origin.y - m_rect.y * 0.5f + m_verticalPadding),
            new Vector2(
                m_sideWidth * 2f,
                m_bottomHeight - m_verticalPadding
            ));

        private Square TopBox => new(
            transform.position.ToVector2() + new Vector2(
                0f,
                m_rect.y * 0.5f + m_origin.y + m_sideWidth
            ),
            new Vector2(m_rect.x * 0.5f, m_sideWidth));
        #endregion

        private void FixedUpdate()
        {
            CheckBottom();
            CheckSides();
            CheckTop();
        }

        public void Move(Vector2 vector)
        {
            if (!CanMove)
                return;

            if (m_collision.HasFlag(CollisionChecker.Bottom) && vector.y < 0f)
                vector.y = 0f;

            if (m_collision.HasFlag(CollisionChecker.Right) && vector.x > 0f)
                vector.x = 0f;

            if (m_collision.HasFlag(CollisionChecker.Left) && vector.x < 0f)
                vector.x = 0f;

            if (m_collision.HasFlag(CollisionChecker.Top) && vector.y > 0f)
                vector.y = 0f;

            transform.Translate(vector, Space.World);
        }

        private void CheckBottom()
        {
            m_collision &= ~CollisionChecker.Bottom;
            m_passThrough &= ~CollisionChecker.Bottom;

            if (!m_collisionToggle.HasFlag(CollisionChecker.Bottom))
            {
                StartCoroutine(EnableCollisionToggle(CollisionChecker.Bottom));
                return;
            }

            Collider2D[] bottomColliders = Physics2D.OverlapAreaAll(BottomBox.TopLeft, BottomBox.BottomRight, m_groundDetection);
            if (bottomColliders.Length > 0)
                m_collision |= CollisionChecker.Bottom;

            if(m_collision.HasFlag(CollisionChecker.Bottom))
            {
                RaycastHit2D leftHit = Physics2D.Raycast(transform.position + new Vector3(-m_bottomExtents.x, 1f, 0f), Vector3.down, 1f, m_groundDetection);
                RaycastHit2D middleHit = Physics2D.Raycast(transform.position + Vector3.up, Vector3.down, 1f, m_groundDetection);
                RaycastHit2D rightHit = Physics2D.Raycast(transform.position + new Vector3(m_bottomExtents.x, 1f, 0f), Vector3.down, 1f, m_groundDetection);

                Debug.DrawRay(transform.position + new Vector3(-m_bottomExtents.x, 0.3f, 0f), Vector3.down, Color.darkRed);
                Debug.DrawRay(transform.position + new Vector3(0f, 0.3f, 0f), Vector3.down, Color.darkRed);
                Debug.DrawRay(transform.position + new Vector3(m_bottomExtents.x, 0.3f, 0f), Vector3.down, Color.darkRed);

                float[] yArray = CheckRaycasts(leftHit, middleHit, rightHit);

                if(yArray.Length > 0)
                {
                    float maxY = yArray.Max();
                    Vector3 targetLocation = new Vector3(transform.position.x, maxY, transform.position.z);
                    transform.position = Vector3.Lerp(transform.position, targetLocation, m_bottomFixFactor * Time.deltaTime);
                }
            }

            Collider2D[] bottomPass = Physics2D.OverlapAreaAll(BottomBox.TopLeft, BottomBox.BottomRight, m_passthroughDetection);
            if (bottomPass.Length > 0)
                m_passThrough |= CollisionChecker.Bottom;
        }

        private float[] CheckRaycasts(params RaycastHit2D[] hits)
        {
            List<float> result = new();

            foreach(RaycastHit2D hit in hits)
            {
                if (hit.point != Vector2.zero)
                    result.Add(hit.point.y);
            }

            return result.ToArray();
        }

        private void CheckSides()
        {
            if(XFlipped)
            {
                CheckLeft();
                return;
            }

            CheckRight();
        }

        private void CheckLeft()
        {
            m_collision &= ~CollisionChecker.Left;

            int top = Physics2D.OverlapAreaAll(TopLeftBox.TopRight, TopLeftBox.BottomLeft, m_groundDetection).Length;
            int middle = Physics2D.OverlapAreaAll(MiddleLeftBox.TopRight, MiddleLeftBox.BottomLeft, m_groundDetection).Length;
            int bottom = Physics2D.OverlapAreaAll(BottomLeftBox.TopRight, BottomLeftBox.BottomLeft, m_groundDetection).Length;
            
            if (middle > 0)
                m_collision |= CollisionChecker.Left;

            CanWallGripLeft = false;
            CanHangLeft = false;

            if (top > 0 && middle > 0 && bottom > 0)
            {
                CanWallGripLeft = true;
                return;
            }

            if (top > 0 && middle == 0)
                CanHangLeft = true;
        }

        private void CheckRight()
        {
            m_collision &= ~CollisionChecker.Right;

            int top = Physics2D.OverlapAreaAll(TopRightBox.TopLeft, TopRightBox.BottomRight, m_groundDetection).Length;
            int middle = Physics2D.OverlapAreaAll(MiddleRightBox.TopLeft, MiddleRightBox.BottomRight, m_groundDetection).Length;
            int bottom = Physics2D.OverlapAreaAll(BottomRightBox.TopLeft, BottomRightBox.BottomRight, m_groundDetection).Length;
            
            if (middle > 0)
                m_collision |= CollisionChecker.Right;

            CanWallGripRight = false;
            CanHangRight = false;

            if (top > 0 && middle > 0 && bottom > 0)
            {
                CanWallGripRight = true;
                return;
            }

            if (top > 0 && middle == 0)
                CanHangRight = true;
        }

        private void CheckTop()
        {
            m_collision &= ~CollisionChecker.Top;

            Collider2D[] topColliders = Physics2D.OverlapAreaAll(TopBox.TopLeft, TopBox.BottomRight, m_groundDetection);
            if (topColliders.Length > 0)
                m_collision |= CollisionChecker.Top;
        }

        private IEnumerator EnableCollisionToggle(CollisionChecker side)
        {
            yield return m_enableToggleWait;
            m_collisionToggle |= side;
        }

        public void FixHungPosition()
        {
            Square selectBox()
            {
                if (XFlipped)
                    return TopLeftBox;
                return TopRightBox;
            }

            Square box = selectBox();

            Collider2D ledge = Physics2D.OverlapArea(box.TopLeft, box.BottomRight, m_groundDetection);

            if (ledge == null)
                return;

            Vector3 currentPosition = new(transform.position.x, transform.position.y - m_hangAltitude, transform.position.z);
            Vector3 targetPosition = GetHangTargetPosition(ledge.bounds, XFlipped);

            transform.position = Vector3.Lerp(currentPosition, targetPosition, m_bottomFixFactor * Time.deltaTime);
        }

        private Vector3 GetHangTargetPosition(Bounds bounds, bool xFlipped)
        {
            if(XFlipped)
            {
                return new Vector3(bounds.center.x + bounds.size.x * 0.5f, bounds.center.y + bounds.size.y * 0.5f);
            }

            return new Vector3(bounds.center.x - bounds.size.x * 0.5f, bounds.center.y + bounds.size.y * 0.5f);
        }

        public Bounds? StartClimb()
        {
            Square selectBox()
            {
                if (XFlipped)
                    return TopLeftBox;
                return TopRightBox;
            }

            Square box = selectBox();

            Collider2D ledge = Physics2D.OverlapArea(box.TopLeft, box.BottomRight, m_groundDetection);

            if (ledge == null)
                return null;

            return ledge.bounds;
        }

        public void Climb(Bounds? bounds, float factor)
        {
            if (bounds == null)
                return;

            Vector3 targetPosition = GetHangTargetPosition(bounds.Value, XFlipped);
            transform.position = Vector3.Lerp(transform.position, targetPosition + 0.4f * Vector3.up, (factor) * Time.deltaTime);
        }


#if UNITY_EDITOR
        [Flags]
        public enum CharacterControllerGizmos
        {
            None = 0,
            Origin = 1,
            BottomBox = 2,
            SideBoxes = 4,
            TopBox = 8,
            HangAltitude = 16,
        }

        [Header("Gizmos")]
        [SerializeField] private CharacterControllerGizmos m_showGizmos;
        [SerializeField] private float m_circleRadius = 0.1f;

        private void OnValidate()
        {
            if (m_collider == null)
            {
                Debug.LogError($"{nameof(m_collider)} is not set");
                return;
            }

            m_collider.size = m_rect;
            m_collider.offset = m_origin;
        }

        private void OnDrawGizmos()
        {
            if (m_showGizmos.HasFlag(CharacterControllerGizmos.Origin))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, m_circleRadius);
            }

            if(m_showGizmos.HasFlag(CharacterControllerGizmos.HangAltitude))
            {
                Gizmos.color = Color.yellow;
                Vector3 hangPosition = new(transform.position.x, transform.position.y + m_hangAltitude, transform.position.z);
                Gizmos.DrawSphere(hangPosition, m_circleRadius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_showGizmos.HasFlag(CharacterControllerGizmos.TopBox))
            {
                Gizmos.color = Color.yellow;
                TopBox.DrawGizmo();
            }

            void selectBox(Square leftBox, Square rightBox, bool flipped)
            {
                if (flipped)
                {
                    leftBox.DrawGizmo();
                    return;
                }

                rightBox.DrawGizmo();
            }

            if (m_showGizmos.HasFlag(CharacterControllerGizmos.SideBoxes))
            {
                Gizmos.color = Color.red;
                selectBox(TopLeftBox, TopRightBox, XFlipped);

                Gizmos.color = Color.green;
                selectBox(MiddleLeftBox, MiddleRightBox, XFlipped);

                Gizmos.color = Color.blue;
                selectBox(BottomLeftBox, BottomRightBox, XFlipped);
            }

            if (m_showGizmos.HasFlag(CharacterControllerGizmos.BottomBox))
            {
                Gizmos.color = Color.cyan;
                BottomBox.DrawGizmo();
            }
        }
#endif
    }
}
