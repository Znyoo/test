using UnityEngine;
using UnityEngine.SceneManagement; // 다른 방(씬)으로 이동할 때 꼭 필요한 부품이야!

public class MainMenu : MonoBehaviour
{
    // 이 함수가 실행되면 미로 게임방으로 넘어갈 거야!
    public void PlayGame()
    {
        // 네 진짜 미로 씬 이름인 MAZEGAME으로 변경!
        SceneManager.LoadScene("MAZEGAME");
    }
}