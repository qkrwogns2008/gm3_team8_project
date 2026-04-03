using UnityEngine;

public class CheckButton : MonoBehaviour
{
    // 체크 이미지
    [SerializeField] private GameObject checkImage;

    // 카드 매니저 연결
    [SerializeField] private CardManager cardManager;

    // 현재 체크 상태
    private bool isChecked = false;

    // 버튼 클릭 시 호출
    public void OnClick()
    {
        isChecked = !isChecked;

        // 체크 이미지 표시
        if (checkImage != null)
            checkImage.SetActive(isChecked);

        // 카드 표시 처리
        if (isChecked)
        {
            cardManager.ShowOwnedOnly();
        }
        else
        {
            cardManager.ShowAll();
        }
    }
}