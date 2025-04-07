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
        
        // 이동 후 퍼즐 정답 확인
        if (board.tileList.All(tile => tile.isCorrected))
        {
            Debug.Log("All Correct");

            // 퍼즐 성공 함수 실행
            board.UnLockedPhone();
        }

        board.isTileMoving = false; // 퍼즐 이동 끝 알림
    }

    // 부동 소수점 오차 방지 함수
    bool ApproximatelyEqual(Vector2 a, Vector2 b, float tolerance = 0.01f)
    {
        return Vector2.Distance(a, b) < tolerance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭했을 때 행동
        board.IsMoveTile(this);
    }
}
