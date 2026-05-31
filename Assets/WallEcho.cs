using UnityEngine;
using System.Collections;

public class WallEcho : MonoBehaviour
{
    private Material mat;
    private Coroutine glowCoroutine;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
    }

    void Update()
    {
        // EchoWave 오브젝트 찾아서 거리 체크
        GameObject[] echos = GameObject.FindGameObjectsWithTag("EchoWave");
        foreach (GameObject echo in echos)
        {
            float dist = Vector3.Distance(transform.position, echo.transform.position);
            float echoRadius = echo.transform.localScale.x * 0.5f;

            if (dist < echoRadius + 1f)
            {
                if (glowCoroutine != null) StopCoroutine(glowCoroutine);
                glowCoroutine = StartCoroutine(Glow());
            }
        }
    }

    IEnumerator Glow()
    {
        float duration = 1f;
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