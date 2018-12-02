using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FangFighter : FirstEdition.ProtectorateStarfighter.ProtectorateStarfighter
        {
            public FangFighter() : base()
            {
                ShipInfo.ShipName = "Fang Fighter";

                IconicPilots[Faction.Scum] = typeof(FennRau);

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Modification);

                ShipAbilities.Add(new Abilities.SecondEdition.ConcordiaFaceoffAbility());

                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)));
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BoostAction), typeof(FocusAction)));

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/b4/Maneuver_fang_fighter.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you defend, if the attack range is 1 and you are in the attacker's forward firing arc,
    // change 1 result to an evade result.

    public class ConcordiaFaceoffAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Concordia Faceoff",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsAvailable()
        {
            return
            (
                Combat.AttackStep == CombatStep.Defence &&
                Combat.Defender == HostShip &&
                Combat.ShotInfo.Range == 1 &&
                Combat.ShotInfo.InPrimaryArc
            );
        }

        public int AiPriority()
        {
            //TODO: Change to enum
            return 100;
        }
    }
}