using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEDDefender
{
    public class CaptainDobbs : TIEDDefender
    {
        public CaptainDobbs() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Captain Dobbs",
                "Reliable Replacement",
                Faction.Imperial,
                3,
                7,
                15,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.CaptainDobbsAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                }
            );

            ImageUrl = "https://i.imgur.com/RfgdAPL.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    // While another friendly ship at range 0-1 defends, before the Neutralize Results step,
    // if you are in the attack arc and are not ionized, you may gain 1 ion token to cancel 1 Hit result.

    public class CaptainDobbsAbility : GenericAbility
    {
        public override string Name => HostShip.PilotInfo.PilotName;

        private GenericShip curToDamage;
        private DamageSourceEventArgs curDamageInfo;

        public override void ActivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal += CheckDrawTheirFireAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryDamagePreventionGlobal -= CheckDrawTheirFireAbility;
        }

        private void CheckDrawTheirFireAbility(GenericShip ship, DamageSourceEventArgs e)
        {
            curToDamage = ship;
            curDamageInfo = e;

            if (AbilityCanBeUsed())
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, AskToUseDobbsAbility);
        }

        protected virtual bool AbilityCanBeUsed()
        {
            // Is the damage type a ship attack?
            if (curDamageInfo.DamageType != DamageTypes.ShipAttack)
                return false;

            // Is the defender on our team and not us? If not return.
            if (curToDamage.Owner.PlayerNo != HostShip.Owner.PlayerNo || curToDamage.ShipId == HostShip.ShipId)
                return false;

            // Is the defender at range 1 and is there a hit result?
            if (!Board.IsShipAtRange(HostShip, curToDamage, 1) || curToDamage.AssignedDamageDiceroll.RegularSuccesses < 1)
                return false;

            ShotInfoArc shotInfoArc = new ShotInfoArc(Combat.Attacker, HostShip, Combat.ArcForShot);
            if (!shotInfoArc.InArc) return false;

            if (HostShip.Tokens.CountTokensByType<IonToken>() > 0)
                return false;

            return true;
        }

        protected class CaptainDobbsDecisionSubPhase : DecisionSubPhase { }

        protected virtual void AskToUseDobbsAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                CancelDamageAndGainIon,
                descriptionLong: "Do you want to gain 1 Ion token to cancel 1 Hit result?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void CancelDamageAndGainIon(object sender, System.EventArgs e)
        {
            Die dieToRemove = curToDamage.AssignedDamageDiceroll.DiceList.Find(n => n.Side == DieSide.Success);
            curToDamage.AssignedDamageDiceroll.DiceList.Remove(dieToRemove);

            HostShip.Tokens.AssignToken(typeof(IonToken), DecisionSubPhase.ConfirmDecision);
        }
    }
}