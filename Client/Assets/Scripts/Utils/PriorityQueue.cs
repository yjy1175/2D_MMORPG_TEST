using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T>
{
    List<T> heap = new List<T>();

    public void Push(T data)
    {
        // ���� �� ���� ���ο� �����͸� �����Ѵ�
        heap.Add(data);

        int _now = heap.Count - 1;
        // ������⸦ ����
        while (_now > 0)
        {
            // ������⸦ �õ�
            int _next = (_now - 1) / 2;
            if (heap[_now].CompareTo(heap[_next]) < 0)
                break; // ����

            // �� ���� ��ü�Ѵ�
            T _temp = heap[_now];
            heap[_now] = heap[_next];
            heap[_next] = _temp;

            // �˻� ��ġ�� �̵��Ѵ�
            _now = _next;
        }
    }

    public T Pop()
    {
        // ��ȯ�� �����͸� ���� ����
        T _ret = heap[0];

        // ������ �����͸� ��Ʈ�� �̵��Ѵ�
        int _lastIndex = heap.Count - 1;
        heap[0] = heap[_lastIndex];
        heap.RemoveAt(_lastIndex);
        _lastIndex--;

        // ������ �������� ������� ����
        int _now = 0;
        while (true)
        {
            int _left = 2 * _now + 1;
            int _right = 2 * _now + 2;

            int _next = _now;
            // ���ʰ��� ���簪���� ũ��, �������� �̵�
            if (_left <= _lastIndex && heap[_next].CompareTo(heap[_left]) < 0)
                _next = _left;
            // �������� ���簪(���� �̵� ����)���� ũ��, ���������� �̵�
            if (_right <= _lastIndex && heap[_next].CompareTo(heap[_right]) < 0)
                _next = _right;

            // ����/������ ��� ���簪���� ������ ����
            if (_next == _now)
                break;

            // �� ���� ��ü�Ѵ�
            T temp = heap[_now];
            heap[_now] = heap[_next];
            heap[_next] = temp;
            // �˻� ��ġ�� �̵��Ѵ�
            _now = _next;
        }

        return _ret;
    }

    public int Count { get { return heap.Count; } }
}
