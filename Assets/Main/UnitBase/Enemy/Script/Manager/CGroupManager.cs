using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HeroEntry
{
    public EHeroID heroID;
    public HeroDataSO dataSO;
    public GameObject prefab;
}

public class CGroupManager : MonoBehaviour
{
    public static CGroupManager instance;

    #region 인스펙터
    [Header("데이터베이스 매칭 리스트")]
    [SerializeField] private List<HeroEntry> _heroDatabase;

    [Header("영웅 배치 데이터")]
    [SerializeField] private Dictionary<int, CAutoPlayerMove> _activeHeroes = new Dictionary<int, CAutoPlayerMove>();
    [SerializeField] private Vector3[] _slotOffsets = new Vector3[16];
    #endregion
    


    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        //  DB에서 Array 읽어와서 배치하기
    }

    void CalculateOffset()
    {
        // 사이 간격
        float spacing = 1.2f;

        for(int i = 0; i<16; i++)
        {
            int col = i % 4;
            int row = i / 4;

            // 중앙 정렬
            float x = (col - 1.5f) * spacing;
            float y = (row - 1.5f) * -spacing;

            _slotOffsets[i] = new Vector3(x, y, 0);
        }
    }

    public void SetUpGroupFromDB()
    {
        int[] heroArray = CDataManager.Instance.UserData.Hero_Array;

        for(int i = 0; i < heroArray.Length;i++)
        {
            EHeroID id = (EHeroID)heroArray[i];
            if(id == EHeroID.None)
            {
                continue;
            }

            HeroEntry entry = _heroDatabase.Find(x => x.heroID == id);

            if(entry.prefab != null && entry.dataSO != null)
            {
                Vector3 spawnPos = transform.position + _slotOffsets[i];

                // 소환 (PoolManager사용)
                GameObject obj = PoolManager.Instance.Pop(entry.prefab, spawnPos, Quaternion.identity);

                /// 초기화

                ///

                CAutoPlayerMove moveScript = obj.GetComponent<CAutoPlayerMove>();
                if(moveScript != null)
                {
                    _activeHeroes.Add(i, moveScript);
                    moveScript.SetGroupTarget(spawnPos);
                }
            }


        }
    }


    private void Update()
    {
        
        // 대열 유지
        foreach(var pair in _activeHeroes)
        {
            Vector3 targetWorldPos = transform.position + _slotOffsets[pair.Key];
            pair.Value.SetGroupTarget(targetWorldPos);
        }
    }
}
