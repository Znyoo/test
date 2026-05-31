using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class EchoManager : MonoBehaviour
{
    [Header("에코 설정")]
    public GameObject echoWavePrefab;
    public float echoDuration = 1.5f;
    public float footstepInterval = 0.4f;
    public float echoSpeed = 15f;

    private float footstepTimer = 0f;

    void Update()
    {
        bool moving = Keyboard.current.wKey.isPressed ||
                      Keyboard.current.sKey.isPressed ||
                      Keyboard.current.aKey.isPressed ||
                      Keyboard.current.dKey.isPressed;

        if (moving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                SpawnEcho(transform.position, transform.forward);
                footstepTimer = 0f;
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnEcho(transform.position, transform.forward);
        }
    }

    void SpawnEcho(Vector3 position, Vector3 direction)
    {
        GameObject echo = Instantiate(echoWavePrefab, position, Quaternion.identity);
        echo.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
        StartCoroutine(ExpandEcho(echo, direction));
    }

    IEnumerator ExpandEcho(GameObject echo, Vector3 direction)
    {
        float elapsed = 0f;
        Material mat = echo.GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        while (elapsed < echoDuration)
        {
            if (echo == null) yield break;

            elapsed += Time.deltaTime;
            float progress = elapsed / echoDuration;

            // 앞으로 이동
            echo.transform.position += direction * echoSpeed * Time.deltaTime;

            // 납작한 원반 모양 유지
            echo.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);

            // 점점 사라짐
            float alpha = Mathf.Lerp(0.8f, 0f, progress);
            Color col = mat.color;
            col.a = alpha;
            mat.color = col;

            yield return null;
        }

        if (echo != null) Destroy(echo);
    }
}