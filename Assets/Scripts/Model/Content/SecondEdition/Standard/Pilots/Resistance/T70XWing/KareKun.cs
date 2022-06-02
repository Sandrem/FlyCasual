using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class KareKun : T70XWing
        {
            public KareKun() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Kare Kun",
                    "Woman of Action",
                    Faction.Resistance,
                    4,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KareKunAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/42/59/42597afe-f592-4bac-98ad-f70e876fb451/swz25_kare_a1.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KareKunAbility : Abilities.SecondEdition.DareDevilAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += ChangeBoostTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= ChangeBoostTemplates;
        }

        private void ChangeBoostTemplates(List<BoostMove> availableMoves, GenericAction action)
        {
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, false));
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, false));
        }
    }
}