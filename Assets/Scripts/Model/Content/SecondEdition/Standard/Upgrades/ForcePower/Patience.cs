using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Patience : GenericUpgrade
    {
        public Patience() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Patience",
                UpgradeType.ForcePower,
                cost: 2,
                restriction: new TagRestriction(Tags.DarkSide),
                abilityType: typeof(Abilities.SecondEdition.PatienceAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5c/f9/5cf91573-a7a8-47ec-9139-f3d0043fce0c/swz79_patience.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PatienceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (HostShip.SectorsInfo.IsShipInSector(enemyShip, Arcs.ArcType.Front))
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = $"Patience ({HostShip.ShipId})",
                            TriggerType = TriggerTypes.OnCombatPhaseStart,
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            EventHandler = AskToRegenForce
                        }
                    );
                    return;
                }
            }
        }

        private void AskToRegenForce(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Patience",
                NeverUseByDefault,
                GainDepleteAndRegenForce,
                descriptionLong: "Do you want to gain Deplete token to recover 1 Force?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GainDepleteAndRegenForce(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(
                typeof(Tokens.DepleteToken),
                delegate
                {
                    if (HostShip.State.Force < HostShip.State.MaxForce) HostShip.State.RestoreForce();
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}