using UnityEngine;

public class Button_AddHero : MonoBehaviour
{
    public HeroList heroList;     // 영웅목록 참조
    public GameObject heroPrefab;   // 추가할 영웅 프리팹

    // 버튼 클릭 시 호출
    public void OnClickAddHero()
    {
        if (heroList == null || heroPrefab == null)
        {
            Debug.LogWarning("HeroList 또는 heroPrefab이 연결되지 않았습니다.");
            return;
        }

        // 영웅목록에 영웅 추가
        heroList.AddHero(heroPrefab);
    }
}