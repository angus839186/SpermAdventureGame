using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SpermController : MonoBehaviour
{
    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("視角")]
    [SerializeField] private Transform cameraTarget;

    private CharacterController characterController;

    public void SetCameraTarget(Transform target)
    {
        cameraTarget = target;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 moveInput = ReadMoveInput();

        Vector3 forward = cameraTarget != null ? cameraTarget.forward : transform.forward;
        Vector3 right = cameraTarget != null ? cameraTarget.right : transform.right;

        if (forward.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private static Vector2 ReadMoveInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (keyboard.wKey.isPressed)
            input.y += 1f;
        if (keyboard.sKey.isPressed)
            input.y -= 1f;
        if (keyboard.dKey.isPressed)
            input.x += 1f;
        if (keyboard.aKey.isPressed)
            input.x -= 1f;

        return input.sqrMagnitude > 1f ? input.normalized : input;
    }
}
