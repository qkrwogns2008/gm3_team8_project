using UnityEngine;

public class Info_Swap : MonoBehaviour
{
    [SerializeField] private GameObject objectA; // 끌 오브젝트
    [SerializeField] private GameObject objectB; // 켤 오브젝트

    public void Swap()
    {
        if (objectA != null)
            objectA.SetActive(false);

        if (objectB != null)
            objectB.SetActive(true);
    }
}