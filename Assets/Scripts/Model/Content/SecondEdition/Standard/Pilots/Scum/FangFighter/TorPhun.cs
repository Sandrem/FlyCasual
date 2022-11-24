using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class TorPhun : FangFighter
        {
            public TorPhun() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tor Phun",
                    "Direct Pressure",
                    Faction.Scum,
                    3,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TorPhunAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Skull Squadron"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/bbede7a9-748c-4269-8d6c-cdab20cc7029/SWZ97_TorPhunlegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TorPhunAbility : GenericAbility
    {
        private bool PerformedRegularAttack;

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (Tools.IsSameShip(ship, Combat.Defender)
                && Tools.IsSameShip(HostShip, Combat.Attacker))
            {
                HostShip.OnCombatCheckExtraAttack += RegisterTorPhunAbility;
            }
        }

        private void RegisterTorPhunAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterTorPhunAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, StartTorPhunAbility);
        }

        private void StartTorPhunAbility(object sender, EventArgs e)
        {
            CameraScript.RestoreCamera();

            HostShip.OnCanPerformActionWhileStressed += TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed += TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed += DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped += DisallowActionsWhileStressedAlt;

            List<GenericAction> actions = HostShip.GetAvailableActions();

            HostShip.AskPerformFreeAction(actions,
                StartExtraAttack,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perfrom an action, even while stressed",
                imageHolder: HostShip
            );
        }

        private void DisallowActionsWhileStressed(GenericAction action)
        {
            HostShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped-= DisallowActionsWhileStressedAlt;
        }

        private void DisallowActionsWhileStressedAlt(GenericShip ship)
        {
            HostShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
            HostShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
            HostShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
            HostShip.OnActionIsSkipped -= DisallowActionsWhileStressedAlt;
        }

        private void TemporaryAllowAnyActionsWhileStressed(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void TemporaryAllowActionsWhileStressed(ref bool isAllowed)
        {
            isAllowed = true;
        }

        private void StartExtraAttack()
        {
            PerformedRegularAttack = HostShip.IsAttackPerformed;

            HostShip.OnAttackStartAsAttacker += MarkTorPhurAbilityAsUsed;

            HostShip.IsCannotAttackSecondTime = true;

            Combat.StartSelectAttackTarget(
                HostShip,
                FinishAbility,
                AnyTarget,
                HostShip.PilotInfo.PilotName,
                "You may gain 2 Strain tokens to perform a bonus attack",
                HostShip
            );
        }

        private bool AnyTarget(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            return true;
        }

        private void FinishAbility()
        {
            HostShip.IsAttackPerformed = PerformedRegularAttack;
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            HostShip.OnAttackStartAsAttacker -= MarkTorPhurAbilityAsUsed;

            Triggers.FinishTrigger();
        }

        private void MarkTorPhurAbilityAsUsed()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AssignTwoStrainTokens);
        }

        private void AssignTwoStrainTokens(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: 2 Strain tokens are gained to perform a bonus attack");
            HostShip.Tokens.AssignTokens(CreateStrainToken, 2, Triggers.FinishTrigger);
        }

        private GenericToken CreateStrainToken()
        {
            return new StrainToken(HostShip);
        }
    }
}