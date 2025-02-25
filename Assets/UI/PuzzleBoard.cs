using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public List<PuzzleTile> tileList;

    public List<float> neighborTileDistance; // �ٷ� �� Ÿ�ϰ��� �Ÿ�
    public Vector3 emptyTilePosition; // �� ������ ������ ��
    public bool isTileMoving = false; // Ÿ�� �̵� ������ üũ�ϴ� ����

    private void Awake()
    {
        emptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().anchoredPosition;

        // Ÿ�� ���� Ȯ�ο� bool �� �ʱ�ȭ
        tileList.ForEach(tile => tile.isCorrected = false);
        tileList[^1].isCorrected = true; // ������ index �� ���ۺ��� true ��
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine("OnSuffle");
        }
    }

    public void IsMoveTile(PuzzleTile tile)
    {
        // �� �ڸ����� �Ÿ�
        float dist = Vector3.Distance(emptyTilePosition, tile.GetComponent<RectTransform>().anchoredPosition);

        if (neighborTileDistance.Contains(dist))
        {
            Vector3 goalPosition = emptyTilePosition;

            emptyTilePosition = tile.GetComponent<RectTransform>().anchoredPosition;

            tile.OnMoveTo(goalPosition);
        }
    }

    private IEnumerator OnSuffle()
    {
        float current = 0;
        float percent = 0;
        float time = 1.5f;

        // Ÿ���� �̵� ���� �ƴϸ� ��� ����

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            for (int i = Random.Range(0, tileList.Count - 1); i < tileList.Count - 1; i++)
            {
                float dist = Vector3.Distance(emptyTilePosition, tileList[i].GetComponent<RectTransform>().anchoredPosition);

                if (neighborTileDistance.Contains(dist))
                {
                    // isTileMoving = true;

                    Vector3 goalPosition = emptyTilePosition;

                    emptyTilePosition = tileList[i].GetComponent<RectTransform>().anchoredPosition;

                    tileList[i].OnMoveTo(goalPosition);

                    break;
                }
            }
            yield return null;

        }

    }
}
