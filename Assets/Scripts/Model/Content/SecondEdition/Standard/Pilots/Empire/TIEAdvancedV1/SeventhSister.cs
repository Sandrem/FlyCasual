using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class SeventhSister : TIEAdvancedV1
        {
            public SeventhSister() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Seventh Sister",
                    "Sadistic Interrogator",
                    Faction.Imperial,
                    4,
                    5,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SeventhSisterAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                    },
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.Tie
                    },
                    seImageNumber: 100
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeventhSisterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += SeventhSisterDiceMofication;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= SeventhSisterDiceMofication;
        }

        private void SeventhSisterDiceMofication(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.SeventhSisterDiceModification()
            {
                ImageUrl = HostShip.ImageUrl,
                HostShip = host,
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}


namespace ActionsList.SecondEdition
{
    public class SeventhSisterDiceModification : GenericAction
    {
        public SeventhSisterDiceModification()
        {
            Name = DiceModificationName = "Seventh Sister's ability";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollDefence.Successes > 0 && Combat.ShotInfo.InArc && Combat.Attacker.State.Force >= 2)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
            Combat.Attacker.State.SpendForce(2, callBack);
        }

    }
}