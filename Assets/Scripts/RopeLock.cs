using UnityEngine;

public class RopeLock : MonoBehaviour
{
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 🔒 เริ่มเกม = เชือกนิ่ง
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Activate()
    {
        // 🔥 ตอนโดนจับ = เริ่มแกว่ง
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}