using UnityEngine;
public class ParallaxLayerElement : MonoBehaviour
{
    [Range(-2f, 2f)]
    public float factorX; // 이 레이어의 이동 비율
    [Range(-2f, 2f)]
    public float factorY; // 이 레이어의 이동 비율
}