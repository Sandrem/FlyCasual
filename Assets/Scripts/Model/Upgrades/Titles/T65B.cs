using Ship;
using Ship.XWing;
using Upgrade;
using Mods.ModsList;
using Abilities;
using ActionsList;

namespace UpgradesList
{
    public class T65B : GenericUpgradeSlotUpgrade
    {
        public T65B() : base()
        {
            FromMod = typeof(TitlesForClassicShips);

            Types.Add(UpgradeType.Title);
            Name = "T-65B";
            Cost = -1;

            ImageUrl = "https://i.imgur.com/Kz6l6SZ.png";

            UpgradeAbilities.Add(new T65BAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship is XWing);
        }        
    }
}

namespace Abilities
{
    public class T65BAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += AddT65BAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= AddT65BAction;
        }

        private void AddT65BAction(GenericShip host)
        {
            GenericAction newAction = new T65BAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
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