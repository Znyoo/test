using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MouseMovement : MonoBehaviour {

    [Header("조작 설정")]
    public float moveSpeed = 3.0f;           
    public float mouseRotationSpeed = 20f; 

    [Header("컴포넌트 참조")]
    public Animator anim;
    public CharacterController controller;
    
    private Camera cam;

    void Start () {
        cam = Camera.main;
        if(anim == null) anim = GetComponent<Animator>();
        if(controller == null) controller = GetComponent<CharacterController>();
    }
    
    void Update () {
        // 1. 키보드 입력에 의한 이동 (이동 방향을 월드 좌표 기준으로 고정)
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S
        
        // 카메라를 기준으로 이동 방향을 정하지 않고, 순수하게 월드 좌표로 이동
        // 이러면 WASD가 헷갈리지 않고 항상 같은 방향으로 이동해
        Vector3 move = new Vector3(x, 0, z).normalized; 
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 애니메이션 연결
        if(anim != null) anim.SetFloat("Blend", move.magnitude, 0.1f, Time.deltaTime);

        // 2. 마우스 커서 방향을 향해 즉시 회전 (이동과 별개로 작동)
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance)) {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 lookDirection = targetPoint - transform.position;
            lookDirection.y = 0f; 

            if (lookDirection.magnitude > 0.1f) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                // 마우스 회전은 매우 빠르게 반응하도록 설정
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mouseRotationSpeed * Time.deltaTime);
            }
        }
    }
}