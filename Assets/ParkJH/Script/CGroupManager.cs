using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;

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
    [Header("조작 설정")]
    [SerializeField] private CJoyStickInput _joystick;
    [SerializeField] private float _groupSpeed = 15f;
    [SerializeField] private float _spacing = 10f; // 영웅 간격
    [Header("데이터베이스 매칭 리스트")]
    [SerializeField] private List<HeroEntry> _heroDatabase;

    [Header("영웅 배치 데이터")]
    [SerializeField] private Dictionary<int, CAutoPlayerMove> _activeHeroes = new Dictionary<int, CAutoPlayerMove>();
    [SerializeField] private Vector3[] _slotOffsets;
    #endregion

    public bool IsJoystickActive => _joystick != null && _joystick.InputVector.sqrMagnitude > 0.01f;
    public float JoystickX => _joystick != null ? _joystick.InputVector.x : 0;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetUpGroupFromDB();
    }

   

    void CalculateOffset()
    {
        //  DB에서 Array 읽어와서 배치하기if (CDataManager.Instance != null && CDataManager.Instance.UserData != null)
        {
            _slotOffsets = new Vector3[CDataManager.Instance.UserData.Hero_Array.Length];
        }
        // 사이 간격

        for(int i = 0; i < CDataManager.Instance.UserData.Hero_Array.Length; i++)
        {
            int col = i % 4;
            int row = i / 4;

            // 중앙 정렬
            float x = (col - 1.5f) * _spacing;
            float y = (row - 1.5f) * -_spacing;

            _slotOffsets[i] = new Vector3(x, y, 0);
        }
    }

    public void SetUpGroupFromDB()
    {
        CalculateOffset();
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
                GameObject obj = PoolManager.Instance.Pop(entry.prefab, spawnPos, Quaternion.identity); // PoolManager.Pop = 오브젝트 소환

                CHero hero = obj.GetComponent<CHero>();
                if(hero != null)
                {
                    InjectHeroData(hero, entry.dataSO);
                }

                CAutoPlayerMove moveScript = obj.GetComponent<CAutoPlayerMove>();
                if(moveScript != null)
                {
                    _activeHeroes.Add(i, moveScript);
                    moveScript.SetGroupTarget(spawnPos);
                }
            }


        }
    }

    private void InjectHeroData(CHero hero, HeroDataSO data)
    {
        FieldInfo field = typeof(CUnitBase).GetField("OriginData", BindingFlags.NonPublic | BindingFlags.Instance);
        if(field != null)
        {
            field.SetValue(hero, data);
            // 초기화 메서드 강제 호출
            MethodInfo initMethod = typeof(CHero).GetMethod("InitUnitStats", BindingFlags.NonPublic | BindingFlags.Instance);
            initMethod.Invoke(hero, null);
        }
    }
   
    private void HeroGroupMoving()  // 우재님과 Moving 함수 확인해보기
    { 
        Vector3 Center = transform.position;
        foreach(var pair in _activeHeroes)
        {
            Vector3 targetWorldPos = Center + _slotOffsets[pair.Key];
            pair.Value.SetGroupTarget(targetWorldPos);
        }
    }

    private void Update()
    {
        if (IsJoystickActive)
        {
            Vector3 moveDir = new Vector3(_joystick.InputVector.x, _joystick.InputVector.y, 0);
            transform.position += moveDir * _groupSpeed * Time.deltaTime;
        }
        // 대열 유지
        HeroGroupMoving();
    }
}
