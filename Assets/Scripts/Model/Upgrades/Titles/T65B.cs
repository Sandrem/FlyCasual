using Ship;
using Ship.XWing;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList
{
    public class T65B : GenericUpgradeSlotUpgrade
    {
        public T65B() : base()
        {
            FromMod = typeof(T65BMod);

            Type = UpgradeType.Title;
            Name = "T-65B";
            Cost = -1;

            ImageUrl = "https://media.discordapp.net/attachments/307486197905620993/386997692997435401/T-65B.png?width=195&height=300";
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing);
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += AddT65BAction;
        }

        private void AddT65BAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.T65BAction()
            {
                ImageUrl = ImageUrl,
                Host = Host
            };
            host.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{

    public class T65BAction : GenericAction
    {
        public T65BAction()
        {
            Name = EffectName = "T-65B Boost";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            if (!Selection.ThisShip.IsAlreadyExecutedAction(typeof(BoostAction)))
            {
                Phases.CurrentSubPhase.Pause();
                Phases.StartTemporarySubPhaseOld(
                    "Boost",
                    typeof(SubPhases.BoostPlanningSubPhase),
                    Phases.CurrentSubPhase.CallBack
                );
            }
            else
            {
                Messages.ShowError("Cannot use T65B: Boost is already executed");
                Phases.CurrentSubPhase.Resume();
            }
        }
    }
}