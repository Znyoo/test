using UnityEngine;
using System.Collections;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;

public class EchoManager : MonoBehaviour
{
    [Header("에코 설정")]
    public GameObject echoWavePrefab;
    public float echoDuration = 1.5f;
    public float echoSpeed = 15f;

    [Header("🔋 에너지 시스템 설정")]
    public Slider energySlider;       
    public TextMeshProUGUI energyText;
    
    [Range(1, 20)] public int maxEchoCount = 10; 
    public float rechargeSpeed = 20f; // R키 누를 때 차오르는 속도
    
    private float currentEnergy;
    private float maxEnergy = 100f;
    private float spaceEchoCost; 

    [Header("🎨 UI 연출 설정")]
    public Image vignetteWarningImage; 
    private Vector3 originalTextPosition; 

    private WallEcho[] allWalls;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        spaceEchoCost = maxEnergy / maxEchoCount;
        currentEnergy = maxEnergy;
        
        if (energyText != null) originalTextPosition = energyText.transform.localPosition;
        
        // 경고 없는 최신 방식으로 벽들 찾기
        allWalls = FindObjectsByType<WallEcho>(FindObjectsSortMode.None);
        UpdateUI();
    }

    void Update()
    {
        // 1. R키를 누르면 충전 (Old Input Manager 방식)
        if (Input.GetKey(KeyCode.R))
        {
            RechargeEnergy(rechargeSpeed * Time.deltaTime);
        }

        // 2. [합침] 마우스 조준 시스템 (캐릭터가 마우스 방향을 바라봄)
        if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(rayDistance);
                Vector3 lookDirection = mouseWorldPosition - transform.position;
                lookDirection.y = 0f; 

                if (lookDirection.magnitude > 0.1f)
                {
                    // 마우스 방향으로 부드럽게 회전
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 20f);
                }
            }
        }

        // 3. [핵심] 발사 시스템 + UI 감소 한 번에 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentEnergy >= spaceEchoCost)
            {
                UseEnergy(spaceEchoCost); // 여기서 에너지를 깎고 UI를 갱신함!
                SpawnEcho(transform.position, transform.forward); // 웨이브 발사!
            }
        }

        ApplyGlitchEffect();
        ApplyVignetteWarning();
    }

    void OnValidate()
    {
        spaceEchoCost = maxEnergy / Mathf.Max(1, maxEchoCount);
    }

    void SpawnEcho(Vector3 position, Vector3 direction)
    {
        if (echoWavePrefab == null) return;

        GameObject echo = Instantiate(echoWavePrefab, position, Quaternion.identity);
        echo.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
        echo.tag = "EchoWave"; 

        if (allWalls == null || allWalls.Length == 0) 
            allWalls = FindObjectsByType<WallEcho>(FindObjectsSortMode.None);
            
        foreach (WallEcho wall in allWalls) if (wall != null) wall.RegisterEcho(echo);

        StartCoroutine(ExpandEcho(echo, direction));
    }

    IEnumerator ExpandEcho(GameObject echo, Vector3 direction)
    {
        float elapsed = 0f;
        Renderer rend = echo.GetComponent<Renderer>();
        if (rend == null) yield break;

        Material mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        yield return null;

        while (elapsed < echoDuration)
        {
            if (echo == null) yield break;
            elapsed += Time.deltaTime;
            float progress = elapsed / echoDuration;
            echo.transform.position += direction * echoSpeed * Time.deltaTime;
            
            float alpha = Mathf.Lerp(0.8f, 0f, progress);
            Color col = mat.color; col.a = alpha; mat.color = col;
            yield return null;
        }
        if (echo != null) Destroy(echo);
    }

    void UseEnergy(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        UpdateUI(); // 바뀐 에너지를 UI에 적용
    }

    public void RechargeEnergy(float amount)
    {
        currentEnergy += amount;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        UpdateUI(); // 바뀐 에너지를 UI에 적용
    }

    void UpdateUI()
    {
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }

        // 바로 이 부분이 "WAVE READY : IIIIII" 글자를 그려주는 곳이야!
        int currentCount = Mathf.FloorToInt(currentEnergy / spaceEchoCost);
        string bars = "";
        for (int i = 0; i < currentCount; i++) bars += "I";
        
        if (energyText != null) 
            energyText.text = $"WAVE READY : <color=#00FFFF>{bars}</color>";
    }

    void ApplyGlitchEffect() { }
    void ApplyVignetteWarning() { }
}