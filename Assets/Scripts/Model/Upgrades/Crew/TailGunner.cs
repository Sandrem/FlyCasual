using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using Ship;
using Upgrade;

namespace UpgradesList
{
    public class TailGunner : GenericUpgrade
    {
		public TailGunner() : base()
        {
            Types.Add(UpgradeType.Crew);
			Name = "Tail Gunner";
            Cost = 2;

			isLimited = true;

            AvatarOffset = new Vector2(45, 0);

            UpgradeAbilities.Add(new Abilities.TailGunnerAbility());
        }
    }
}

namespace Abilities
{
	public class TailGunnerAbility : GenericAbility
    {
		public override void ActivateAbility()
		{
			HostShip.OnAttackStartAsAttacker += AddTailGunnerAbility;
		}

		public override void DeactivateAbility()
		{
			HostShip.OnAttackStartAsAttacker -= AddTailGunnerAbility;
		}

		public void AddTailGunnerAbility()
		{
			if (Selection.ThisShip.ShipId == HostShip.ShipId) 
			{
				//Gather shot info to determine if in rear arc
				ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
				//make sure card requirements are met.
				//can't reduce defender agility past 0 and must be aux arc
				if (Combat.Defender.Agility != 0 && shotInfo.InRearAuxArc) {
					Messages.ShowError ("Tail Gunner: Agility is decreased");
					Combat.Defender.Tokens.AssignCondition (new Conditions.TailGunnerCondition (Combat.Defender));
					Combat.Defender.ChangeAgilityBy (-1);
					Combat.Defender.OnAttackFinish += RemoveTailGunnerAbility;
				}
			}
		}

		public void RemoveTailGunnerAbility(GenericShip ship)
		{
			Messages.ShowInfo("Agility is restored");
			Combat.Defender.Tokens.RemoveCondition(typeof(Conditions.TailGunnerCondition));
			ship.ChangeAgilityBy(+1);
			ship.OnAttackFinish -= RemoveTailGunnerAbility;
		}
    }
}

namespace Conditions
{
	public class TailGunnerCondition : Tokens.GenericToken
	{
		public TailGunnerCondition(GenericShip host) : base(host)
		{
			Name = "Debuff Token";
			Temporary = false;
			Tooltip = new UpgradesList.TailGunner().ImageUrl;
		}
	}
}