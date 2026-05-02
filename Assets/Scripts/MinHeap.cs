using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MinHeap<T> where T : IComparable<T>
{
    private readonly List<T> _data = new List<T>();

    public int Count => _data.Count;

    public void Push(T item)
    {
        _data.Add(item);
        BubbleUp(_data.Count - 1);
    }

    public T Pop()
    {
        T top = _data[0];
        int last = _data.Count - 1;
        _data[0] = _data[last];
        _data.RemoveAt(last);
        if (_data.Count > 0) SiftDown(0);
        return top;
    }

    private void BubbleUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (_data[i].CompareTo(_data[parent]) >= 0) break;
            (_data[i], _data[parent]) = (_data[parent], _data[i]);
            i = parent;
        }
    }

    private void SiftDown(int i)
    {
        int n = _data.Count;
        while (true)
        {
            int smallest = i, l = 2 * i + 1, r = 2 * i + 2;
            if (l < n && _data[l].CompareTo(_data[smallest]) < 0) smallest = l;
            if (r < n && _data[r].CompareTo(_data[smallest]) < 0) smallest = r;
            if (smallest == i) break;
            (_data[i], _data[smallest]) = (_data[smallest], _data[i]);
            i = smallest;
        }
    }
}

