using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EBuffFlags
{
	None = 0,
	CriticalChanceBoost_Alice = 1 << 0,
}

[Serializable]
public class BuffInfo
{
	public EBuffFlags Type;
	public float Value;
	public float Duration;
	public CUnitBase Provider;

	public bool IsDurationEnd => (Duration != -1) && (Duration <= 0);

	public BuffInfo(EBuffFlags type, float value, float duration, CUnitBase provider)
	{
		Type = type;
		Value = value;
		Duration = duration;
		Provider = provider;
	}
}

public class BuffSystem : MonoBehaviour
{
	[SerializeField] private List<BuffInfo> _buffInfos = new List<BuffInfo>(); // 적용중인 모든 버프 정보
	private EBuffFlags _currentBuffFlags = EBuffFlags.None; // 현재 적용중인 버프 종류

	public EBuffFlags CurrentBuffFlags => _currentBuffFlags;
	public event Action OnBuffChanged;

	private void Update()
	{
		UpdateBuffDuration();
	}

	// 버프 등록
	public void AddBuff(EBuffFlags type, float value, float duration, CUnitBase provider)
	{
		BuffInfo buff = new BuffInfo(type, value, duration, provider);
		_buffInfos.Add(buff);

		UpdateBuffFlags();
	}

	/// <summary>
	/// 특정 시전자가 제공하는 모든 버프를 제거합니다. 시전자가 제거될 때 패시브 버프들에 적용됩니다.
	/// </summary>
	/// <param name="provider">버프 시전자</param>
	public void RemoveBuffByProvider(CUnitBase provider)
	{
		for (int i = _buffInfos.Count - 1; i >= 0; i--)
		{
			if (_buffInfos[i].Provider == provider &&
				_buffInfos[i].Duration == -1)
			{
				_buffInfos.RemoveAt(i);
			}
		}

		UpdateBuffFlags();
	}

	// 등록된 버프 지속시간 감소 처리
	private void UpdateBuffDuration()
	{
		for (int i = _buffInfos.Count - 1; i >= 0; i--)
		{
			if (_buffInfos[i].Duration == -1)
			{
				continue;
			}

			_buffInfos[i].Duration -= Time.deltaTime;

			if (_buffInfos[i].IsDurationEnd)
			{
				_buffInfos.RemoveAt(i);

				UpdateBuffFlags();
			}
		}
	}

	// 버프 플래그 갱신, 유닛에게 전송
	private void UpdateBuffFlags()
	{
		_currentBuffFlags = EBuffFlags.None;

		for (int i = 0; i < _buffInfos.Count; i++)
		{
			_currentBuffFlags |= _buffInfos[i].Type;
		}

		OnBuffChanged?.Invoke();
	}

	/// <summary>
	/// type 플래그를 가진 버프 누적 계수를 반환합니다.
	/// </summary>
	public float GetBuffEffectTotalValue(EBuffFlags type)
	{
		if ((_currentBuffFlags & type) == 0)
		{
			return 0f;
		}

		float total = 0f;

		for (int i = 0; i < _buffInfos.Count; i++)
		{
			if (_buffInfos[i].Type == type)
			{
				total += _buffInfos[i].Value;
			}
		}

		return total;
	}
}