using Abilities.SecondEdition;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ResistanceTransport
    {
        public class TakaJamoreesa : ResistanceTransport
        {
            public TakaJamoreesa() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Taka Jamoreesa",
                    "Snograth Enthusiast",
                    Faction.Resistance,
                    2,
                    4,
                    15,
                    isLimited: true,
                    abilityType: typeof(TakaJamoreesaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/716ee284-e908-4d1d-9f52-ce361d8a88ae/SWZ97_TakaJamoreesalegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TakaJamoreesaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {

        }

        public override void DeactivateAbility()
        {

        }
    }
}
