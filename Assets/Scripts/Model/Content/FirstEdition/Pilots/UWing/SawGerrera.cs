using ActionsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.UWing
    {
        public class SawGerrera : UWing
        {
            public SawGerrera() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Saw Gerrera",
                    6,
                    26,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SawGerreraPilotAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class SawGerreraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddSawGerreraPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSawGerreraPilotAbility;
        }

        private void AddSawGerreraPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (Combat.Attacker.Tokens.HasToken(typeof(StressToken)) || Combat.Attacker.State.HullCurrent < Combat.Attacker.State.HullMax)
            {
                Combat.Attacker.AddAvailableDiceModification(new SawGerreraPilotAction() { Host = HostShip, ImageUrl = HostShip.ImageUrl });
            }
        }

        private class SawGerreraPilotAction : FriendlyRerollAction
        {
            public SawGerreraPilotAction() : base(1, 3, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Saw Gerrera's ability";
                IsReroll = true;
            }
        }
    }
}
