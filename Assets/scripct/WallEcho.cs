using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallEcho : MonoBehaviour
{
    private Material mat;
    private Coroutine glowCoroutine;
    
    // 현재 체크해야 할 파동들을 담아둘 리스트
    private List<GameObject> activeEchoes = new List<GameObject>();

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
    }

    // EchoManager가 파동을 쏠 때 이 함수를 호출해서 벽에 파동을 등록해줍니다.
    public void RegisterEcho(GameObject newEcho)
    {
        if (newEcho != null && !activeEchoes.Contains(newEcho))
        {
            activeEchoes.Add(newEcho);
        }
    }

    void Update()
    {
        if (activeEchoes.Count == 0) return;

        // 리스트를 돌면서 거리 체크 (Find 형식을 쓰지 않아 성능 저하가 없습니다)
        for (int i = activeEchoes.Count - 1; i >= 0; i--)
        {
            GameObject echo = activeEchoes[i];

            if (echo == null)
            {
                activeEchoes.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(transform.position, echo.transform.position);
            float echoRadius = echo.transform.localScale.x * 0.5f;

            // 파동 범위 안에 들어오면 빛나기 시작
            if (dist < echoRadius + 1.5f)
            {
                if (glowCoroutine != null) StopCoroutine(glowCoroutine);
                glowCoroutine = StartCoroutine(Glow());
                
                // 이미 빛났으므로 이 파동은 리스트에서 제외하여 중복 계산 방지
                activeEchoes.RemoveAt(i);
            }
        }
    }

    IEnumerator Glow()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            float intensity = Mathf.Lerp(3f, 0f, progress);
            mat.SetColor("_EmissionColor", Color.white * intensity);

            yield return null;
        }

        mat.SetColor("_EmissionColor", Color.black);
    }
}