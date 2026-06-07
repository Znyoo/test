using UnityEngine;

public class MyPlayerMovement : MonoBehaviour
{
    [Header("조작 설정")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Animator anim;
    private float rotationY = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        
        // 플레이 누르면 마우스 커서를 화면 중앙에 고정하고 숨김 (ESC 누르면 풀림)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 마우스 회전 (내가 마우스를 돌리는 대로 로봇도 같이 회전)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0, rotationY, 0);

        // 2. 키보드 WASD 이동 (내가 쳐다보는 방향 기준으로 앞뒤좌우 이동)
        float x = Input.GetAxisRaw("Horizontal"); // A, D
        float z = Input.GetAxisRaw("Vertical");   // W, S
        
        // 바라보는 방향 기준 벡터 계산
        Vector3 moveDir = (transform.forward * z + transform.right * x).normalized;

        // CharacterController를 사용해 부드럽게 이동
        if (moveDir.magnitude > 0.1f)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // 3. 순정 애니메이션 발걸음 연동
        if (anim != null)
        {
            // 움직임이 있을 때 달리는 애니메이션 재생
            float speedFactor = moveDir.magnitude;
            anim.SetFloat("Blend", speedFactor, 0.1f, Time.deltaTime);
        }
    }
}