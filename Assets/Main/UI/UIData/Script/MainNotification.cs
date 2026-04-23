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
    [SerializeField] TextMeshProUGUI _textUI1;
    [SerializeField] TextMeshProUGUI _textUI2;
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
    public void StartMainNotification(string text,string text2, float duration)
    {

        if (_textUI1 != null)
            _textUI1.text = text;
        if (_textUI2 != null)
            _textUI2.text = text2;
        StopAllCoroutines();
        StartCoroutine(FadeInOutRoutine(duration));
    }
    private IEnumerator FadeInOutRoutine(float DurationTime)
    {
        float timer = 0f;
        /* Fade In (1초)
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer/ _fadeDuration);
            yield return null;
        }
        */
        SetAlpha(1f);
        _stayDuration = DurationTime;
        // 대기 (2초)
        yield return new WaitForSeconds(_stayDuration);

        // Fade Out (1초)
        timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float nextAlpha = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            SetAlpha(nextAlpha); // 여기서 바로 호출!
            yield return null;
        }
        SetAlpha(0f);
    }
    private void SetAlpha(float alpha)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = alpha;
        // 완전히 불투명(1)할 때만 클릭을 막고, 그 외엔 통과시킴
        _canvasGroup.interactable = (alpha > 0.001f);
        _canvasGroup.blocksRaycasts = (alpha > 0.001f);
    }
}
