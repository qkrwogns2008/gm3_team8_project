using UnityEngine;
public class ParallaxLayerElement : MonoBehaviour
{
    [Range(-2f, 2f)]
    public float factorX; // 이 레이어의 이동 비율
    [Range(-2f, 2f)]
    public float factorY = 0.0095f; // 이 레이어의 이동 비율
}