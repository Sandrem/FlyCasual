using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class GavynSykes : NabooRoyalN1Starfighter
        {
            public GavynSykes() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Gavyn Sykes",
                    "Bravo Six",
                    Faction.Republic,
                    3,
                    4,
                    16,
                    isLimited: true,
                    abilityType: typeof(GavynSykesAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/d857e3ca-7688-4854-9787-8f051dec8144/SWZ97_GavynSykeslegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GavynSykesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Gavyn Sykes",
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                new List<DieSide>() { DieSide.Blank },
                timing: DiceModificationTimingType.AfterRolled
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            if (HostShip.RevealedManeuver == null || Combat.Defender.RevealedManeuver == null) return false;

            return ((Combat.AttackStep == CombatStep.Attack && Combat.Attacker == HostShip && HostShip.RevealedManeuver.Speed > Combat.Defender.RevealedManeuver.Speed)
                || (Combat.AttackStep == CombatStep.Defence && Combat.Defender == HostShip && HostShip.RevealedManeuver.Speed > Combat.Attacker.RevealedManeuver.Speed)) && Combat.CurrentDiceRoll.Blanks > 0;
        }

        private int GetAiPriority()
        {
            return 95;
        }
    }
}
