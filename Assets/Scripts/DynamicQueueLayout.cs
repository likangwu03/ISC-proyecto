using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[ExecuteAlways]
public class DynamicQueueLayout : MonoBehaviour
{
    [Header("Base")]
    public Transform origin;

    [Header("Area (zigzag zone)")]
    public BoxCollider area;

    [Header("Settings")]
    public float spacing = 1.25f;
    public int zigzagRowLength = 5;
    public Vector3 right = Vector3.right;
    public Vector3 forward = Vector3.forward;

    [Header("Overflow direction (straight line)")]
    public Vector3 overflowDirection = Vector3.back;

    [Header("Preview")]
    public int previewCount = 25;

    private List<Vector3> allPositions;
    private Dictionary<int, GameObject> occupied = new Dictionary<int, GameObject>();
    private int last = 0;



    public List<Vector3> Generate(int count)
    {
        List<Vector3> result = new List<Vector3>();

        if (origin == null) return result;

        Bounds b = area.bounds;

        int i = 0;

        // -------------------------
        // 1. ZIGZAG INSIDE AREA
        // -------------------------
        while (i < count)
        {
            int localIndex = i;
            int row = localIndex / zigzagRowLength;
            int col = localIndex % zigzagRowLength;

            bool reverse = (row % 2 == 1);
            if (reverse)
                col = (zigzagRowLength - 1) - col;
            Vector3 pos =
                origin.position +
                forward * (row * spacing) +
                right * (col * spacing);

            // si está dentro del área → zigzag válido
            if (b.Contains(pos))
            {
                result.Add(pos);
                i++;
            }
            else
            {
                break; // salimos al overflow
            }
        }

        // guardar último punto válido
        Vector3 last = result.Count > 0 ? result[result.Count - 1] : origin.position;

        // -------------------------
        // 2. OVERFLOW LINEAR
        // -------------------------
        while (i < count)
        {
            last += overflowDirection.normalized * spacing;
            result.Add(last);
            i++;
        }

        return result;
    }

    private void Start()
    {
        allPositions = Generate(previewCount);

    }

    public int Enqueue(GameObject obj)
    {
        if (last < 0 || last >= allPositions.Count)
            return -1;

        occupied[last] = obj;
        last++;
        return last-1;
    }

    public GameObject Leave(int index)
    {
        if (occupied.ContainsKey(index))
        {
            GameObject agent = occupied[index];
            occupied[index] = null;
            return agent;
        }
        return null;
    }

    public Vector3 GetPosition(int index)
    {
        if (index < 0 || index >= allPositions.Count)
            return origin.position;
        return allPositions[index];
    }

    public bool IsFree(int index)
    {
        return !occupied.ContainsKey(index) || occupied[index]==null;
    }

    public int advance(int index)
    {
        if (!IsFree(index-1)) return index;
        if (last - 1 == index) last = index;
        occupied[index-1] = occupied[index];
        occupied[index] = null;
        return index-1;
    }

    

    private void OnDrawGizmosSelected()
    {
        if (origin == null || area == null) return;

        var points = Generate(previewCount);

        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = i == 0 ? Color.green : Color.cyan;
            Gizmos.DrawSphere(points[i], 0.2f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(points[i] + Vector3.up * 0.2f, i.ToString());
#endif
        }

        Gizmos.color = Color.yellow;
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i - 1], points[i]);
        }
    }
}
