using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class AhsokaTano : TIEFighter
        {
            public AhsokaTano() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ahsoka Tano",
                    7,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.AhsokaTanoPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Rebel
                );

                ModelInfo.ModelName = "TIE Fighter Rebel";
                ModelInfo.SkinName = "Rebel";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class AhsokaTanoPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Count >= 1 && HostShip.Tokens.HasToken(typeof(FocusToken)))
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterTargets,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Choose a ship at range 1 to perform a free action",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly, TargetTypes.This)
                && FilterTargetsByRange(ship, 1, 1) || ship == HostShip;
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            int shipFocusTokens = ship.Tokens.CountTokensByType(typeof(FocusToken));
            if (shipFocusTokens == 0) result += 100;
            result += (5 - shipFocusTokens);
            return result;
        }

        private void SelectAbilityTarget()
        {
            HostShip.Tokens.RemoveToken(
                typeof(FocusToken),
                delegate {
                    var coordinatingShip = Selection.ThisShip;
                    Selection.ThisShip = TargetShip;
                    Selection.ThisShip.AskPerformFreeAction(Selection.ThisShip.GetAvailableActions(), delegate
                    {
                        Selection.ThisShip = coordinatingShip;
                        SelectShipSubPhase.FinishSelection();
                    });
                }
            );
        }
    }
}