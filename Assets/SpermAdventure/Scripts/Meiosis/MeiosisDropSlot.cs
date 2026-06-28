using UnityEngine;

public class MeiosisDropSlot : MonoBehaviour
{
    [Header("染色體放置設定")]
    [SerializeField] private ChromosomeColor expectedColor = ChromosomeColor.Blue;
    [SerializeField] private ChromosomeLength expectedLength = ChromosomeLength.Long;

    [Header("是否需要成對")]
    [SerializeField] private bool needPair;
    [SerializeField] private bool requireExactColor = true;
    [SerializeField] private RectTransform snapTarget;

    private bool occupied;

    public bool Occupied => occupied;
    public RectTransform RectTransform { get; private set; }
    public RectTransform SnapTarget => snapTarget != null ? snapTarget : RectTransform;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        if (snapTarget == null)
            snapTarget = RectTransform;
    }

    public bool TryAccept(MeiosisDraggable draggable)
    {
        if (occupied)
            return false;

        if (draggable.Length != expectedLength || draggable.IsPair != needPair)
            return false;

        if (requireExactColor && draggable.ColorType != expectedColor)
            return false;

        occupied = true;
        return true;
    }
}
