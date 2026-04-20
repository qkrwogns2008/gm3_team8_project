using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RubyTextUI : MonoBehaviour
{
    [Header("루비 텍스트들")]
    public List<TMP_Text> rubyTexts = new List<TMP_Text>(); // 여러 UI 연결

    void Update()
    {
        if (CDataManager.Instance == null) return; // 데이터매니저 체크

        int ruby = CDataManager.Instance.UserData.Ruby; // 루비 값 가져오기

        // 모든 텍스트 갱신
        for (int i = 0; i < rubyTexts.Count; i++)
        {
            if (rubyTexts[i] == null) continue;

            rubyTexts[i].text = ruby.ToString(); // 값 표시
        }
    }
}