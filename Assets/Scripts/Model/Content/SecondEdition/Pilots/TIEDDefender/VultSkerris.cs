using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class VultSkerrisDefender : TIEDDefender
        {
            public VultSkerrisDefender() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Vult Skerris",
                    5,
                    82,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VultSkerrisDefenderAbility),
                    extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.Talent, UpgradeType.Sensor },
                    charges: 1,
                    regensCharges: -1
                );

                ModelInfo.SkinName = "Yellow Edges";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/50/1e/501ea754-bb34-499e-aec9-1f65f941666a/swz84_ship_vultskerris.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VultSkerrisDefenderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddOwnAction;
            HostShip.OnCombatActivation += BeforeEngageActionGrant;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddOwnAction;
            HostShip.OnCombatActivation -= BeforeEngageActionGrant;
        }

        private void AddOwnAction(GenericShip ship)
        {
            HostShip.AddAvailableAction(
                new ActionsList.SecondEdition.VultSkerrisAction()
                {
                    HostShip = HostShip,
                    ImageUrl = HostShip.ImageUrl
                }
            );
        }

        protected void BeforeEngageActionGrant(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, GrantAction);
            }
        }

        protected void GrantAction(object sender, EventArgs e)
        {
            HostShip.OnActionIsPerformed -= SpendOwnCharge;

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActions(),
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "Before you engage, you may spend 1 charge to perform an action",
                HostShip
            );
        }

        private void SpendOwnCharge(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= SpendOwnCharge;

            if (action != null) HostShip.SpendCharge();
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class VultSkerrisAction : GenericAction
    {
        public VultSkerrisAction()
        {
            Name = DiceModificationName = "Vult Skerris";
        }

        public override void ActionTake()
        {
            HostShip.RestoreCharges(1);

            HostShip.Tokens.AssignToken(
                typeof(StrainToken),
                Phases.CurrentSubPhase.CallBack
            );
        }

        public override bool IsActionAvailable()
        {
            return HostShip.State.Charges == 0;
        }

        public override int GetActionPriority()
        {
            return 0;
        }
    }
}