using UnityEngine;
using Ship;
using System.Collections;
using System.Collections.Generic;
using SubPhases;
using Abilities;
using BoardTools;
using System.Linq;
using RuleSets;

namespace Ship
{
	namespace Z95
	{
		public class LtBlount : Z95, ISecondEditionPilot
		{
			public LtBlount() : base()
			{
				PilotName = "Lieutenant Blount";
				PilotSkill = 6;
				Cost = 17;
				IsUnique = true;
				PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

				PilotAbilities.Add(new LtBlountAbiliity());
				faction = Faction.Rebel;

				SkinName = "Red";
			}

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 30;

                PilotAbilities.RemoveAll(ability => ability is Abilities.LtBlountAbiliity);
                PilotAbilities.Add(new Abilities.SecondEdition.LtBlountAbiliitySE());

                SEImageNumber = 28;
            }
        }
	}
}

namespace Abilities
{
	public class LtBlountAbiliity : GenericAbility
	{
		public override void ActivateAbility()
		{
			HostShip.AttackIsAlwaysConsideredHit = true;
		}

		public override void DeactivateAbility()
		{
			HostShip.AttackIsAlwaysConsideredHit = false;
		}
	}
}

namespace Abilities.SecondEdition
{
    public class LtBlountAbiliitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterLtBlountAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterLtBlountAbility;
        }

        private void RegisterLtBlountAbility()
        {
            if(Combat.Attacker == HostShip)
            {
                bool isFriendlyShips = false;

                foreach(GenericShip ship in Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Enemy))
                {
                    if (ship != HostShip)
                        isFriendlyShips = true;
                }

                if(isFriendlyShips)
                {
                    HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += LtBlountAddAttackDice;
                }
            }
        }

        private void LtBlountAddAttackDice(ref int value)
        {
            value++;
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= LtBlountAddAttackDice;
        }
    }
}