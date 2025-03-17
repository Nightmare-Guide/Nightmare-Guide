using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder { 
    [System.Serializable]
    public class Blackboard
    {
        public Vector3 moveToPosition;
        public bool isDetected = false; // 적을 발견했는지 여부

        // 데이터를 저장할 Dictionary
        private Dictionary<string, object> data = new();

        // ⭐ 인덱서 사용 → blackboard["isDetected"] = true; 형태로 저장 가능
        public object this[string key]
        {
            get => data.TryGetValue(key, out var value) ? value : null;
            set => data[key] = value;
        }

        // 특정 타입으로 변환하여 가져오기
        public T Get<T>(string keyName)
        {
            return data.TryGetValue(keyName, out var value) && value is T typedValue ? typedValue : default;
        }

        // 데이터 저장 메서드 (제네릭)
        public void Set<T>(string keyName, T value)
        {
            data[keyName] = value;
        }
    }
}