using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class SecondSister : TIEInterceptor
        {
            public SecondSister() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Second Sister",
                    "Manipulative Monster",
                    Faction.Imperial,
                    4,
                    5,
                    14,
                    force: 2,
                    isLimited: true,
                    abilityType: typeof(SecondSisterAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/f542f260-755f-48ff-84bf-7d8486ffd6b7/SWZ97_SecondSisterlegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SecondSisterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults += TrySecondSisterDiceMofication;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsCompareResults -= TrySecondSisterDiceMofication;
        }

        private void TrySecondSisterDiceMofication(GenericShip host)
        {
            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
            {
                AddDiceModification(host);
            }
        }

        private void AddDiceModification(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.SecondSisterDiceModification()
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
    public class SecondSisterDiceModification : GenericAction
    {
        public SecondSisterDiceModification()
        {
            Name = DiceModificationName = "Second Sister's ability";
            DiceModificationTiming = DiceModificationTimingType.CompareResults;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.Defender.State.ShieldsCurrent < Combat.DiceRollAttack.Successes) result = 100;
            result -= ActionsHolder.CountEnemiesTargeting(HostShip) * 50;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes && Combat.Attacker.State.Force >= 2)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeAll(DieSide.Success, DieSide.Crit, false);
            Combat.Attacker.State.SpendForce(2, callBack);
        }

    }
}