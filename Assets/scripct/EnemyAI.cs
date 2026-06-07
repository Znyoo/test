using UnityEngine;
using UnityEngine.SceneManagement; // 게임오버 시 씬 재시작을 위해 필요

public class EnemyAI : MonoBehaviour
{
    [Header("괴물 이동 설정")]
    public float moveSpeed = 3.5f;     // 괴물의 이동 속도
    private Vector3 targetPosition;   // 괴물이 이동할 목적지
    private bool isChasing = false;   // 현재 추격 중인지 여부

    void Start()
    {
        // 게임 시작 시에는 제자리에 가만히 서 있도록 설정
        targetPosition = transform.position;
    }

    void Update()
    {
        // 추격 상태일 때만 목적지를 향해 이동
        if (isChasing)
        {
            // 플레이어가 스페이스바를 누른 위치(targetPosition)로 등속 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 목적지에 거의 다 와가면(거리가 0.1 이하) 추격을 멈추고 대기
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isChasing = false;
                Debug.Log("괴물: 소리가 난 곳에 왔지만 아무것도 없군...");
            }
        }
    }

    // [중요] EchoManager에서 스페이스바 파동을 쏠 때 이 함수를 원격 호출함!
    public void HearSound(Vector3 soundPosition)
    {
        targetPosition = soundPosition; // 소리가 난 플레이어의 위치를 목적지로 저장
        isChasing = true;              // 추격 시작
        Debug.Log("괴물: 으아악! 소리를 감지했다! 그쪽으로 간다!");
    }

    // 괴물이 플레이어와 부딪혔을 때 (잡혔을 때) 처리
    private void OnCollisionEnter(Collision collision)
    {
        // 3D 환경이므로 Collision(또는 Collider)을 사용해 검사
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("괴물에게 잡혔습니다! 스테이지를 재시작합니다.");
            // 현재 활성화된 미로 씬을 처음부터 다시 로드 (게임 오버)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // 만약 괴물이나 플레이어의 콜라이더가 Trigger 모드라면 이 함수가 실행됨 (보험용)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("괴물에게 잡혔습니다! (Trigger)");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}