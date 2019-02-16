using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Tucker : V19TorrentStarfighter
    {
        public Tucker()
        {
            PilotInfo = new PilotCardInfo(
                "\"Tucker\"",
                2,
                42,
                true,
                abilityType: typeof(Abilities.SecondEdition.TuckerAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/79/05/790527b0-486c-4f5e-a1cb-bab1cf29fb5b/swz32_tucker.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 1-2 performs an attack against an enemy ship in your front arc, you may perform a focus action.
    public class TuckerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            var range = new BoardTools.DistanceInfo(HostShip, Combat.Attacker).Range;

            if (Combat.Attacker.Owner == HostShip.Owner 
                && Combat.Defender.Owner != HostShip.Owner
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front)
                && range >= 1 && range <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskPerformFocusAction);
            }
        }

        private void AskPerformFocusAction(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(HostName + ": you may perform a focus action");

            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            HostShip.AskPerformFreeAction(
                new FocusAction(),
                () => {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}
