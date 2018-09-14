using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using ActionsList;
using Tokens;
using Ship;
using SubPhases;
using Upgrade;
using RuleSets;

namespace Ship
{
    namespace UWing
    {
        public class SawGerrera : UWing, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 52;

                PilotAbilities.RemoveAll(ability => ability is Abilities.SawGerreraPilotAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.SawGarreraAbilitySE());

                SEImageNumber = 55;
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
            GenericShip.OnGenerateDiceModificationsGlobal += AddSawGerreraPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSawGerreraPilotAbility;
        }

        private void AddSawGerreraPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            if (Combat.Attacker.Tokens.HasToken(typeof(StressToken)) || Combat.Attacker.Hull < Combat.Attacker.MaxHull)
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

namespace Abilities.SecondEdition
{
    public class SawGarreraAbilitySE : GenericAbility
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
            Combat.Attacker.AddAvailableDiceModification(new SawGarreraAction() { Host = this.HostShip });
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
