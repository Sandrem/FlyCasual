using ActionsList;
using BoardTools;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JessikaPava : T70XWing
        {
            public JessikaPava() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jessika Pava",
                    3,
                    52,
                    isLimited: true,
                    charges: 1,
                    regensCharges: true,
                    abilityType: typeof(Abilities.SecondEdition.JessikaPavaAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/bc26d8864f421f1362473aa4982108ba.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JessikaPavaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddJessPavaActionEffects;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddJessPavaActionEffects;
        }

        private void AddJessPavaActionEffects(GenericShip host)
        {
            GenericAction actionPilot = new ActionsList.SecondEdition.JessPavaActionEffect()
            {
                HostShip = host,
                ImageUrl = host.ImageUrl
            };
            host.AddAvailableDiceModification(actionPilot);
        }
    }
}

namespace ActionsList.SecondEdition
{

    public class JessPavaActionEffect : ActionsList.FirstEdition.JessPavaActionEffect
    {
        public override bool IsDiceModificationAvailable()
        {
            bool baseResult = base.IsDiceModificationAvailable();

            return baseResult && (HostShip.State.Charges > 0 || GetAstromechWithNonRecurrungCharges() != null);
        }

        private GenericUpgrade GetAstromechWithNonRecurrungCharges()
        {
            GenericUpgrade astromechUpgrade = HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Astromech);
            if (astromechUpgrade != null)
            {
                if (astromechUpgrade.State.Charges > 0 & !astromechUpgrade.UpgradeInfo.RegensCharges) return astromechUpgrade;
            }

            return null;
        }

        protected override bool InRange(DistanceInfo distanceInfo)
        {
            return distanceInfo.Range <= 1;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.SpendCharge();
            }
            else
            {
                GenericUpgrade astromechUpgrade = GetAstromechWithNonRecurrungCharges();
                if (astromechUpgrade != null)
                {
                    Sounds.PlayShipSound("Astromech-Beeping-and-whistling");
                    astromechUpgrade.State.SpendCharge();
                }
                else
                {
                    Messages.ShowError("Error: No way to spend a charge");
                }
            }

            base.ActionEffect(callBack);
        }

    }

}