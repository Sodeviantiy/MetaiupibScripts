using UnityEngine;

public class ColliderMover : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1, 0); // �������� ������ ����������

    void Start()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.center = offset;
            Debug.Log("��������� ������ ��: " + offset);
        }
        else
        {
            Debug.LogError("�� ������� ��� CapsuleCollider!");
        }
    }
}
