using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.BTLA4YWing
{
    public class ArlizHadrassian : BTLA4YWing
    {
        public ArlizHadrassian() : base()
        {
            PilotInfo = new PilotCardInfo
            (
                "Arliz Hadrassian",
                4,
                38,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.ArlizHadrassianAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Tech
                },
                factionOverride: Faction.Scum
            );

            ModelInfo.SkinName = "Gray";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/14/35/1435fe85-32d8-4fb3-8e80-e4d68add15ef/swz85_pilot_arlizhadrassian.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ArlizHadrassianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsPosititveAvailable,
                GetPositiveAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Crit
            );

            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsNegativeAvailable,
                GetNegativeAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.AfterRolled,
                isForcedModification: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsPosititveAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponInfo.ArcRestrictions.Contains(Arcs.ArcType.Front)
                && HostShip.Damage.IsDamaged
                && Combat.DiceRollAttack.HasResult(DieSide.Focus);
        }

        private int GetPositiveAiPriority()
        {
            return 70;
        }

        private bool IsNegativeAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence
                && HostShip.Damage.IsDamaged
                && Combat.DiceRollDefence.HasResult(DieSide.Focus);
        }

        private int GetNegativeAiPriority()
        {
            return 1;
        }
    }
}