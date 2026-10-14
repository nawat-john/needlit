using UnityEngine;
using UnityEngine.InputSystem;

namespace Needlit
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        public GameMode Mode { get; private set; } = GameMode.Explore;

        InputAction move;
        InputAction look;
        InputAction interact;
        InputAction cancel;
        CharacterController body;
        Camera view;
        float pitch;
        float fallSpeed;
        bool focused = true;
        bool skipLookFrame;

        void Awake()
        {
            body = GetComponent<CharacterController>();
            view = GetComponentInChildren<Camera>();
            move = new InputAction("Move", InputActionType.Value);
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            look = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
            interact = new InputAction("Interact", InputActionType.Button, "<Keyboard>/e");
            cancel = new InputAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
        }

        void OnEnable()
        {
            move.Enable();
            look.Enable();
            interact.Enable();
            cancel.Enable();
            ApplyCursor();
        }

        void OnDisable()
        {
            move.Disable();
            look.Disable();
            interact.Disable();
            cancel.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnDestroy()
        {
            move.Dispose();
            look.Dispose();
            interact.Dispose();
            cancel.Dispose();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            focused = hasFocus;
            skipLookFrame = hasFocus;
            ApplyCursor();
        }

        public void SetMode(GameMode next)
        {
            if (Mode != GameMode.Explore && next == GameMode.Explore)
                skipLookFrame = true;
            Mode = next;
            ApplyCursor();
        }

        void ApplyCursor()
        {
            bool explore = focused && Mode == GameMode.Explore;
            Cursor.lockState = explore ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !explore;
        }

        void Update()
        {
            // shortcut: M0 pause stops input only; pause simulation when world systems arrive.
            if (focused && cancel.WasPressedThisFrame())
                SetMode(Mode == GameMode.Explore ? GameMode.Paused : GameMode.Explore);

            if (!focused || Mode != GameMode.Explore) return;

            Vector2 delta = skipLookFrame ? Vector2.zero : look.ReadValue<Vector2>() * 0.12f;
            skipLookFrame = false;
            transform.Rotate(0f, delta.x, 0f);
            pitch = Mathf.Clamp(pitch - delta.y, -85f, 85f);
            view.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);

            Vector2 axis = move.ReadValue<Vector2>();
            Vector3 direction = transform.right * axis.x + transform.forward * axis.y;
            fallSpeed = body.isGrounded ? -1f : fallSpeed + Physics.gravity.y * Time.deltaTime;
            body.Move((direction * 4f + Vector3.up * fallSpeed) * Time.deltaTime);

            if (interact.WasPressedThisFrame() && Physics.Raycast(view.transform.position, view.transform.forward,
                    out RaycastHit hit, 3f) && hit.collider.TryGetComponent(out SceneInteraction target))
                target.Use(this);
        }

        void OnGUI()
        {
            GUI.Label(new Rect(16, 16, 600, 28), $"NEEDLIT  |  {Mode}  |  WASD move  |  Mouse look  |  E interact  |  Esc pause/back");
            if (Mode == GameMode.Explore)
                GUI.Label(new Rect(Screen.width / 2f - 5f, Screen.height / 2f - 12f, 30f, 30f), "+");
            if (Mode == GameMode.Grinding)
                GUI.Label(new Rect(16, 48, 600, 28), "Grinding prototype arrives in M1. Esc to explore.");
        }
    }
}
