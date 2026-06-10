using UnityEngine;
using UnityEngine.EventSystems;

public class MeiosisDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("是否為精子細胞")]
    [SerializeField] private bool isSpermCell;
    [Header("染色體拖曳設定")]
    [SerializeField] private ChromosomeColor colorType;
    [SerializeField] private ChromosomeLength length;
    [SerializeField] private bool isPair;

    private MeiosisGameController controller;
    private Vector2 pointerOffset;

    public RectTransform RectTransform { get; private set; }
    public bool IsSpermCell => isSpermCell;
    public ChromosomeColor ColorType => colorType;
    public ChromosomeLength Length => length;
    public bool IsPair => isPair;
    public bool IsLocked { get; private set; }

    public void Bind(MeiosisGameController owner)
    {
        controller = owner;
        ReadChromosomeView();
    }

    public void LockTo(Vector2 anchoredPosition, float rotation)
    {
        IsLocked = true;
        RectTransform.anchoredPosition = anchoredPosition;
        RectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
    }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        ReadChromosomeView();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsLocked)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsLocked)
            return;

        RectTransform parent = RectTransform.parent as RectTransform;
        if (parent == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        RectTransform.anchoredPosition = localPoint - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsLocked)
            return;

        if (controller != null)
            controller.NotifyDragReleased(this);
    }

    private void ReadChromosomeView()
    {
        MeiosisChromosomeView chromosomeView = GetComponent<MeiosisChromosomeView>();
        if (chromosomeView == null)
            return;

        colorType = chromosomeView.ColorType;
        length = chromosomeView.Length;
        isPair = chromosomeView.IsPair;
    }
}
