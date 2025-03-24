using System;
using UnityEngine;
using UnityEngine.UI; // UI.Text ���

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        public Text uiText; // UI.Text ������Ʈ ����

        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

            // Text ������Ʈ�� �ݵ�� �����ؾ� ��
            if (uiText == null)
            {
                Debug.LogError("UI Text ������Ʈ�� ������� �ʾҽ��ϴ�!");
            }
        }

        private void Update()
        {
            // FPS ���
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;

                // UI �ؽ�Ʈ�� FPS ǥ��
                if (uiText != null)
                {
                    uiText.text = string.Format(display, m_CurrentFps);
                }
            }
        }
    }
}
