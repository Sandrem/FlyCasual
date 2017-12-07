using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using SubPhases;

namespace Ship
{
    namespace XWing
    {
        public class GarvenDreis : XWing
        {
            public GarvenDreis() : base()
            {
                PilotName = "Garven Dreis";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PilotAbilities.Add(new AbilitiesNamespace.GarvenDreisAbility());
            }
        }
    }
}

namespace AbilitiesNamespace
{
    public class GarvenDreisAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnTokenIsSpent += RegisterGarvenDreisPilotAbility;
        }

        private void RegisterGarvenDreisPilotAbility(GenericShip ship, System.Type type)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, StartSubphaseForGarvenDreisPilotAbility);
        }

        private void StartSubphaseForGarvenDreisPilotAbility(object sender, System.EventArgs e)
        {
            if (HostShip.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectGarvenDreisAbilityTarget,
                    new List<TargetTypes>() { TargetTypes.OtherFriendly },
                    new Vector2(1, 2)
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectGarvenDreisAbilityTarget()
        {
            MovementTemplates.ReturnRangeRuler();

            TargetShip.AssignToken(new Tokens.FocusToken(), SelectShipSubPhase.FinishSelection);
        }
    }
}
