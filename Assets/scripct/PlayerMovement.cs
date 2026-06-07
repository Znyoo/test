using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 속도")]
    public float moveSpeed = 5f;
    
    [Header("마우스 회전 감도")]
    public float mouseSensitivity = 2f;

    private Rigidbody rb;
    private float rotationY = 0f;

    void Start()
    {
        // 캐릭터의 Rigidbody 부품 가져오기
        rb = GetComponent<Rigidbody>();
        
        // 게임 시작하면 마우스 커서를 화면 중앙에 가두고 숨기기 (ESC 누르면 풀림)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 마우스로 좌우 회전 감지 (기본 마우스 입력 방식)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY += mouseX;
        
        // 캐릭터 몸통을 마우스 움직임에 맞춰 회전
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    void FixedUpdate()
    {
        // 2. 키보드 WASD 이동 감지
        Vector2 moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) moveInput.y += 1; // 앞으로
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1; // 뒤로
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1; // 왼쪽으로
        if (Input.GetKey(KeyCode.D)) moveInput.x += 1; // 오른쪽으로

        // 내가 바라보는 앞방향(forward)과 우측방향(right)을 기준으로 실제 이동할 방향 계산
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        
        // Rigidbody 물리 힘을 이용해 캐릭터를 진짜로 이동시킴 (y축 중력은 유지)
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }
}