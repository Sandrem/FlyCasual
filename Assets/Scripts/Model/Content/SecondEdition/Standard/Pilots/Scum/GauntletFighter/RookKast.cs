using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class RookKast : GauntletFighter
        {
            public RookKast() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rook Kast",
                    "Stoic Super Commando",
                    Faction.Scum,
                    3,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RookKastAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Red"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/e1/Rookkast.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RookKastAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddRookKastPilotAbility;
            HostShip.OnCombatActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddRookKastPilotAbility;
            HostShip.OnCombatActivation += RegisterAbility;
        }

        private void AddRookKastPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == ship.ShipId
                && Combat.Attacker.IsStrained
                && (Combat.DiceRollAttack.Focuses > 0 || Combat.DiceRollAttack.Blanks > 0))
            {
                ship.AddAvailableDiceModificationOwn(new RookKastAction
                {
                    ImageUrl = HostShip.ImageUrl,
                    HostShip = HostShip
                });
            }
        }

        private class RookKastAction : ActionsList.GenericAction
        {
            public RookKastAction()
            {
                Name = DiceModificationName = "Rook Kast's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Combat.CurrentDiceRoll.Blanks > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
                }
                else if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Attack &&
                    Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon &&
                    Combat.Attacker.Tokens.HasToken(typeof(Tokens.StrainToken)))
                {
                    result = true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack)
                {
                    int attackBlanks = Combat.DiceRollAttack.Blanks;
                    if (attackBlanks > 0)
                    {
                        if ((attackBlanks == 1) && (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                        {
                            result = 100;
                        }
                        else
                        {
                            result = 55;
                        }
                    }
                }

                return result;
            }
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                ActivateOwnAbility,
                descriptionLong: "Do you want to gain 1 strain token?",
                imageHolder: HostUpgrade
            );
        }

        private void ActivateOwnAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }
    }
}