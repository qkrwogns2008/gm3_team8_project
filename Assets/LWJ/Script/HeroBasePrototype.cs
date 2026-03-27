using Spine.Unity;
using System.Collections;
using UnityEngine;

// no use Dummy script file
public class HeroBasePrototype : MonoBehaviour
{
	/*
	해당 스크립트를 상속 받아서 영웅 캐릭터 별로 구체화 시키고 프리팹으로 묶는다.
	영웅의 수치 데이터는 SO로 주입받는 형태.
	(체력, 공격력, 공격 속도 등)
	
	- 캐릭터 상태 분리
	- Idle, Move, Attack, Skill, Dead

	- 기본 공격 기능 구현
	→ 스파인 애니메이션(공격)
	근거리 → 타겟 데미지 처리
	원거리 → 투사체 생성 → 투사체 이동 (타겟 위치)

	- 외부 주입 스탯 적용

	- 스킬 기능 구현
	→ 스킬 이펙트
	→ 스킬 쿨타임
	→ 스킬 데미지
	*/
	[SerializeField] protected string _unitName;
	[SerializeField] protected float _health;
	[SerializeField] protected float _attackDamage;
	[SerializeField] protected float _attackRange;
	[SerializeField] protected float _attackSpeed;
	[SerializeField] protected float _moveSpeed;
	//[SerializeField] protected HeroData _data;

	[SerializeField] protected SkeletonAnimation _skeletonAni;
	[SerializeField] protected GameObject _attackEffectPrefab;
	[SerializeField] protected Transform _effectPos;

	protected Vector3 _targetPos = Vector3.one;
	protected Coroutine _routine;

	public string UnitName => _unitName;
	public float Health => _health;
	public float AttackDamage => _attackDamage;
	public float AttackSpeed => _attackSpeed;
	public float MoveSpeed => _moveSpeed;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Debug.Log("F1 키 입력 감지");
			Attack();
		}
	}

	// override
	protected void Attack()
	{
		if (_targetPos == null)
		{
			Debug.Log("방어 코드 작동");

			return;
		}
		if (_routine != null)
		{
			Debug.Log("방어 코드2 작동");
			return;
		}

		Debug.Log("Attack 호출");
		_routine = StartCoroutine(Co_AttackRoutine());
	}

	protected IEnumerator Co_AttackRoutine()
	{
		_skeletonAni.AnimationState.SetAnimation(0, "Attack_A", false);

		GameObject fx = Instantiate(_attackEffectPrefab, transform.position, Quaternion.identity);

		yield return new WaitForSeconds(0.2f);
		Debug.Log("fx 파괴");
		Destroy(fx);

		_routine = null;
	}
}