using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Hate : GenericUpgrade
    {
        public Hate() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hate",
                UpgradeType.Force,
                cost: 3,
                restriction: new FactionRestriction(Faction.Scum, Faction.Imperial, Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.HateAbility)//,
                //seImageNumber: 22
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/4a10b5c8a3d796116163a741d145f4e9.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HateAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShieldLost += RegisterHateAbilityShield;
            HostShip.OnDamageCardIsDealt += RegisterHateAbilityHull;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShieldLost -= RegisterHateAbilityShield;
            HostShip.OnDamageCardIsDealt -= RegisterHateAbilityHull;
        }

        private void RegisterHateAbilityShield()
        {
            RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, RecoverForceToken);
        }

        private void RegisterHateAbilityHull(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, RecoverForceToken);
        }

        private void RecoverForceToken(object sender, System.EventArgs e)
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
            {
                HostShip.State.Force++;
            }
            Triggers.FinishTrigger();
        }
    }
}