using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainNotification : MonoBehaviour
{
    public static MainNotification Instance { get; private set; }
    [SerializeField] float _fadeDuration = 1f;
    [SerializeField] float _stayDuration = 2f;
    [SerializeField] TextMeshProUGUI _textUI;
    private CanvasGroup _canvasGroup;
     void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        if (_canvasGroup == null)
        {
            Debug.LogError("CanvasGroup 컴포넌트가 없습니다. MainNotification 오브젝트에 CanvasGroup을 추가해주세요.");
        }
        if (_canvasGroup != null)
        {
            SetAlpha(0f);
        }
        else
        {
            Debug.LogError("CanvasGroup 컴포넌트가 누락되었습니다!");
        }
    }
    public void StartMainNotification(string text)
    {

        if (_textUI != null)
            _textUI.text = text;
        StopAllCoroutines();
        StartCoroutine(FadeInOutRoutine());
    }
    private IEnumerator FadeInOutRoutine()
    {
        // Fade In (1초)
        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer/ _fadeDuration);
            yield return null;
        }
        SetAlpha(1f);

        // 대기 (2초)
        yield return new WaitForSeconds(_stayDuration);

        // Fade Out (1초)
        timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            yield return null;
        }
        SetAlpha(0f);
    }
    private void SetAlpha(float alpha)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = alpha;
        // 완전히 불투명(1)할 때만 클릭을 막고, 그 외엔 통과시킴
        _canvasGroup.interactable = (alpha >= 1f);
        _canvasGroup.blocksRaycasts = (alpha >= 1f);
    }
}
