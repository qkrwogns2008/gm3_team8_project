using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAlice : RangedHeroBase
{
	#region 인스펙터
	[Header("치명타 원거리 세팅")]
	[SerializeField] protected MissileBase CriticalMissilePrefab;
	[SerializeField] protected MissileDataSO CriticalMissileData;
	#endregion

	#region 내부 변수
	protected Coroutine BindCo;
	#endregion

	protected override void OnEnable()
	{
		base.OnEnable();

		if (BindCo != null)
		{
			StopCoroutine(BindCo);
		}
		BindCo = StartCoroutine(Co_WaitSubscribe());

		enableUseSkill = false; // 패시브 스킬
	}

	protected virtual IEnumerator Co_WaitSubscribe()
	{
		while (CHeroManager.Instance == null)
		{
			yield return null;
		}

		CHeroManager.Instance.OnHeroActived += OnHeroActived;
		ApplyBuffAllHero(EBuffFlags.CriticalChanceBoost, 0.16f, -1f, this);
		BindCo = null;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (BindCo != null)
		{
			StopCoroutine(BindCo);
			BindCo = null;
		}

		if (CHeroManager.Instance != null)
		{
			CHeroManager.Instance.OnHeroActived -= OnHeroActived;
			RemoveBuffAllHeroByProvider(this);
		}
	}

	// 스킬은 패시브이므로 재정의
	public override void TryAttack(CUnitBase target)
	{
		if (!IsAvailable() || target == null)
		{
			return;
		}

		// 치명타 체크
		bool isCriAttack = (Random.Range(0f, 100f) <= FinalCriticalChance);

		if (EnableCriticalAttack && isCriAttack)
		{
			ExecuteCombat(EAttackType.Critical, target);
		}
		else if (EnableAttack)
		{
			ExecuteCombat(EAttackType.Normal, target);
		}
	}

	protected override void ProcessCriticalHit(CUnitBase target)
	{
		if (CriticalMissilePrefab == null || CriticalMissileData == null)
		{
			Debug.LogWarning($"[{UnitName}] 원거리 투사체 null.");
			return;
		}

		Quaternion rot = Quaternion.Euler(-42f, 0f, 0f);

		MissileBase missile = PoolManager.Instance.Pop(CriticalMissilePrefab, CenterPos, rot);
		missile.Init(CriticalMissilePrefab, CriticalMissileData, CriticalDamage, target, this);
	}

	#region 버프 로직
	protected virtual void OnHeroActived(CUnitBase hero)
	{
		ApplyBuff(hero, EBuffFlags.CriticalChanceBoost, 0.16f, -1, this);
	}

	protected virtual void ApplyBuff(CUnitBase target, EBuffFlags buffFlags, float value, float duration, CUnitBase provider)
	{
		if (Team == ETeamType.Hero)
		{
			CHero hero = target as CHero;

			if (hero == null)
			{
				return;
			}

			hero.BuffSystem.AddBuff(buffFlags, value, duration, provider);
		}
	}

	/// <summary>
	/// 모든 영웅에게 패시브 버프를 부여합니다.
	/// </summary>
	protected virtual void ApplyBuffAllHero(EBuffFlags buffFlags, float value, float duration, CUnitBase provider)
	{
		if (Team == ETeamType.Hero)
		{
			IReadOnlyList<CUnitBase> heros = CHeroManager.Instance.ActiveHero;

			for (int i = 0; i < heros.Count; i++)
			{
				CHero hero = heros[i] as CHero;

				if (hero == null)
				{
					continue;
				}
				if (hero == this)
				{
					continue;
				}

				hero.BuffSystem.AddBuff(buffFlags, value, duration, provider);
			}
		}
	}

	/// <summary>
	/// 모든 영웅에게서 provider 개체가 부여한 패시브 버프를 제거합니다.
	/// </summary>
	protected virtual void RemoveBuffAllHeroByProvider(CUnitBase provider)
	{
		if (Team == ETeamType.Hero)
		{
			IReadOnlyList<CUnitBase> heros = CHeroManager.Instance.ActiveHero;

			for (int i = 0; i < heros.Count; i++)
			{
				CHero hero = heros[i] as CHero;

				if (hero == null)
				{
					continue;
				}

				hero.BuffSystem.RemoveBuffByProvider(provider);
			}
		}
	}
	#endregion
}