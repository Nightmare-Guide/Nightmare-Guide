using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CSVRoad_Story))]
public class CSVRoadStoryEditor : Editor
{
    // Foldout ���¸� ������ �迭 (é�ͺ��� ����/���� ���� ����)
    private bool[] foldoutStates;

    public override void OnInspectorGUI()
    {
        // �⺻ Inspector ����
        DrawDefaultInspector();

        // ��ũ��Ʈ ����
        CSVRoad_Story script = (CSVRoad_Story)target;

        // é�� ����
        int[] chapterCounts = { 3, 16 }; // é�� 0�� 4��, é�� 1�� 16��

        // Foldout ���� �ʱ�ȭ
        if (foldoutStates == null || foldoutStates.Length != chapterCounts.Length)
        {
            foldoutStates = new bool[chapterCounts.Length];
        }

        EditorGUILayout.LabelField("Chapters & SubChapters", EditorStyles.boldLabel);

        // �� é�ͺ��� Foldout ����
        for (int chapterIndex = 0; chapterIndex < chapterCounts.Length; chapterIndex++)
        {
            foldoutStates[chapterIndex] = EditorGUILayout.Foldout(foldoutStates[chapterIndex], $"Chapter {chapterIndex}");

            if (foldoutStates[chapterIndex]) // Foldout�� ������ ���� ��ư ǥ��
            {
                int subChapterCount = chapterCounts[chapterIndex];

                // ����é�� ��ư ���� (���� 2����)
                for (int subChapterIndex = 0; subChapterIndex <= subChapterCount; subChapterIndex++)
                {
                    if (subChapterIndex % 2 == 0) GUILayout.BeginHorizontal(); // 2���� ���� ����

                    if (GUILayout.Button($"SubChapter {chapterIndex}_{subChapterIndex}", GUILayout.Height(40)))
                    {
                        script.OnSelectChapter($"{chapterIndex}_{subChapterIndex}"); // �ش� ����é�� ȣ��
                    }

                    if (subChapterIndex % 2 == 1 || subChapterIndex == subChapterCount) GUILayout.EndHorizontal(); // 2���� ���� ��
                }
            }
        }
    }
}
