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
        corretPos = rectTransform.anchoredPosition;
        dist = Vector3.Distance(corretPos, board.emptyTilePosition);
    }

    public void OnMoveTo(Vector3 end)
    {
        StartCoroutine("MoveTo", end);
    }

    private IEnumerator MoveTo(Vector3 end)
    {
        float current = 0;
        float percent = 0;
        float moveTime = 0.1f;
        Vector3 start = rectTransform.anchoredPosition;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            rectTransform.anchoredPosition = Vector3.Lerp(start, end, percent);

            yield return null;
        }

        isCorrected = corretPos == rectTransform.anchoredPosition ? true : false;

        // �̵� �� ���� ���� Ȯ��
        if (board.tileList.All(tile => tile.isCorrected))
        {
            Debug.Log("All Correct");

            // ���� ���� �Լ� ����
            board.UnLockedPhone();
        }

        board.isTileMoving = false; // ���� �̵� �� �˸�
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ������ �� �ൿ
        board.IsMoveTile(this);
    }
}
