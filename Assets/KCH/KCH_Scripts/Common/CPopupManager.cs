using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CPopupManager : MonoBehaviour
{
    public static CPopupManager Instance;
    #region ÀÎ½ºÆåÅÍ
    [Header("ÆË¾÷ ÇÁ¸®Æé ¸®½ºÆ®")]
    public List<GameObject> PopupPrefabs = new List<GameObject>();

    [Header("º¸»ó µ¥ÀÌÅÍ SO")]
    public CRewardDataSO RewardDataSO;
    [SerializeField] private Transform _canvasTransform;
    #endregion

    #region ³»ºÎ º¯¼ö
    // ÆË¾÷ Ä³½Ì µñ¼Å³Ê¸®
    private Dictionary<string, GameObject> _popupDict = new Dictionary<string, GameObject>();
    #endregion


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        BuildPopupMap();
    }

    //  ÇÁ¸®ÆÕ ¸®½ºÆ® µñ¼Å³Ê¸® º¯È¯
    private void BuildPopupMap()
    {
        int count = PopupPrefabs.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = PopupPrefabs[i];

            if (prefab != null && !_popupDict.ContainsKey(prefab.name))
            {
                _popupDict.Add(prefab.name, prefab);
            }
        }
    }

    public void ShowRewardPopup(List<SQuestReward> rewards)
    {
        if (rewards == null || rewards.Count == 0) return;

        if (_canvasTransform == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                _canvasTransform = canvas.transform;
            }
        }

        string popupName = "RewardPopup";

        if (_popupDict.ContainsKey(popupName))
        {
            GameObject popup = Instantiate(_popupDict[popupName], _canvasTransform);

            popup.SetActive(true);

            CPopupRewardView view = popup.GetComponent<CPopupRewardView>();

            if (view != null)
            {
                if(rewards.Count == 1)
                {
                    Sprite icon = RewardDataSO.GetIcon(rewards[0].Type);
                    view.SetPopup(rewards[0].Title, icon, rewards[0].Amount);
                }

                else
                {
                    view.SetPopupList("¸ðµç º¸»ó È¹µæ", rewards, RewardDataSO);
                }
            }
        }
    }

}
