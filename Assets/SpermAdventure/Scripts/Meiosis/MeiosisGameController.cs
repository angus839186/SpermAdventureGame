using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeiosisGameController : MonoBehaviour
{
    [Header("標題")]
    [SerializeField] private string title;
    [Header("提示按鈕文字")]
    [SerializeField] private string hintButtonLabel;
    [Header("重試按鈕文字")]
    [SerializeField] private string retryButtonLabel;
    [Header("關卡流程Prefabs")]
    [SerializeField] private GameObject[] stagePrefabs;
    [Header("UI文字")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text promptText;
    [SerializeField] private Text hintText;

    [Header("UI按鈕")]
    [SerializeField] private Button primaryButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button retryButton;

    [Header("UI容器")]
    [SerializeField] private RectTransform stageRoot;
    [SerializeField] private RectTransform uterusTarget;

    [Header("遊戲設定")]
    [SerializeField] private string completionSceneName;
    [SerializeField] private float completionDelaySeconds = 1.5f;
    [SerializeField, TextArea] private string completionMessage;

    private MeiosisStagePrefab currentStage;
    private GameObject currentStageInstance;
    private int stageIndex;
    private bool completionInProgress;

    private void Start()
    {
        if (!ValidateUiReferences())
            return;

        LoadStage(0);
    }


    public void ValidateCurrentStage()
    {
        if (currentStage == null)
            return;

        if (currentStage.PassWithoutSlots || AllSlotsFilled())
        {
            NextStage();
            return;
        }

        ShowFeedback(currentStage.FailMessage, currentStage.Hint);
    }

    public void ShowHint()
    {
        if (currentStage != null)
            hintText.text = currentStage.Hint;
    }

    public void RetryCurrentStage()
    {
        LoadStage(stageIndex);
    }

    public void NotifyDragReleased(MeiosisDraggable draggable)
    {
        if (TrySnapToCorrectSlot(draggable))
            return;

        Vector2 dragScreenPoint = RectTransformUtility.WorldToScreenPoint(null, draggable.RectTransform.position);
        if (draggable.IsSpermCell && uterusTarget != null && RectTransformUtility.RectangleContainsScreenPoint(uterusTarget, dragScreenPoint, null))
            SceneManager.LoadScene("02_SpermRunner");
    }

    private void LoadStage(int index)
    {
        completionInProgress = false;
        stageIndex = index;
        if (currentStageInstance != null)
            Destroy(currentStageInstance);

        if (stagePrefabs == null || index < 0 || index >= stagePrefabs.Length || stagePrefabs[index] == null)
            return;

        currentStageInstance = Instantiate(stagePrefabs[index], stageRoot);
        currentStageInstance.SetActive(true);
        currentStage = currentStageInstance.GetComponent<MeiosisStagePrefab>();
        foreach (MeiosisDraggable draggable in currentStageInstance.GetComponentsInChildren<MeiosisDraggable>(true))
            draggable.Bind(this);

        MeiosisUterusTarget stageUterus = currentStageInstance.GetComponentInChildren<MeiosisUterusTarget>(true);
        uterusTarget = stageUterus != null ? stageUterus.GetComponent<RectTransform>() : null;
        ApplyStageUi();
    }

    private void NextStage()
    {
        if (stageIndex + 1 >= stagePrefabs.Length)
        {
            CompleteFlow();
            return;
        }

        LoadStage(stageIndex + 1);
    }

    private void CompleteFlow()
    {
        if (completionInProgress || string.IsNullOrEmpty(completionSceneName))
            return;

        completionInProgress = true;
        primaryButton.gameObject.SetActive(false);
        ShowFeedback(completionMessage, string.Empty);
        StartCoroutine(LoadCompletionSceneAfterDelay());
    }

    private IEnumerator LoadCompletionSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(completionDelaySeconds);
        SceneManager.LoadScene(completionSceneName);
    }

    private bool TrySnapToCorrectSlot(MeiosisDraggable draggable)
    {
        if (draggable.IsLocked || draggable.IsSpermCell || currentStageInstance == null)
            return false;

        foreach (MeiosisDropSlot slot in currentStageInstance.GetComponentsInChildren<MeiosisDropSlot>(true))
        {
            Vector2 dragScreenPoint = RectTransformUtility.WorldToScreenPoint(null, draggable.RectTransform.position);
            if (!RectTransformUtility.RectangleContainsScreenPoint(slot.RectTransform, dragScreenPoint, null))
                continue;

            if (!slot.TryAccept(draggable))
                return false;

            RectTransform snap = slot.SnapTarget;
            draggable.LockTo(GetAnchoredPositionInStageRoot(snap), snap.localEulerAngles.z);
            hintText.text = string.Empty;
            return true;
        }

        return false;
    }

    private bool AllSlotsFilled()
    {
        MeiosisDropSlot[] slots = currentStageInstance.GetComponentsInChildren<MeiosisDropSlot>(true);
        if (slots.Length == 0)
            return false;

        foreach (MeiosisDropSlot slot in slots)
        {
            if (!slot.Occupied)
                return false;
        }

        return true;
    }

    private Vector2 GetAnchoredPositionInStageRoot(RectTransform target)
    {
        Vector3 worldPosition = target.TransformPoint(target.rect.center);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stageRoot,
            RectTransformUtility.WorldToScreenPoint(null, worldPosition),
            null,
            out Vector2 localPoint);
        return localPoint;
    }

    private void ApplyStageUi()
    {
        titleText.text = GetCurrentStageTitle();
        promptText.text = currentStage != null ? currentStage.Prompt : string.Empty;
        hintText.text = string.Empty;
        primaryButton.gameObject.SetActive(currentStage != null && !currentStage.HidePrimaryButton);
        primaryButton.GetComponentInChildren<Text>().text = currentStage != null ? currentStage.PrimaryButtonLabel : string.Empty;
        primaryButton.onClick.RemoveAllListeners();
        primaryButton.onClick.AddListener(ValidateCurrentStage);
        hintButton.GetComponentInChildren<Text>().text = hintButtonLabel;
        retryButton.GetComponentInChildren<Text>().text = retryButtonLabel;
    }

    private void ShowFeedback(string message, string hint)
    {
        promptText.text = message;
        hintText.text = hint;
    }

    private string GetCurrentStageTitle()
    {
        if (currentStage != null && !string.IsNullOrEmpty(currentStage.Title))
            return currentStage.Title;

        return title;
    }

    private bool ValidateUiReferences()
    {
        bool valid = titleText != null
            && promptText != null
            && hintText != null
            && primaryButton != null
            && hintButton != null
            && retryButton != null
            && stageRoot != null;

        if (!valid)
            Debug.LogError("MeiosisGameController UI references are incomplete. Use the Stage_Root prefab or assign all UI fields.", this);

        return valid;
    }
}
