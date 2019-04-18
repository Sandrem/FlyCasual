using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class Ensnare : GenericUpgrade
    {
        public Ensnare() : base()
        {
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo(
                "Ensnare",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.EnsnareAbility),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.NantexClassStarfighter.NantexClassStarfighter))
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a0/4e/a04e0bc0-8028-46f1-acfa-33c8dc815da2/swz_w5_card-ensnare.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EnsnareAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseEnd_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.IsTractored)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseEnd, TrySelectAnotherShip);
            }
        }

        private void TrySelectAnotherShip(object sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                TransferTractorTokenStart,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                "Ensnare",
                "You may transfer Tractor token to a ship in your mobile arc at range 0-1",
                HostUpgrade
            );
        }

        private void TransferTractorTokenStart()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " transfers Tractor token to " + TargetShip.PilotInfo.PilotName);
            HostShip.Tokens.RemoveToken(typeof(Tokens.TractorBeamToken), TransferTractorTokenFinish);
        }

        private void TransferTractorTokenFinish()
        {
            TargetShip.Tokens.AssignToken(
                new Tokens.TractorBeamToken(TargetShip, HostShip.Owner),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons.First());
            return FilterTargetsByRange(ship, 0, 1) && shotInfo.InArcByType(Arcs.ArcType.SingleTurret);
        }

        private int GetAiPriority(GenericShip ship)
        {
            if (ship.Owner == HostShip.Owner)
            {
                return 0;
            }
            else
            {
                return ship.PilotInfo.Cost;
            }
        }
    }
}