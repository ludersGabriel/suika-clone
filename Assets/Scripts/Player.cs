using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        moveInput = PlayerInputControl.moveInput;
    }

    void FixedUpdate() {
        Vector3 vel = rb.linearVelocity;
        vel.x = moveInput.x * moveSpeed;
        rb.linearVelocity = vel;
    }
}
