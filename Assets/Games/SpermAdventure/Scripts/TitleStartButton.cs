using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleStartButton : MonoBehaviour
{
    [SerializeField] private string meiosisSceneName = "01_Meiosis";

    public void StartGame()
    {
        SceneManager.LoadScene(meiosisSceneName);
    }
}
