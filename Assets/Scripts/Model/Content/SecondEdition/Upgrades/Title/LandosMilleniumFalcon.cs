using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandosMilleniumFalcon : GenericUpgrade
    {
        public LandosMilleniumFalcon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando's Millenium Falcon",
                UpgradeType.Title,
                cost: 3,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Scum),
                    new ShipRestriction(typeof(Ship.SecondEdition.CustomizedYT1300LightFreighter.CustomizedYT1300LightFreighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.LandosMilleniumFalconAbility),
                seImageNumber: 164
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LandosMilleniumFalconAbility : GenericAbility
    {
        private List<GenericShip> DockableShips;

        public override void ActivateAbility()
        {
            ActivateDocking(FilterEscapeCraft);

            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckStressedTargetBonus;
            HostShip.OnTryDamagePrevention += CheckDamageRedirection;
        }

        public override void DeactivateAbility()
        {
            DeactivateDocking();

            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckStressedTargetBonus;
            HostShip.OnTryDamagePrevention -= CheckDamageRedirection;
        }

        private bool FilterEscapeCraft(GenericShip ship)
        {
            return ship is Ship.SecondEdition.EscapeCraft.EscapeCraft;
        }

        private void CheckDamageRedirection(GenericShip ship, DamageSourceEventArgs e)
        {
            if (HostShip.DockedShips.Count > 0 && HostShip.DockedShips.First().State.ShieldsCurrent > 0 && HostShip.DockedShips.First().State.ShieldsCurrent > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTryDamagePrevention, AskToRedirectDamage);
            }
        }

        private void AskToRedirectDamage(object sender, EventArgs e)
        {
            AskToRedirectDamageRecursive();
        }

        private void AskToRedirectDamageRecursive()
        {
            if (HostShip.AssignedDamageDiceroll.Count > 0 && HostShip.DockedShips.Count > 0 && HostShip.DockedShips.First().State.ShieldsCurrent > 0)
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    UseShieldsOfDockedShip,
                    descriptionLong: "Do you want to use shield of docked ship instead?",
                    imageHolder: HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseShieldsOfDockedShip(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Shields of Escape Craft were spent");

            HostShip.AssignedDamageDiceroll.RemoveType(GetDieSideToSuffer());

            GenericShip dockedShip = HostShip.DockedShips.First();
            dockedShip.LoseShield();

            AskToRedirectDamageRecursive();
        }

        private DieSide GetDieSideToSuffer()
        {
            DieSide redirectedResult = DieSide.Unknown;
            if (HostShip.AssignedDamageDiceroll.CriticalSuccesses > 0)
            {
                redirectedResult = DieSide.Crit;
            }
            else
            {
                redirectedResult = DieSide.Success;
            }

            return redirectedResult;
        }

        private void CheckStressedTargetBonus(ref int count)
        {
            if (Combat.Defender.IsStressed)
            {
                count++;
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": defender is stressed, attacker gains +1 attack die");
            }
        }
    }
}