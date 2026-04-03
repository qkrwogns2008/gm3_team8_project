using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOut : MonoBehaviour
{
    public GameObject image1;
    public GameObject image2;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                HeroSelectUI.DeselectAll(image1, image2);
            }
        }
    }
}