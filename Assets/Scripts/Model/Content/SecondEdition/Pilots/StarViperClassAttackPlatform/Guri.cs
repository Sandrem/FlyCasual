using Actions;
using ActionsList;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class Guri : StarViperClassAttackPlatform
        {
            public Guri() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Guri",
                    5,
                    62,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.GuriAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.ActionIcons.Actions.RemoveAll(a => a.ActionType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CalculateAction)));

                ShipInfo.ActionIcons.LinkedActions.RemoveAll(a => a.ActionType == typeof(BoostAction) && a.ActionLinkedType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), typeof(CalculateAction)));
                ShipInfo.ActionIcons.LinkedActions.RemoveAll(a => a.ActionType == typeof(BarrelRollAction) && a.ActionLinkedType == typeof(FocusAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), typeof(CalculateAction)));

                SEImageNumber = 178;
            }
        }
    }
}