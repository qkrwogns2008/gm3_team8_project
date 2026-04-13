using UnityEngine;

public class DebugHeroPickupManager : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			GetHeroCard(EHeroID.Baskin);
			GetHeroCard(EHeroID.Alice);
			GetHeroCard(EHeroID.Teo);
			GetHeroCard(EHeroID.Elga);
			Debug.Log("Ä«µå È¹µæ");
		}
	}

	private void GetHeroCard(EHeroID heroId = EHeroID.None)
	{
		if (heroId == EHeroID.None)
		{
			return;
		}
		CDataManager.Instance.AddHeroData(heroId);
	}
}