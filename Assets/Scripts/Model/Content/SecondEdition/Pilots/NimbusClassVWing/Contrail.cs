using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class Contrail : NimbusClassVWing
        {
            public Contrail() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Contrail\"",
                    5,
                    33,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ContrailAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/bc/b0bcccdb-fd02-4ab1-847b-66bae01e7ddc/swz80_ship_contrail.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ContrailAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                "\"Contrail\"",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            return ManeuversHaveTheSameBearing()
                && Combat.CurrentDiceRoll.HasResult(DieSide.Focus);
        }

        private bool ManeuversHaveTheSameBearing()
        {
            bool result = false;

            GenericShip anotherShip = GetAnotherShip();
            if (HostShip.RevealedManeuver != null && anotherShip != null && anotherShip.RevealedManeuver != null)
            {
                if (HostShip.RevealedManeuver.Bearing == anotherShip.RevealedManeuver.Bearing) result = true;
            }

            return result;
        }

        private GenericShip GetAnotherShip()
        {
            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    return Combat.Defender;
                case CombatStep.Defence:
                    return Combat.Attacker;
                default:
                    return null;
            }
        }

        private int GetAiPriority()
        {
            return 55;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}