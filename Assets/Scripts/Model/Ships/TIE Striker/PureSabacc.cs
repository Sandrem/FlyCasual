using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace TIEStriker
    {
        public class PureSabacc : TIEStriker
        {
            public PureSabacc() : base()
            {
                PilotName = "\"Pure Sabacc\"";
                PilotSkill = 6;
                Cost = 22;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new PureSabaccAbility());
            }
        }
    }
}

namespace Abilities
{
    public class PureSabaccAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckPureSabaccAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckPureSabaccAbility;
        }

        private void CheckPureSabaccAbility(ref int value)
        {
            if (HostShip.Damage.DamageCards.Count <= 1) value++;
        }
    }
}
