using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public struct HeroUIEntry
{
    public EHeroID heroID;
    public SkeletonGraphic UIprefab;
}

public class HeroArrayUI : MonoBehaviour
{
    public static HeroArrayUI instance;

    #region 인스펙터
    [SerializeField] private float _spacing = 40f; // UI 영웅 간격
    [Header("데이터베이스 매칭 리스트")]
    [SerializeField] private List<HeroUIEntry> _heroDatabase;

    
    private Vector3[] _slotOffsets;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(CoWaitAndSetup());
    }

    IEnumerator CoWaitAndSetup()
    {
        yield return new WaitUntil(() => CDataManager.Instance != null && CDataManager.Instance.UserData != null);
        SetUpUIGroupFromDB();
    }

    private void CalculateOffset()
    {
        {
            _slotOffsets = new Vector3[CDataManager.Instance.UserData.Hero_Array.Length];
        }
        // 사이 간격

        for (int i = 0; i < CDataManager.Instance.UserData.Hero_Array.Length; i++)
        {
            int col = i % 4;
            int row = i / 4;

            // 중앙 정렬
            float x = (col - 1.5f) * _spacing;
            float y = (row - 1.5f) * -_spacing;

            _slotOffsets[i] = new Vector3(x, y, 0);
        }
    }

    public void SetUpUIGroupFromDB()
    {
        CalculateOffset();

        int[] heroArray = CDataManager.Instance.UserData.Hero_Array;
        for (int i = 0; i < _heroDatabase.Count; i++)
        {
            _heroDatabase[i].UIprefab.gameObject.SetActive(false);
            _heroDatabase[i].UIprefab.Initialize(false);

        }
            for (int i = 0; i < heroArray.Length; i++)
        {
            EHeroID id = (EHeroID)heroArray[i];
            if (id == EHeroID.None)
            {
                continue;
            }

            HeroUIEntry entry = _heroDatabase.Find(x => x.heroID == id);

            if (entry.UIprefab != null)
            {
                entry.UIprefab.gameObject.SetActive(true);
                entry.UIprefab.Initialize(true);

                Vector3 spawnPos = transform.position + _slotOffsets[i];
                entry.UIprefab.rectTransform.anchoredPosition = (Vector2)_slotOffsets[i];



            }


        }
    }

    


    private void Update()
    {

    }
}
