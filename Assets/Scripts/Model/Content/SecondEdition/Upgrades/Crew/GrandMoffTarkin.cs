using Ship;
using Upgrade;
using SubPhases;
using Conditions;
using System.Linq;
using Tokens;
using ActionsList;
using System;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class GrandMoffTarkin : GenericUpgrade
    {
        public GrandMoffTarkin()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Grand Moff Tarkin",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,                
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial), 
                    new ActionBarRestriction(new ActionInfo(typeof(TargetLockAction)))
                    ),
                abilityType: typeof(Abilities.SecondEdition.GrandMoffTarkinAbility),
                seImageNumber: 117,
                charges: 2,
                regensCharges: true
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GrandMoffTarkinAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSystemsPhaseStart += CheckForAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSystemsPhaseStart -= CheckForAbility;
        }

        private void CheckForAbility()
        {
            if (HostUpgrade.State.Charges >= 2 && HostShip.Tokens.CountTokensByType<BlueTargetLockToken>() > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToUseTarkinAbility);
            }
        }

        private void AskToUseTarkinAbility(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, 
                UseAbility,
                dontUseAbility: delegate { DecisionSubPhase.ConfirmDecision(); },                
                infoText: "Grand Moff Tarkin: Allow all allies to lock " + HostShip.PilotInfo.PilotName + "'s locked ship?");
        }

        protected void UseAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharges(2);
            var tarkinsLocks = HostShip.Tokens.GetTokens<BlueTargetLockToken>('*');
            var targets = tarkinsLocks
                .Where(t => t.OtherTokenOwner is GenericShip)
                .Select(t => t.OtherTokenOwner)
                .ToArray();
            // Limit the list of friendlies down to those that can actually lock these targets.
            var friendlies = HostShip.Owner.Ships.Values
                .Where(f => targets.Any(t =>
                {
                    var range = BoardTools.Board.GetRangeOfShips(f, t);
                    return f.TargetLockMinRange <= range && f.TargetLockMaxRange >= range;
                }))
                .ToArray();
            if (friendlies.Length == 0)
            {
                Messages.ShowInfo(string.Format("No friendly ship is at lock range of any of {0}'s targets", HostShip.PilotInfo.PilotName));

            }
            else
            {
                foreach (var friendly in friendlies)
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = friendly.PilotInfo.PilotName + " (" + friendly.ShipId + ") can acquire " + HostShip.PilotInfo.PilotName + "'s lock",
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            TriggerType = TriggerTypes.OnAbilityDirect,
                            EventHandler = AskToAcquireTarkinsLock,
                            Sender = HostShip,
                            EventArgs = new TarkinAbilityEventArgs
                            {
                                tarkinsShip = HostShip,
                                tarkinsFriend = friendly,
                                tarkinsLocks = targets
                            }
                        }
                    );
                }
            }
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, DecisionSubPhase.ConfirmDecision);
        }

        public class TarkinAbilityEventArgs : EventArgs
        {
            public GenericShip tarkinsShip;
            public GenericShip tarkinsFriend;
            public GenericShip[] tarkinsLocks;            
        }

        protected void AskToAcquireTarkinsLock(object sender, EventArgs e)
        {
            var args = e as TarkinAbilityEventArgs;
            var tarkinsShip = args.tarkinsShip;
            var targets = args.tarkinsLocks;
            var ship = args.tarkinsFriend;
            var shipName = ship.PilotInfo.PilotName;
            if (!ship.PilotInfo.IsLimited)
            {
                shipName += " (" + ship.ShipId + ")";
            }
            SelectTargetForAbility(
                () => LockTarget(ship, TargetShip),
                t => targets.Contains(t),
                t => GetAiTargetPriority(ship, t),
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                shipName + ": You may lock any of " + tarkinsShip.PilotInfo.PilotName + "'s target(s).",
                HostUpgrade
            );
        }

        protected void LockTarget(GenericShip ship, GenericShip target)
        {
            ActionsHolder.AcquireTargetLock(ship, target, SelectShipSubPhase.FinishSelection, SelectShipSubPhase.FinishSelection, false);
        }

        protected int GetAiTargetPriority(GenericShip subject, GenericShip target)
        {
            var isInArc = subject.ArcsInfo.Arcs
                .Select(arc => new BoardTools.ShotInfoArc(subject, target, arc))
                .Any(arc => arc.InArc);
            return isInArc ? 1 : 0;
        }
    }
}