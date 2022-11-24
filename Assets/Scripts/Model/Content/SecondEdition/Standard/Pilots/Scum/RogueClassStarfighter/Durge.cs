using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class Durge : RogueClassStarfighter
        {
            public Durge() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Durge",
                    "Hard to Kill",
                    Faction.Scum,
                    5,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DurgeAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>()
                    {
                        Tags.BountyHunter
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/9e694fa9-2526-4309-9c51-ad78603548bb/SWZ97_Durge2legal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DurgeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += DurgeActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= DurgeActionEffect;
        }

        private void DurgeActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.DurgeActionEffect()
            {
                ImageUrl = HostShip.ImageUrl,
                HostShip = host
            };
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList
{
    public class DurgeActionEffect : GenericAction
    {

        public DurgeActionEffect()
        {
            Name = DiceModificationName = "Durge";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            //If shields cancel crit
            if (Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes - 1 >= HostShip.State.ShieldsCurrent) result = 100;
            //If crit is better than to destroyed
            if (Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes - HostShip.State.HullCurrent == 0) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.CompareResults
                && Tools.IsSameShip(HostShip, Combat.Defender)
                && (Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes > HostShip.State.ShieldsCurrent))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Success, DieSide.Crit);
            Combat.DiceRollAttack.ChangeOne(DieSide.Success, DieSide.Blank);
            
            callBack();
        }
    }

}