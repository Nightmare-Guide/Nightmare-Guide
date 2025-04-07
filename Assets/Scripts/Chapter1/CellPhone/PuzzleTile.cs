using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PuzzleTile : MonoBehaviour, IPointerClickHandler
{
    public Vector2 corretPos;
    public bool isCorrected;
    public float dist;

    private RectTransform rectTransform;
    public PuzzleBoard board;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // corretPos = rectTransform.anchoredPosition;
        dist = Vector3.Distance(corretPos, board.emptyTilePosition);
    }

    public void OnMoveTo(Vector2 end)
    {
        StartCoroutine(MoveTo(end));
    }

    private IEnumerator MoveTo(Vector2 end)
    {
        float current = 0;
        float percent = 0;
        float moveTime = 0.1f;
        Vector2 start = rectTransform.anchoredPosition;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            rectTransform.anchoredPosition = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        rectTransform.anchoredPosition = end;

        isCorrected = ApproximatelyEqual(corretPos, rectTransform.anchoredPosition);
        
        // �̵� �� ���� ���� Ȯ��
        if (board.tileList.All(tile => tile.isCorrected))
        {
            Debug.Log("All Correct");

            // ���� ���� �Լ� ����
            board.UnLockedPhone();
        }

        board.isTileMoving = false; // ���� �̵� �� �˸�
    }

    // �ε� �Ҽ��� ���� ���� �Լ�
    bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ������ �� �ൿ
        board.IsMoveTile(this);
    }
}
