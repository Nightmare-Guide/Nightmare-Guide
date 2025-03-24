using System;
using UnityEngine;
using UnityEngine.UI; // UI.Text 사용

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        public Text uiText; // UI.Text 컴포넌트 참조

        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

            // Text 컴포넌트를 반드시 연결해야 함
            if (uiText == null)
            {
                Debug.LogError("UI Text 컴포넌트가 연결되지 않았습니다!");
            }
        }

        private void Update()
        {
            // FPS 계산
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;

                // UI 텍스트에 FPS 표시
                if (uiText != null)
                {
                    uiText.text = string.Format(display, m_CurrentFps);
                }
            }
        }
    }
}
