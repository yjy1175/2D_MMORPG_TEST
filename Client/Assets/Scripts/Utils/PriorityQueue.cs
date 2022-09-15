using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> heap = new List<T>();

    public void Push(T data)
    {
        // 힙의 맨 끝에 새로운 데이터를 삽입한다
        heap.Add(data);

        int _now = heap.Count - 1;
        // 도장깨기를 시작
        while (_now > 0)
        {
            // 도장깨기를 시도
            int _next = (_now - 1) / 2;
            if (heap[_now].CompareTo(heap[_next]) < 0)
                break; // 실패

            // 두 값을 교체한다
            T _temp = heap[_now];
            heap[_now] = heap[_next];
            heap[_next] = _temp;

            // 검사 위치를 이동한다
            _now = _next;
        }
    }

    public T Pop()
    {
        // 반환할 데이터를 따로 저장
        T _ret = heap[0];

        // 마지막 데이터를 루트로 이동한다
        int _lastIndex = heap.Count - 1;
        heap[0] = heap[_lastIndex];
        heap.RemoveAt(_lastIndex);
        _lastIndex--;

        // 역으로 내려가는 도장깨기 시작
        int _now = 0;
        while (true)
        {
            int _left = 2 * _now + 1;
            int _right = 2 * _now + 2;

            int _next = _now;
            // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
            if (_left <= _lastIndex && heap[_next].CompareTo(heap[_left]) < 0)
                _next = _left;
            // 오른값이 현재값(왼쪽 이동 포함)보다 크면, 오른쪽으로 이동
            if (_right <= _lastIndex && heap[_next].CompareTo(heap[_right]) < 0)
                _next = _right;

            // 왼쪽/오른쪽 모두 현재값보다 작으면 종료
            if (_next == _now)
                break;

            // 두 값을 교체한다
            T temp = heap[_now];
            heap[_now] = heap[_next];
            heap[_next] = temp;
            // 검사 위치를 이동한다
            _now = _next;
        }

        return _ret;
    }

    public int Count { get { return heap.Count; } }
}
