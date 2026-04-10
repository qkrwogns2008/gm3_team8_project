using UnityEngine;


[CreateAssetMenu(menuName = "Gacha/Unit", fileName = "GachaDataSO_")]
public class CGachaDataSO : ScriptableObject
{
    public enum ERarity
    {
        Normal = 0,                 // 노멀
        Rare = 1,                   // 레어
        Epic = 2,                   // 에픽
        Unique = 3,                 // 유니크
        Legend = 4,                 // 레전드
    }

    [Header("유닛 정보")]
    public EHeroID HeroID;              // 영웅 ID 번호
    public string UnitName;         // 유닛 이름
    public ERarity Rarity;          // 레어도
    public float Weight;            // 뽑힐 확률 - 가중치 (노멀: 58.9%, 레어: 24%, 에픽: 14%, 유니크: 3%, 레전드: 0.1%)

    [Header("유닛 초상화")]
    public Sprite UnitIcon;         // 유닛 초상화 아이콘
    public Sprite UnitBackground;   // 등급별 카드 배경
    public Sprite UnitBorder;       // 등급별 카드 테두리
}
