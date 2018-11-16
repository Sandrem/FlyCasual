using Ship;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class ZebOrrelios : TIEFighter
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Zeb\" Orrelios",
                    3,
                    13,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ZebOrreliosPilotAbility)
                );

                ShipInfo.Faction = Faction.Rebel;

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ZebOrreliosPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst += CancelCritsFirstIfDefender;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst -= CancelCritsFirstIfDefender;
        }

        private void CancelCritsFirstIfDefender(GenericShip ship)
        {
            if (ship.ShipId == Combat.Defender.ShipId)
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;
            }
        }
    }
}