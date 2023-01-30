using Content;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAgAggressor
    {
        public class LieutenantKestal : TIEAgAggressor
        {
            public LieutenantKestal() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Kestal",
                    "Innate Deadeye",
                    Faction.Imperial,
                    4,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantKestalAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 127,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantKestalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Cancel,
                int.MaxValue,
                new List<DieSide>() { DieSide.Focus, DieSide.Blank },
                timing: DiceModificationTimingType.Opposite,
                payAbilityCost: PayCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence && HostShip.Tokens.HasToken<FocusToken>();
        }

        private int GetAiPriority()
        {
            return 80;
        }

        private void PayCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasToken<FocusToken>())
            {
                HostShip.Tokens.RemoveToken(typeof(FocusToken), () => callback(true));
            }
            else callback(false);
        }
    }
}
