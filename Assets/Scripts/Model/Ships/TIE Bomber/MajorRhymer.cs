using System;
using System.Linq;
using ActionsList;
using RuleSets;
using Ship;
using Upgrade;

namespace Ship
{
    namespace TIEBomber
    {
        public class MajorRhymer : TIEBomber, ISecondEditionPilot
        {
            public MajorRhymer() : base()
            {
                PilotName = "Major Rhymer";
                PilotSkill = 4;
                Cost = 34;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.MajorRhymerAbility());

                SEImageNumber = 109;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorRhymerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnUpdateWeaponRange += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUpdateWeaponRange -= CheckAbility;
        } 
        
        private void CheckAbility(GenericSecondaryWeapon weapon, ref int minRange, ref int maxRange)
        {
            if (weapon.Types.Contains(UpgradeType.Missile) || weapon.Types.Contains(UpgradeType.Torpedo))
            {
                if (minRange > 0) minRange--;
                if (maxRange < 3) maxRange++;
            }
        }
    }
}
