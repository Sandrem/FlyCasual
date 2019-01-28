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
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BosskPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 210
                );
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
            if (Combat.ChosenWeapon == HostShip.PrimaryWeapons)
            {
                base.RegisterBosskPilotAbility();
            }
        }
    }
}