using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwing2D : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Start Movement")]
    public float startForwardSpeed = 7f;
    public float startJumpForce = 8f;

    [Header("Attach")]
    public float frequency = 3f;
    public float damping = 0.5f;

    [Header("Boost")]
    public float releaseBoost = 1.5f;

    [Header("Air Control 🔥")]
    public float downForce = 15f;
    public float forwardForce = 5f;

    bool hasStarted = false;

    SpringJoint2D joint;
    Rigidbody2D currentRope;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = true;
        rb.linearDamping = 0f;
    }

    void Update()
    {
        bool press = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool hold = Keyboard.current.spaceKey.isPressed;
        bool release = Keyboard.current.spaceKey.wasReleasedThisFrame;

        // 🎬 เริ่มเกม
        if (!hasStarted && press)
        {
            hasStarted = true;
            rb.linearVelocity = new Vector2(startForwardSpeed, startJumpForce);
            return;
        }

        if (!hasStarted) return;

        if (hold)
        {
            Attach();

            // 🔥 กดค้าง = ดิ่ง + พุ่ง
            ApplyAirControl();
        }

        if (release)
        {
            Detach();
        }
    }

    void ApplyAirControl()
    {
        // ⬇️ ดึงลง
        rb.AddForce(Vector2.down * downForce);

        // ➡️ ดันไปข้างหน้า
        rb.AddForce(Vector2.right * forwardForce);
    }

    void Attach()
    {
        if (joint != null) return;
        if (currentRope == null) return;

        // 🔥 ปลดล็อคเชือก
        RopeLock ropeLock = currentRope.GetComponent<RopeLock>();
        if (ropeLock != null)
        {
            ropeLock.Activate();
        }

        joint = gameObject.AddComponent<SpringJoint2D>();

        joint.connectedBody = currentRope;

        Vector2 hitPoint = (Vector2)transform.position;

        joint.autoConfigureDistance = false;
        joint.anchor = Vector2.zero;

        joint.connectedAnchor = currentRope.transform.InverseTransformPoint(hitPoint);

        joint.distance = 0.1f;

        joint.frequency = frequency;
        joint.dampingRatio = damping;
    }

    void Detach()
    {
        if (joint != null)
        {
            // 🚀 Boost ตอนปล่อย
            Vector2 v = rb.linearVelocity;
            v.x *= releaseBoost;
            rb.linearVelocity = v;

            Destroy(joint);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            currentRope = other.GetComponent<Rigidbody2D>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            if (currentRope == other.GetComponent<Rigidbody2D>())
                currentRope = null;
        }
    }
}