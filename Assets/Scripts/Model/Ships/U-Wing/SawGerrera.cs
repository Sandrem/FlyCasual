using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using ActionsList;
using Tokens;
using Ship;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace UWing
    {
        public class SawGerrera : UWing
        {
            public SawGerrera() : base()
            {
                PilotName = "Saw Gerrera";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                SkinName = "Partisan";

                PilotAbilities.Add(new SawGerreraPilotAbility());
            }
        }
    }
}

namespace Abilities
{
    public class SawGerreraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddSawGerreraPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddSawGerreraPilotAbility;
        }

        private void AddSawGerreraPilotAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (Combat.Attacker.Tokens.HasToken(typeof(StressToken)) || Combat.Attacker.Hull < Combat.Attacker.MaxHull)
            {
                Combat.Attacker.AddAvailableActionEffect(new SawGerreraPilotAction() { Host = HostShip, ImageUrl = HostShip.ImageUrl });
            }
        }

        private class SawGerreraPilotAction : FriendlyAttackRerollAction
        {
            public SawGerreraPilotAction() : base(1, 2, true)
            {
                Name = EffectName = "Saw Gerrera's ability";
                IsReroll = true;
            }
        }
    }
}
