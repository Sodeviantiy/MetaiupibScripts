using UnityEngine;

public class ColliderMover : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 1, 0); // Смещение центра коллайдера

    void Start()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            col.center = offset;
            Debug.Log("Коллайдер смещён на: " + offset);
        }
        else
        {
            Debug.LogError("На объекте нет CapsuleCollider!");
        }
    }
}
