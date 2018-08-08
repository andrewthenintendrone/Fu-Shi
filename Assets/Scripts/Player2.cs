using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    #region Fields

    public float m_rayDistance;
    public float m_slopeLimitAngle;

    private BoxCollider2D m_boxCollider;

    private int m_colLayerMask;

    private RaycastHit2D m_hitBottom;
    private RaycastHit2D m_hitBottomLeft;
    private RaycastHit2D m_hitBottomRight;
    private RaycastHit2D m_hitTop;
    private RaycastHit2D m_hitTopLeft;
    private RaycastHit2D m_hitTopRight;
    private RaycastHit2D m_hitLeft;
    private RaycastHit2D m_hitLeftTop;
    private RaycastHit2D m_hitLeftBottom;
    private RaycastHit2D m_hitRight;
    private RaycastHit2D m_hitRightTop;
    private RaycastHit2D m_hitRightBottom;

    private float m_limitMinY;
    private float m_limitMaxY;
    private float m_limitMinX;
    private float m_limitMaxX;

    private float m_posMinY;
    private float m_posMaxY;
    private float m_posMinX;
    private float m_posMaxX;

    private bool m_isGrounded;
    private bool m_isTopBlocked;
    private bool m_isLeftBlocked;
    private bool m_isRightBlocked;
    private bool m_isAboveSlope;
    private bool m_isOnSlope;

    private bool m_isRunning;
    private bool m_isJumping;

    private Vector2 m_speed;

    #endregion

    #region Properties

    public Vector2 Speed { get { return m_speed; } }
    public bool isGrounded { get { return m_isGrounded; } }
    public bool isJumping { get { return m_isJumping; } }

    #endregion

    #region Init

    void Awake()
    {
        m_boxCollider = Utils.GetSafeComponent<BoxCollider2D>(gameObject);

        m_colLayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    void Start()
    {
        m_limitMinY = ComputeLimitBottom();
        m_limitMinX = ComputeLimitLeft();
        m_limitMaxX = ComputeLimitRight();
        m_limitMaxY = ComputeLimitTop();
    }

    #endregion

    void Update()
    {
        #region ComputeLimits

        // Compute limits when needed

        m_limitMinY = ComputeLimitBottom();

        if (m_speed.x < 0)
            m_limitMinX = ComputeLimitLeft();

        if (m_speed.x > 0)
            m_limitMaxX = ComputeLimitRight();

        if (m_speed.y > 0)
            m_limitMaxY = ComputeLimitTop();

        // Calculate collider position for each limits
        m_posMinY = m_limitMinY + m_boxCollider.size.y / 2 - m_boxCollider.offset.y;
        m_posMaxY = m_limitMaxY - m_boxCollider.size.y / 2 - m_boxCollider.offset.y;
        m_posMinX = m_limitMinX + m_boxCollider.size.x / 2 - m_boxCollider.offset.x;
        m_posMaxX = m_limitMaxX + m_boxCollider.size.x / 2 - m_boxCollider.offset.x;

        #endregion

        #region Handle collisions

        // Define controller state (bigger buffer to leave grounded state if on a slope)

    }

    #region Compute limits methods

    private float ComputeLimitBottom()
    {
        Ray rayBottomLeft = new Ray(new Vector3(m_boxCollider.bounds.min.x, m_boxCollider.bounds.center.y, 0), Vector3.down);
        Ray rayBottomRight = new Ray(new Vector3(m_boxCollider.bounds.max.x, m_boxCollider.bounds.max.y, 0), Vector3.down);
        Ray rayBottom = new Ray(new Vector3(m_boxCollider.bounds.center.x, m_boxCollider.bounds.max.y, 0), Vector3.down);

        Vector3 limitBottomLeft = rayBottomLeft.origin + Vector3.down * m_rayDistance;
        Vector3 limitBottomRight = rayBottomRight.origin + Vector3.down * m_rayDistance;
        Vector3 limitBottom = rayBottomLeft.origin + Vector3.down * m_rayDistance;

        bool slopeLeft = false;
        bool slopeRight = false;

        if (Physics.Raycast(rayBottomLeft, out m_hitBottomLeft, m_rayDistance, m_colLayerMask))
        {
            limitBottomLeft = m_hitBottomLeft.point;
            slopeLeft = Mathf.Abs(Vector3.Angle(m_hitBottomLeft.normal, Vector3.right) - 90) >= 5;
        }
        if (Physics.Raycast(rayBottomRight, out m_hitBottomRight, m_rayDistance, m_colLayerMask))
        {
            limitBottomRight = m_hitBottomLeft.point;
            slopeRight = Mathf.Abs(Vector3.Angle(m_hitBottomRight.normal, Vector3.right) - 90) >= 5;
        }

        m_isAboveSlope = (slopeLeft && slopeRight) ||
            (slopeLeft && !slopeRight && limitBottomLeft.y >= limitBottomRight.y) ||
            (!slopeLeft && slopeRight && limitBottomRight.y >= limitBottomLeft.y);

        if (m_isAboveSlope)
        {
            if (Physics.Raycast(rayBottom, out m_hitBottom, m_rayDistance, m_colLayerMask))
            {
                limitBottom = m_hitBottom.point;
            }

            if (slopeLeft && limitBottomLeft.y - limitBottom.y > 5)
                return limitBottomLeft.y;

            else if (slopeRight && limitBottomRight.y - limitBottom.y > 5)
                return limitBottomRight.y;

            else
                return limitBottom.y;
        }
        else
        {
            return Mathf.Max(limitBottomLeft.y, limitBottomRight.y);
        }
    }

    private float ComputeLimitTop()
    {
        Ray rayTopLeft = new Ray(new Vector3(m_boxCollider.bounds.min.x + 2, m_boxCollider.bounds.center.y, 0), Vector3.up);
        Ray rayTopRight = new Ray(new Vector3(m_boxCollider.bounds.min.x - 2, m_boxCollider.bounds.center.y, 0), Vector3.up);

        Vector3 limitTopLeft = rayTopLeft.origin + Vector3.up * m_rayDistance;
        Vector3 limitTopRight = rayTopRight.origin + Vector3.up * m_rayDistance;

        if(Physics.Raycast())
    }

    #endregion
}
