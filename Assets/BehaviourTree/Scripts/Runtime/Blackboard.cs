using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    [System.Serializable]
    public class Blackboard
    {
        public Vector3 moveToPosition;

        // 데이터를 저장할 Dictionary
        private Dictionary<string, object> data = new();

        // 기본값 설정
        public Blackboard()
        {
            data["isDetected"] = false;
            data["lockerDetected"] = false;
            data["isCollidedWithPlayer"] = false;   // 🔸 새로 추가된 충돌 상태 값
        }

        // 인덱서
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

        // 데이터 저장 메서드
        public void Set<T>(string keyName, T value)
        {
            data[keyName] = value;
        }

        // isDetected 관련
        public bool IsDetected() => Get<bool>("isDetected");
        public void UpdateDetectionStatus(bool value) => Set("isDetected", value);

        // lockerDetected 관련
        public bool LockerDetected() => Get<bool>("lockerDetected");
        public void UpdateLockerDetectionStatus(bool value) => Set("lockerDetected", value);

        // 🔸 isCollidedWithPlayer 관련
        public bool IsCollidedWithPlayer() => Get<bool>("isCollidedWithPlayer");
        public void UpdateCollisionStatus(bool value) => Set("isCollidedWithPlayer", value);
    }
}
