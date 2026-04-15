using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rank_Star : MonoBehaviour
{
    [Header("랭크 텍스트")]
    [SerializeField] private TextMeshProUGUI rankText;

    [Header("별 프론트 이미지 리스트")]
    [SerializeField] private List<GameObject> starImages = new List<GameObject>();

    void Update()
    {
        if (rankText == null) return;

        int rank = 0;

        // 텍스트에서 숫자 파싱
        int.TryParse(rankText.text, out rank);

        UpdateStars(rank);
    }

    void UpdateStars(int rank)
    {
        for (int i = 0; i < starImages.Count; i++)
        {
            if (starImages[i] == null) continue;

            // 랭크 수만큼 앞에서부터 활성화
            starImages[i].SetActive(i < rank);
        }
    }
}