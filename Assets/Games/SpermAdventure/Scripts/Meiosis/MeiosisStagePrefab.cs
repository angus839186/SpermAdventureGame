using UnityEngine;

public class MeiosisStagePrefab : MonoBehaviour
{
    [Header("提示文字")]
    [SerializeField] private string prompt;
    [SerializeField] private string hint;
    [SerializeField] private string failMessage;
    [SerializeField] private string primaryButtonLabel;

    [Header("關卡選項設定")]
    [SerializeField] private bool passWithoutSlots;
    [SerializeField] private bool hidePrimaryButton;

    public string Prompt => prompt;
    public string Hint => hint;
    public string FailMessage => failMessage;
    public string PrimaryButtonLabel => primaryButtonLabel;
    public bool PassWithoutSlots => passWithoutSlots;
    public bool HidePrimaryButton => hidePrimaryButton;
}
