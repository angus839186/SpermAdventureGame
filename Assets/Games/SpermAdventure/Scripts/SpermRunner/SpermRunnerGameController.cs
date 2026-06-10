using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpermRunnerGameController : MonoBehaviour
{
    [Header("關卡物件")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private SpermRunnerGoal goalLeft;
    [SerializeField] private SpermRunnerGoal goalRight;
    public SpermRunnerGoal correctGoal;

    [Header("介面")]
    [SerializeField] private Text messageText;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private string instructionMessage = "向前移動，找到卵子，小心不要撞到子宮頸壁或免疫細胞";
    [SerializeField] private string deathMessage = "挑戰失敗";


    [Header("聲音提示")]
    [SerializeField] private AudioSource directionAudioSource;
    [SerializeField] private float cueInterval = 1.5f;


    private bool goalsRevealed;
    private bool gameEnded;
    public AudioClip cueClip;

    private void Start()
    {
        if (player != null && spawnPoint != null)
            player.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        deathPanel.SetActive(false);
        messageText.text = instructionMessage;

        retryButton.onClick.AddListener(RestartLevel);
        returnButton.onClick.AddListener(ReturnToTitle);

        cueClip = CreateCueClip();
    }

    public void Die()
    {
        if (gameEnded)
            return;

        gameEnded = true;
        messageText.text = deathMessage;
        deathPanel.SetActive(true);
        SetPlayerEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReachGoal(SpermRunnerGoal goal)
    {
        if (gameEnded || !goalsRevealed)
            return;

        if (goal != correctGoal)
        {
            Die();
            return;
        }

        gameEnded = true;
        messageText.text = "成功找到卵子";
        SetPlayerEnabled(false);
        SceneManager.LoadScene("03_Mitosis");
    }

    private void ChooseCorrectGoal()
    {
        SpermRunnerGoal Left = goalLeft.GetComponent<SpermRunnerGoal>();
        SpermRunnerGoal Right = goalRight.GetComponent<SpermRunnerGoal>();
        correctGoal = Random.value < 0.5f ? Left : Right;
    }

    public void RevealGoals()
    {
        goalsRevealed = true;
        messageText.text = "聆聽聲音提示，找到正確的卵子";
        ChooseCorrectGoal();
        directionAudioSource.panStereo = correctGoal == goalLeft ? -1f : 1f;
        StartCoroutine(GoalSoundCoroutine());
    }

    private void PlayGoalHint()
    {
        directionAudioSource.PlayOneShot(cueClip);
    }

    IEnumerator GoalSoundCoroutine()
    {
        while (!gameEnded)
        {
            PlayGoalHint();
            yield return new WaitForSeconds(cueInterval);
        }
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (player == null)
            return;

        SpermController controller = player.GetComponent<SpermController>();
        if (controller != null)
            controller.enabled = enabled;
    }

    private AudioClip CreateCueClip()
    {
        const int sampleRate = 44100;
        const float duration = 0.18f;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float envelope = 1f - i / (float)sampleCount;
            samples[i] = Mathf.Sin(2f * Mathf.PI * 720f * time) * envelope * 0.2f;
        }

        AudioClip clip = AudioClip.Create("DirectionCue", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene("02_SpermRunner");
    }

    private void ReturnToTitle()
    {
        SceneManager.LoadScene("00_Title");
    }
}
