using ActionsList;
using BoardTools;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Boost : CloneZ95Headhunter
        {
            public Boost() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Boost\"",
                    "CT-4860",
                    Faction.Republic,
                    3,
                    3,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoostAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/boost.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterBoostAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterBoostAbility;
        }

        private void RegisterBoostAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, FreeBoostAbility);
        }

        private void FreeBoostAbility(object sender, System.EventArgs e)
        {

            if (Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Friendly).FindAll(n => n.RevealedManeuver.ColorComplexity == Movement.MovementComplexity.Easy).Count >= 1)
            {
                Selection.ThisShip = HostShip;
                HostShip.AskPerformFreeAction(
                    new BoostAction() { HostShip = HostShip },
                    Triggers.FinishTrigger,
                    HostShip.PilotInfo.PilotName,
                    "At the start of the Engagement Phase, if there is a friendly ship at range 0-1 whose revealed maneuver is blue, you may perform a boost action.",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}