using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class VenisaDoza : T70XWing
        {
            public VenisaDoza() : base()
            {
                IsWIP = true;

                PilotInfo = new PilotCardInfo25
                (
                    "Venisa Doza",
                    "Jade Leader",
                    Faction.Resistance,
                    4,
                    5,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VenisaDozaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/1329f27b-51aa-478c-aa83-8cf46bc2ad31/SWZ97_VenisaDozalegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VenisaDozaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}