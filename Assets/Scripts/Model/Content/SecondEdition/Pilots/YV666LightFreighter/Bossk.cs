using Upgrade;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class Bossk : YV666LightFreighter
        {
            public Bossk() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Bossk",
                    4,
                    70,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.BosskPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 210;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BosskPilotAbility : Abilities.FirstEdition.BosskPilotAbility
    {
        protected override void RegisterBosskPilotAbility()
        {
            if (Combat.ChosenWeapon == HostShip.PrimaryWeapon)
            {
                base.RegisterBosskPilotAbility();
            }
        }
    }
}