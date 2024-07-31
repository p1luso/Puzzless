using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private PuzzleCreator puzzleCreator;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private DraggablePiece targetPiece;
    public static int movementCounter = 0;
    private bool isDragging = false;
    private float scaleIncrease = 1.2f; // Adjust the scale increase as needed

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
    }

    public void Initialize(PuzzleCreator creator)
    {
        puzzleCreator = creator;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.localPosition;
        isDragging = true;
        rectTransform.localScale = originalScale; // Ensure the scale is reset at the start of dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rectTransform.position = globalMousePos;
        }

        // Detect potential target piece to swap with
        targetPiece = null;
        foreach (var piece in puzzleCreator.Pieces)
        {
            if (piece != this)
            {
                RectTransform pieceRect = piece.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(pieceRect, eventData.position, eventData.pressEventCamera))
                {
                    targetPiece = piece;
                    return;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (targetPiece != null)
        {
            SwapPositionWith(targetPiece);
            ScoreManager.Instance.RegisterMove(); // Register the move in ScoreManager
        }
        else
        {
            rectTransform.localPosition = originalPosition;
        }
        rectTransform.localScale = originalScale; // Ensure the scale is reset when dragging ends
    }

    public void SwapPositionWith(DraggablePiece otherPiece)
    {
        movementCounter++;
        Vector3 tempPosition = rectTransform.localPosition;
        rectTransform.localPosition = otherPiece.rectTransform.localPosition;
        otherPiece.rectTransform.localPosition = originalPosition;
        puzzleCreator.PlaySlideSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
        {
            rectTransform.localScale = originalScale * scaleIncrease; // Increase size when hovered
            rectTransform.SetAsLastSibling(); // Move to the top of the hierarchy
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            rectTransform.localScale = originalScale; // Revert to original size when not hovered
        }
    }
}
