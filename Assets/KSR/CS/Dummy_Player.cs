using UnityEngine;

public class Dummy_Player : MonoBehaviour
{
    [Header("업그레이드 버튼 연결")]
    public LV_UP_Button attackButton;
    public LV_UP_Button defenseButton;
    public LV_UP_Button lifeButton;

    [Header("기본 스탯")]
    public float baseAttack;
    public float baseDefense;
    public float baseLife;

    [Header("최종 스탯(자동 계산)")]
    public float attack;
    public float defense;
    public float life;
    public float power;

    [Header("재화")]
    public float gold;

    // =============================

    void Update()
    {
        CalculateStats(); // 실시간 반영
    }

    // =============================

    void CalculateStats()
    {
        // 강화 가져오기
        float attackBonus = attackButton != null ? attackButton.currentStat : 0f;
        float defenseBonus = defenseButton != null ? defenseButton.currentStat : 0f;
        float lifeBonus = lifeButton != null ? lifeButton.currentStat : 0f;

        // 기본값 + 강화 누적값
        attack = baseAttack + attackBonus;
        defense = baseDefense + defenseBonus;
        life = baseLife + lifeBonus;

        // 전투력
        power = attack + defense + life;
    }
}