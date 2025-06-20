﻿using UnityEngine;

public class AddColliders : MonoBehaviour
{
    void Start()
    {
        // �������� ��� �������� ������� ������, ������� � ����
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // ���������, ��� �� ��� ����������
            if (meshFilter.gameObject.GetComponent<Collider>() == null)
            {
                // ��������� Mesh Collider, ���� ��� ���
                MeshCollider collider = meshFilter.gameObject.AddComponent<MeshCollider>();
                collider.convex = false; // ����� Convex, ��������� false ��� ����������� ��������
            }
        }
    }
}
