using Content;
using System.Collections.Generic;
using Upgrade;
using Ship;
using BoardTools;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class FlightLeaderUbbel : TIERbHeavy
        {
            public FlightLeaderUbbel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Flight Leader Ubbel",
                    "Onyx Leader",
                    Faction.Imperial,
                    5,
                    5,
                    15,
                    abilityType: typeof(Abilities.SecondEdition.FlightLeaderUbbelAbility),
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/c4b1a1ef-76b0-4da2-a06e-eb8e04fe0d84/SWZ97_FlightLeaderUbbellegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FlightLeaderUbbelAbility : GenericAbility
    {
        private GenericShip Attacker;
        private bool PerformedRegularAttack;
        private bool IsAlreadyRegistered;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal -= TryRegisterAbility;
        }

        protected void TryRegisterAbility(GenericShip ship)
        {
            if (Tools.IsSameTeam(ship, HostShip)
                && Board.IsShipBetweenRange(HostShip, ship, 0, 3)
                && Tools.IsSameShip(ship, Combat.Defender)
                && !IsAlreadyRegistered
            )
            {
                Attacker = Combat.Attacker;
                Attacker.OnCombatCheckExtraAttack += StartBonusAttack;
                PerformedRegularAttack = HostShip.IsAttackPerformed;
                IsAlreadyRegistered = true;
            }
        }

        private void StartBonusAttack(GenericShip ship)
        {
            IsAlreadyRegistered = false;
            Attacker.OnCombatCheckExtraAttack -= StartBonusAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishBonusAttack,
                    CounterAttackFilter,
                    HostShip.PilotInfo.PilotName,
                    "You may perform a bonus attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot perform second bonus attack", HostShip.PilotInfo.PilotName));
                FinishBonusAttack();
            }
        }

        private bool CounterAttackFilter(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (targetShip != Attacker)
            {
                if (!isSilent) Messages.ShowErrorToHuman(string.Format("{0} can only attack {1}", HostShip.PilotInfo.PilotName, Attacker.PilotInfo.PilotName));
                result = false;
            }

            return result;
        }

        private void FinishBonusAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = PerformedRegularAttack;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            Triggers.FinishTrigger();
        }
    }
}