using UnityEngine;

public class AddColliders : MonoBehaviour
{
    void Start()
    {
        // Получаем все дочерние объекты модели, включая её саму
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            // Проверяем, нет ли уже коллайдера
            if (meshFilter.gameObject.GetComponent<Collider>() == null)
            {
                // Добавляем Mesh Collider, если его нет
                MeshCollider collider = meshFilter.gameObject.AddComponent<MeshCollider>();
                collider.convex = false; // Опция Convex, оставляем false для статических объектов
            }
        }
    }
}
