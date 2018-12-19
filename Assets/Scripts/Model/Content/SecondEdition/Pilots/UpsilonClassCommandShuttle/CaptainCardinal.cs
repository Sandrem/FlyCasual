using BoardTools;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class CaptainCardinal : UpsilonClassCommandShuttle
        {
            public CaptainCardinal() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Cardinal",
                    4,
                    64,
                    isLimited: true,
                    charges: 2,
                    abilityType: typeof(Abilities.SecondEdition.CaptainCardinalAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/be29a69f75726ad48f607eecca671e01.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainCardinalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Captain Cardinal",
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                new List<DieSide>() { DieSide.Focus },
                isGlobal: true
            );

            GenericShip.OnDestroyedGlobal += CheckRemoveCharge;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();

            GenericShip.OnDestroyedGlobal -= CheckRemoveCharge;
        }

        private bool IsDiceModificationAvailable()
        {
            if (HostShip.State.Charges == 0) return false;

            GenericShip currentShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;

            if (currentShip.Owner.PlayerNo != HostShip.Owner.PlayerNo) return false;

            if (currentShip.State.Initiative >= HostShip.State.Initiative) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, currentShip);
            if (distInfo.Range < 1 || distInfo.Range > 2) return false;

            return true;
        }

        private int GetDiceModificationAiPriority()
        {
            return 95;
        }

        private void CheckRemoveCharge(GenericShip destroyedShip, bool isFled)
        {
            if (HostShip.State.Charges == 0) return;

            if (destroyedShip.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, destroyedShip);
                if (distInfo.Range < 4)
                {
                    Messages.ShowError(HostShip.PilotInfo.PilotName + ": Charge is lost");
                    HostShip.LoseCharge();
                }
            }
        }

    }
}
