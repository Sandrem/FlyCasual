using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class Jag : ARC170Starfighter
        {
            public Jag() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Jag\"",
                    3,
                    49,
                    isLimited: true,
                    factionOverride: Faction.Republic,
                    abilityType: typeof(Abilities.SecondEdition.JagAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/06/ec/06ecf59a-74bb-425b-9b5e-0d90a76d3261/swz33_jag.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 1-2 in your left or right arc defends, you may acquire a lock on the attacker.
    public class JagAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += CheckAbility;
        }
        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
    {
            var range = new BoardTools.DistanceInfo(HostShip, Combat.Defender).Range;

            if (Combat.Defender.Owner == HostShip.Owner
                && (HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Left) || HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Right))
                && range >= 1 && range <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, delegate
                {
                    AskToUseAbility(AlwaysUseByDefault, AcquireLock);
                });                
            }
        }

        private void AcquireLock(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostName + ": Acquires lock on " + Combat.Attacker.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, Combat.Attacker, SubPhases.DecisionSubPhase.ConfirmDecision, SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}