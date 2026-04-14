using UnityEngine;

public class DebugHeroPickupManager : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			GetHeroCard(EHeroID.Alice);
			GetHeroCard(EHeroID.Baskin);
			GetHeroCard(EHeroID.Teo);
			GetHeroCard(EHeroID.Yeonhee);
			GetHeroCard(EHeroID.Ecila);
			GetHeroCard(EHeroID.Elga);
			GetHeroCard(EHeroID.Evan);
			GetHeroCard(EHeroID.Jak);
			GetHeroCard(EHeroID.Karon);
			GetHeroCard(EHeroID.Loto);
			GetHeroCard(EHeroID.Nami);
			GetHeroCard(EHeroID.Radgrid);
			GetHeroCard(EHeroID.Rook);
			GetHeroCard(EHeroID.Sarah);
			GetHeroCard(EHeroID.Shane);
			GetHeroCard(EHeroID.Snipper);
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