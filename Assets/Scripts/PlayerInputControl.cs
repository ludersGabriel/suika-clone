using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControl : MonoBehaviour {
    [SerializeField] private PlayerInput playerInput;

    public static Vector2 moveInput { get; set; }
    public static bool isDropPressed { get; set; }
    public static bool isCancelPressed { get; set; }

    private InputAction moveAction;
    private InputAction dropAction;

    private InputAction cancelAction;


    void Awake() {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dropAction = playerInput.actions["Drop"];
        cancelAction = playerInput.actions["Cancel"];
    }


    // Update is called once per frame
    void Update() {
        moveInput = moveAction.ReadValue<Vector2>();
        isDropPressed = dropAction.triggered;
        isCancelPressed = cancelAction.triggered;
    }
}
