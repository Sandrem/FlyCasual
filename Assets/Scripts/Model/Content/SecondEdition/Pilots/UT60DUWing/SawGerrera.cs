﻿using Abilities.SecondEdition;
using ActionsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class SawGerrera : UT60DUWing
        {
            public SawGerrera() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Saw Gerrera",
                    4,
                    52,
                    isLimited: true,
                    abilityType: typeof(SawGerreraPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    seImageNumber: 55
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.SecondEdition
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
                Combat.Attacker.AddAvailableDiceModification(new SawGerreraPilotAction() { HostShip = HostShip, ImageUrl = HostShip.ImageUrl });
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

namespace Abilities.SecondEdition
{
    public class SawGarreraPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddSawGarreraAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSawGarreraAbility;
        }

        private void AddSawGarreraAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new SawGarreraAction() { HostShip = this.HostShip });
        }

        private class SawGarreraAction : FriendlyRerollAction
        {
            public SawGarreraAction() : base(1, 2, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Saw Garrera's ability";
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.Attacker.Damage.IsDamaged())
                    return base.IsDiceModificationAvailable();
                else
                    return false;
            }
        }
    }
}
