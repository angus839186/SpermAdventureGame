using UnityEngine;
using UnityEngine.InputSystem;

public class SpermCameraController : MonoBehaviour
{
    [Header("追蹤目標")]
    [SerializeField] private Transform target;

    [Header("滑鼠控制鏡頭")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private float maxVerticalAngle = 85f;
    [SerializeField] private bool lockCursorOnPlay = true;

    private float yaw;
    private float pitch;

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;

        if (target != null)
        {
            transform.position = target.position;
            Vector3 euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = euler.x;
        }
    }

    private void Start()
    {
        if (target != null)
            SetTarget(target);

        if (lockCursorOnPlay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector2 lookInput = ReadMouseInput();
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);

        transform.SetPositionAndRotation(target.position, Quaternion.Euler(pitch, yaw, 0f));
    }

    private static Vector2 ReadMouseInput()
    {
        Mouse mouse = Mouse.current;
        return mouse == null ? Vector2.zero : mouse.delta.ReadValue();
    }
}
