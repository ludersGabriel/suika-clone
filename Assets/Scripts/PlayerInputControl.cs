using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControl : MonoBehaviour {
    [SerializeField] private PlayerInput playerInput;

    public static Vector2 moveInput { get; set; }
    public static bool isDropPressed { get; set; }

    private InputAction moveAction;
    private InputAction dropAction;


    void Awake() {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dropAction = playerInput.actions["Drop"];
    }


    // Update is called once per frame
    void Update() {
        moveInput = moveAction.ReadValue<Vector2>();
        isDropPressed = dropAction.triggered;

    }
}
