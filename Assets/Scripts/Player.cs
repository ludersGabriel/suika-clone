using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;


    void Awake() {
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    void Update() {
        moveInput = PlayerInputControl.moveInput;
    }

    void FixedUpdate() {
        move();
    }


    void move() {
        Vector3 newPos = transform.position + new Vector3(moveInput.x * moveSpeed, 0, 0);
        rb.linearVelocity = newPos;
    }

}
