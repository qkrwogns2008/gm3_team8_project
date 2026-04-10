using System;
using System.Collections.Generic;
using UnityEngine;

public enum EBuffOverwriteRule
{
	None,
	Overwrite,
}

[Flags]
public enum EBuffFlags
{
	None = 0,
	CriticalChanceBoost = 1 << 0,
	StackGuard = 1 << 1,
}

[Serializable]
public class BuffInfo
{
	#region 인스펙터
	[SerializeField] private EBuffFlags _type;
	[SerializeField] private float _value;
	[SerializeField] private float _duration; // -1 : 무한 지속
	[SerializeField] private CUnitBase _provider;
	#endregion

	#region 프로퍼티
	public bool IsDurationEnd => (_duration != -1) && (_duration <= 0);
	public EBuffFlags Type => _type;
	public float Value
	{
		get => _value;
		set => _value = value;
	}
	public float Duration
	{
		get => _duration;
		set => _duration = value;
	}
	public CUnitBase Provider => _provider;
	#endregion

	public BuffInfo(EBuffFlags type, float value, float duration, CUnitBase provider)
	{
		_type = type;
		_value = value;
		_duration = duration;
		_provider = provider;
	}
}

public class BuffSystem : MonoBehaviour
{
	#region 인스펙터
	[SerializeField] private List<BuffInfo> _buffInfos = new List<BuffInfo>(); // 적용중인 모든 버프 정보
	#endregion

	#region 내부 변수
	private EBuffFlags _currentBuffFlags = EBuffFlags.None; // 현재 적용중인 버프 종류
	#endregion

	public EBuffFlags CurrentBuffFlags => _currentBuffFlags;
	public event Action<EBuffFlags> OnBuffChanged; // 버프 변화를 알림

	private EBuffOverwriteRule GetOverwriteRule(EBuffFlags type)
	{
		switch (type)
		{
			case EBuffFlags.None:
			case EBuffFlags.CriticalChanceBoost:
				return EBuffOverwriteRule.None;

			case EBuffFlags.StackGuard:
				return EBuffOverwriteRule.Overwrite;

			default:
				return EBuffOverwriteRule.None;
		}
	}

	private void Update()
	{
		UpdateBuffDuration();
	}

	// 버프 등록
	public void AddBuff(EBuffFlags type, float value, float duration, CUnitBase provider)
	{
		EBuffOverwriteRule rule = GetOverwriteRule(type);

		// 중복 금지 버프면 이전 버프 제거
		if (rule == EBuffOverwriteRule.Overwrite)
		{
			for (int i = _buffInfos.Count - 1; i >= 0; i--)
			{
				if (_buffInfos[i].Type == type)
				{
					_buffInfos.RemoveAt(i);
				}
			}
		}
		else
		{
			// 동일 버프면 갱신
			for (int i = 0; i < _buffInfos.Count; i++)
			{
				if (_buffInfos[i].Provider == provider &&
					_buffInfos[i].Type == type)
				{
					_buffInfos[i].Duration = duration;

					if (_buffInfos[i].Value < value)
					{
						_buffInfos[i].Value = value;
					}

					UpdateBuffFlags(type);
					return;
				}
			}
		}

		// 버프 부여
		BuffInfo buff = new BuffInfo(type, value, duration, provider);
		_buffInfos.Add(buff);

		UpdateBuffFlags(type);
	}

	/// <summary>
	/// 특정 타입의 버프를 모두 제거합니다.
	/// </summary>
	public void RemoveBuffByFlags(EBuffFlags type)
	{
		for (int i = _buffInfos.Count - 1; i >= 0; i--)
		{
			if (_buffInfos[i].Type == type)
			{
				_buffInfos.RemoveAt(i);
			}
		}

		UpdateBuffFlags(type);
	}

	/// <summary>
	/// 특정 시전자가 제공하는 모든 버프를 제거합니다. 시전자가 제거될 때 패시브 버프들에 적용됩니다.
	/// </summary>
	/// <param name="provider">버프 시전자</param>
	public void RemoveBuffByProvider(CUnitBase provider)
	{
		EBuffFlags removedFlags = EBuffFlags.None;

		for (int i = _buffInfos.Count - 1; i >= 0; i--)
		{
			if (_buffInfos[i].Provider == provider &&
				_buffInfos[i].Duration == -1)
			{
				removedFlags |= _buffInfos[i].Type;
				_buffInfos.RemoveAt(i);
			}
		}

		if (removedFlags != EBuffFlags.None)
		{
			UpdateBuffFlags(EBuffFlags.None);

			// 제거할 버프에 가드 스택이 포함되면 함께 제거
			if ((removedFlags & EBuffFlags.StackGuard) != 0)
			{
				OnBuffChanged?.Invoke(EBuffFlags.StackGuard);
			}
		}
	}

	/// <summary>
	/// 적용 중인 모든 버프를 제거합니다.
	/// </summary>
	public void RemoveBuffAll()
	{
		_buffInfos.Clear();

		UpdateBuffFlags(EBuffFlags.None);

		// 전체 초기화에 적용되지 않는 옵션 추가로 갱신
		UpdateBuffFlags(EBuffFlags.StackGuard);
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
				EBuffFlags type = _buffInfos[i].Type;

				_buffInfos.RemoveAt(i);

				UpdateBuffFlags(type);
			}
		}
	}

	// 버프 플래그 갱신, 유닛에게 전송
	private void UpdateBuffFlags(EBuffFlags type)
	{
		_currentBuffFlags = EBuffFlags.None;

		for (int i = 0; i < _buffInfos.Count; i++)
		{
			_currentBuffFlags |= _buffInfos[i].Type;
		}

		OnBuffChanged?.Invoke(type);
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