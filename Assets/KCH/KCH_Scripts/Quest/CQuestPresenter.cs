using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CQuestPresenter : MonoBehaviour
{
    [Header("퀘스트 프리펩 설정")]
    public GameObject QuestPrefab;      // CQuestUIView
    public Transform QuestTransform;    // 생성될 Quest의 부모

    private void Start()
    {
        // 데이터가 바뀌면 RefreshUI 실행
        CQuestManager.Instance.OnDataUpdate += RefreshUI;

        RefreshUI();
    }


    public void RefreshUI()
    {
        // 기존 슬롯 역순 삭제
        int QuestCount = QuestTransform.childCount;
        for (int i = QuestCount - 1; i >= 0; i--)
        {
            Destroy(QuestTransform.GetChild(i).gameObject);
        }

        // 매니저가 새로 생성
        var dataList = CQuestManager.Instance.QuestDataList;
        var progressList = CQuestManager.Instance.UserProgressList;

        int dataCount = dataList.Count;
        for (int i = 0; i < dataCount; i++)
        {
            // 프리펩 생성
            GameObject quest = Instantiate(QuestPrefab, QuestTransform);
            CQuestUIView questUI = quest.GetComponent<CQuestUIView>();

            if (questUI != null)
            {   
                // 리스트 인덱스로 바로 매칭
                questUI.SetUI(dataList[i], progressList[i]);
            }
        }
    }
}
