using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CSVRoad_Story))]
public class CSVRoadStoryEditor : Editor
{
    // Foldout 상태를 저장할 배열 (챕터별로 열림/닫힘 상태 저장)
    private bool[] foldoutStates;

    public override void OnInspectorGUI()
    {
        // 기본 Inspector 유지
        DrawDefaultInspector();

        // 스크립트 참조
        CSVRoad_Story script = (CSVRoad_Story)target;

        // 챕터 정보
        int[] chapterCounts = { 3, 16 }; // 챕터 0은 4개, 챕터 1은 16개

        // Foldout 상태 초기화
        if (foldoutStates == null || foldoutStates.Length != chapterCounts.Length)
        {
            foldoutStates = new bool[chapterCounts.Length];
        }

        EditorGUILayout.LabelField("Chapters & SubChapters", EditorStyles.boldLabel);

        // 각 챕터별로 Foldout 생성
        for (int chapterIndex = 0; chapterIndex < chapterCounts.Length; chapterIndex++)
        {
            foldoutStates[chapterIndex] = EditorGUILayout.Foldout(foldoutStates[chapterIndex], $"Chapter {chapterIndex}");

            if (foldoutStates[chapterIndex]) // Foldout이 열렸을 때만 버튼 표시
            {
                int subChapterCount = chapterCounts[chapterIndex];

                // 서브챕터 버튼 생성 (가로 2개씩)
                for (int subChapterIndex = 0; subChapterIndex <= subChapterCount; subChapterIndex++)
                {
                    if (subChapterIndex % 2 == 0) GUILayout.BeginHorizontal(); // 2개씩 묶기 시작

                    if (GUILayout.Button($"SubChapter {chapterIndex}_{subChapterIndex}", GUILayout.Height(40)))
                    {
                        script.OnSelectChapter($"{chapterIndex}_{subChapterIndex}"); // 해당 서브챕터 호출
                    }

                    if (subChapterIndex % 2 == 1 || subChapterIndex == subChapterCount) GUILayout.EndHorizontal(); // 2개씩 묶기 끝
                }
            }
        }
    }
}
