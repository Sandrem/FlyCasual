using ActionsList;
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
                PilotInfo = new PilotCardInfo(
                    "Kare Kun",
                    4,
                    53,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KareKunAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                //seImageNumber: 93
                );

                //ModelInfo.SkinName = "Black One";

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

        private void ChangeBoostTemplates(List<BoostMove> availableMoves)
        {
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.LeftTurn1, false));
            availableMoves.Add(new BoostMove(ActionsHolder.BoostTemplates.RightTurn1, false));
        }
    }
}