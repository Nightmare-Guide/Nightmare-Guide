using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoard : MonoBehaviour
{
    public CellPhone cellPhone;
    public List<PuzzleTile> tileList;

    public List<float> neighborTileDistance; // �ٷ� �� Ÿ�ϰ��� �Ÿ�
    public Vector3 emptyTilePosition; // �� ������ ������ ��
    public bool canMovePuzzle = false; // ������ ��밡���� �������� Ȯ���ϴ� ����
    public bool isTileMoving = false; // Ÿ�� �̵� ������ Ȯ���ϴ� ����
    public GameObject suffleButton;

    private void Awake()
    {
        InitializationPuzzle();
    }

    // ���� �ʱ�ȭ �Լ�
    public void InitializationPuzzle()
    {
        emptyTilePosition = tileList[tileList.Count - 1].GetComponent<RectTransform>().anchoredPosition;

        // Ÿ�� ���� Ȯ�ο� bool �� �ʱ�ȭ
        tileList.ForEach(tile => tile.isCorrected = false);
        tileList[^1].isCorrected = true; // ������ index �� ���ۺ��� true ��
        tileList[^1].gameObject.SetActive(false); // Ȱ��ȭ ������ ��� ���� �������� ���� �߻�.

        foreach (PuzzleTile puzzle in tileList)
        {
            puzzle.GetComponent<RectTransform>().anchoredPosition = puzzle.corretPos;
        }

        canMovePuzzle = false;
        isTileMoving = false;
        suffleButton.SetActive(true);
    }

    public void IsMoveTile(PuzzleTile tile)
    {
        if (!canMovePuzzle)
            return;

        isTileMoving = true; // ���� ���� �̵� ����

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

        Debug.Log("Suffle 1");

        // Ÿ���� �̵� ���� �ƴϸ� ��� ����

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            Debug.Log("Suffle 2");

            for (int i = Random.Range(0, tileList.Count - 1); i < tileList.Count - 1; i++)
            {
                Debug.Log("Suffle 3");
                float dist = Vector3.Distance(emptyTilePosition, tileList[i].GetComponent<RectTransform>().anchoredPosition);

                if (neighborTileDistance.Contains(dist))
                {
                    isTileMoving = true;

                    Vector3 goalPosition = emptyTilePosition;

                    emptyTilePosition = tileList[i].GetComponent<RectTransform>().anchoredPosition;

                    tileList[i].OnMoveTo(goalPosition);

                    Debug.Log("Suffle 4");
                    break;
                }
            }
            yield return null;
        }

        // ���� ��� ����
        canMovePuzzle = true;
    }

    public void SufflePuzzle() // ��ư���� ���
    {
        suffleButton.SetActive(false);
        StartCoroutine("OnSuffle");

        // �޴��� �ð�, ��¥, �����̵�� �� ��Ȱ��ȭ
        cellPhone.sliderUI.SetActive(false);
        cellPhone.timeText.gameObject.SetActive(false);
        cellPhone.dateText.gameObject.SetActive(false);
    }

    // ��� ���� �� ��ȭ
    public void UnLockedPhone()
    {
        canMovePuzzle = false;
        cellPhone.unLocked = true;
        SchoolUIManager.instance.SetUIOpacity(cellPhone.puzzleUI[cellPhone.puzzleUI.Length - 1], true, 1f, 0.2f); // ������ ���� ���� ���� ����

        // ���� UI õõ�� �����
        for (int i = 0; i < cellPhone.puzzleUI.Length; i++)
        {
            SchoolUIManager.instance.SetUIOpacity(cellPhone.puzzleUI[i], false, 0.5f, 3f);
        }

        // App Screen UI Ȱ��ȭ
        cellPhone.appScreenUI.SetActive(true);

        foreach (Image img in cellPhone.appScreenImgs)
        {
            SchoolUIManager.instance.SetUIOpacity(img, true, 1f, 3.1f);
        }
        foreach (TextMeshProUGUI text in cellPhone.appScreenTexts)
        {
            SchoolUIManager.instance.SetUIOpacity(text, true, 1f, 3.1f);
        }
    }
}
