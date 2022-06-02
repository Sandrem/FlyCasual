using Arcs;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class Broadside : BTLBYWing
        {
            public Broadside() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Broadside\"",
                    "Shadow Three",
                    Faction.Republic,
                    3,
                    4,
                    13,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.YWing
                    },
                    abilityType: typeof(Abilities.SecondEdition.BroadsideAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/1e/11/1e117608-f88d-490d-a84e-949b48b7af93/swz48_pilot-broadside.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BroadsideAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ArcForShot.ArcType == ArcType.SingleTurret
                && (Combat.ArcForShot.Facing == ArcFacing.Left || Combat.ArcForShot.Facing == ArcFacing.Right)
                && Combat.DiceRollAttack.Blanks > 0;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
