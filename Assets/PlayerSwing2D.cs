using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwing2D : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Start Movement")]
    public float startForwardSpeed = 7f;
    public float startJumpForce = 8f;

    [Header("Swing")]
    public float ropeDistance = 2f;
    public float frequency = 2f;
    public float damping = 0.5f;

    [Header("Boost")]
    public float releaseBoost = 1.5f;

    bool hasStarted = false;

    SpringJoint2D joint;
    Transform currentHook;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // กันกลิ้ง
        rb.freezeRotation = true;

        // ฟีลลื่น
        rb.linearDamping = 0f;
    }

    void Update()
    {
        bool press = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool hold = Keyboard.current.spaceKey.isPressed;
        bool release = Keyboard.current.spaceKey.wasReleasedThisFrame;

        //  เริ่มเกม (พุ่งครั้งเดียว)
        if (!hasStarted && press)
        {
            hasStarted = true;
            rb.linearVelocity = new Vector2(startForwardSpeed, startJumpForce);
            return;
        }

        if (!hasStarted) return;

        //  โหน
        if (hold)
        {
            Attach();
        }

        //  ปล่อย
        if (release)
        {
            Detach();
        }
    }

    //  เกาะ
    void Attach()
    {
        if (joint != null) return;
        if (currentHook == null) return;

        joint = gameObject.AddComponent<SpringJoint2D>();

        // ใช้ตำแหน่ง Hook จริง
        joint.connectedBody = null;
        joint.connectedAnchor = currentHook.position;

        joint.autoConfigureDistance = false;
        joint.distance = ropeDistance;

        joint.frequency = frequency;
        joint.dampingRatio = damping;
    }

    //  หลุด + Boost
    void Detach()
    {
        if (joint != null)
        {
            //  Boost เฉพาะแกน X (ให้พุ่งไปข้างหน้า)
            Vector2 v = rb.linearVelocity;
            v.x *= releaseBoost;
            rb.linearVelocity = v;

            Destroy(joint);
        }
    }

    //  ตรวจจับ Hook
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            currentHook = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            if (currentHook == other.transform)
                currentHook = null;
        }
    }
}