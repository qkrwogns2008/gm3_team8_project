using UnityEngine;

public class FixBackgroundCulling : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            // 경계 상자를 아주 크게 늘려 카메라가 항상 이 물체를 시야 안에 있다고 믿게 만듭니다.
            // (숫자 100은 상황에 따라 더 늘리셔도 됩니다)
            meshFilter.mesh.bounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f));
        }

        // 만약 Sprite Renderer를 사용 중이시라면 아래 코드를 사용하세요.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Sprite의 로컬 바운드를 확장합니다.
            spriteRenderer.localBounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f));
        }
    }
}