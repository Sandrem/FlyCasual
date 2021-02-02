using Ship;
using Upgrade;
using SubPhases;
using Conditions;
using System.Linq;
using Tokens;
using ActionsList;
using System;
using Actions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class GrandMoffTarkin : GenericUpgrade
    {
        public GrandMoffTarkin()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Grand Moff Tarkin",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,                
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Imperial), 
                    new ActionBarRestriction(typeof(TargetLockAction))
                    ),
                abilityType: typeof(Abilities.SecondEdition.GrandMoffTarkinAbility),
                seImageNumber: 117,
                charges: 2,
                regensCharges: true
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(436, 10),
                new Vector2(150, 150)
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
            HostShip.OnCheckSystemsAbilityActivation += CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        private void CheckForAbility(GenericShip ship, ref bool flag)
        {
            if (HostUpgrade.State.Charges >= 2 && HostShip.Tokens.CountTokensByType<BlueTargetLockToken>() > 0) flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges >= 2 && HostShip.Tokens.CountTokensByType<BlueTargetLockToken>() > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToUseTarkinAbility);
            }
        }

        private void AskToUseTarkinAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault, 
                UseAbility,
                dontUseAbility: delegate { DecisionSubPhase.ConfirmDecision(); },                
                descriptionLong: "Do you want to spend 2 Charges? (If you do, each friendly ship may acquire a target lock on a ship that you have locked)",
                imageHolder: HostUpgrade
            );
        }

        protected void UseAbility(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharges(2);
            var tarkinsLocks = HostShip.Tokens.GetTokens<BlueTargetLockToken>('*');
            var targets = tarkinsLocks
                .Where(t => t.OtherTargetLockTokenOwner is GenericShip)
                .Select(t => t.OtherTargetLockTokenOwner)
                .ToArray();
            // Limit the list of friendlies down to those that can actually lock these targets.
            var friendlies = HostShip.Owner.Ships.Values
                .Where(f => targets.Any(t =>
                {
                    var range = t.GetRangeToShip(f);
                    return f.TargetLockMinRange <= range && f.TargetLockMaxRange >= range;
                }))
                .ToArray();
            if (friendlies.Length == 0)
            {
                Messages.ShowInfo(string.Format("No friendly ships are at Target Lock range of any of {0}'s targets", HostShip.PilotInfo.PilotName));

            }
            else
            {
                foreach (var friendly in friendlies)
                {
                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = friendly.PilotInfo.PilotName + " (" + friendly.ShipId + ") can acquire " + HostShip.PilotInfo.PilotName + "'s Target Lock.",
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
            public ITargetLockable[] tarkinsLocks;            
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
                shipName + ": You may lock any of " + tarkinsShip.PilotInfo.PilotName + "'s target(s)",
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