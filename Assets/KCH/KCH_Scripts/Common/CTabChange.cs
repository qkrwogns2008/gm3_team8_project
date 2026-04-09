using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CTabChange : MonoBehaviour
{
    public event Action<int> OnTabChange;

    #region 인스펙터
    [Header("탭 버튼 리스트")]
    [SerializeField] private List<Button> _tabButtons = new List<Button>();
    #endregion

    #region 내부 변수
    private int _currentIndex = -1;
    #endregion

    public void Awake()
    {
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            int index = i;
            _tabButtons[index].onClick.AddListener(()=> SelectTab(index));
        }
    }

    public void SelectTab(int index)
    {
        if (_currentIndex == index || index < 0 || index >= _tabButtons.Count)
        {
            return;
        }

        _currentIndex = index;  

        OnTabChange?.Invoke(_currentIndex);
    }
}
