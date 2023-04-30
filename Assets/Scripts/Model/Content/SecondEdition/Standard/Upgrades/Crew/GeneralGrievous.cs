using Ship;
using Upgrade;
using System.Linq;
using UnityEngine;
using System;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class GeneralGrievous : GenericUpgrade
    {
        public GeneralGrievous() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "General Grievous",
                UpgradeType.Crew,
                cost: 4,
                charges: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.GeneralGrievousCrewAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(330, 42)
            );
        }
    }
}
namespace Abilities.SecondEdition
{
    //While you defend, after the Neutralize Results step, if there are 2 or more hit or crit results, you may spend 1 charge to cancel 1 hit or crit result. 
    //After a friendly ship is destroyed, recover 1 charge].
    public class GeneralGrievousCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTryDamagePrevention += RegisterDefendAbility;
            GenericShip.OnShipIsDestroyedGlobal += RegisterRecoverAbility;
        }

        private void RegisterDefendAbility(GenericShip ship, DamageSourceEventArgs e)
        {
            if (e.DamageType == DamageTypes.ShipAttack)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, AskToUseDefendAbility);
            }
        }

        private void AskToUseDefendAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0 && Combat.DiceRollAttack.Successes >= 2)
            {
                var phase = (DecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(DecisionSubPhase),
                    Triggers.FinishTrigger
                );

                phase.DescriptionShort = HostName;
                phase.DescriptionLong = "You may spend 1 charge to cancel 1 Hit or Crit result";
                phase.ImageSource = HostShip;

                if (HostShip.AssignedDamageDiceroll.RegularSuccesses > 0)
                {
                    phase.AddDecision("Cancel Hit result", delegate { PreventDamage(DieSide.Success); });
                }

                if (HostShip.AssignedDamageDiceroll.CriticalSuccesses > 0)
                {
                    phase.AddDecision("Cancel Crit result", delegate { PreventDamage(DieSide.Crit); });
                }

                //phase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });
                phase.DefaultDecisionName = phase.GetDecisions().Last().Name;
                phase.ShowSkipButton = true;
                phase.DecisionOwner = HostShip.Owner;
                phase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PreventDamage(DieSide type)
        {
            if (HostUpgrade.State.Charges > 0)
            { 
                Die dieToRemove = HostShip.AssignedDamageDiceroll.DiceList.Find(n => n.Side == type);
                HostShip.AssignedDamageDiceroll.DiceList.Remove(dieToRemove);
                HostUpgrade.State.SpendCharge();
                Messages.ShowInfo($"{HostName} cancels 1 {(type == DieSide.Crit ? "Crit" : "Hit" )} result");
            }

            DecisionSubPhase.ConfirmDecision();
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTryDamagePrevention -= RegisterDefendAbility;
            GenericShip.OnShipIsDestroyedGlobal -= RegisterRecoverAbility;
        }

        private void RegisterRecoverAbility(GenericShip destroyedShip, bool isFled)
        {
            if (destroyedShip.Owner == HostShip.Owner)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, RecoverCharge);
            }
        }

        private void RecoverCharge(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges)
            {
                HostUpgrade.State.RestoreCharge();
                Messages.ShowInfo(HostName + " recovers 1 charge");
            }
            Triggers.FinishTrigger();
        }
    }
}