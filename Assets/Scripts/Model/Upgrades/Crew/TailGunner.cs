using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using Ship;
using Upgrade;
using Arcs;

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

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(45, 0));

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
				ShotInfo shotInfo = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.Attacker.PrimaryWeapon);
				//make sure card requirements are met.
				//can't reduce defender agility past 0 and must be aux arc
				if (Combat.Defender.Agility != 0 && shotInfo.InArcByType(ArcTypes.RearAux))
                {
					Messages.ShowError ("Tail Gunner: Agility is decreased");
					Combat.Defender.Tokens.AssignCondition(typeof(Conditions.TailGunnerCondition));
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