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
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/f/f8/Garven-dreis.png";
                PilotSkill = 6;
                Cost = 26;

                IsUnique = true;

                PilotAbilities.Add(new PilotAbilitiesNamespace.GarvenDreisAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class GarvenDreisAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnTokenIsSpent += RegisterGarvenDreisPilotAbility;
        }

        private void RegisterGarvenDreisPilotAbility(GenericShip ship, System.Type type)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, StartSubphaseForGarvenDreisPilotAbility);
        }

        private void StartSubphaseForGarvenDreisPilotAbility(object sender, System.EventArgs e)
        {
            if (Host.Owner.Ships.Count > 1)
            {
                SelectTargetForAbility(
                    SelectGarvenDreisAbilityTarget,
                    new List<TargetTypes>() { TargetTypes.OtherFriendly },
                    new Vector2(1, 2),
                    true
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

            TargetShip.AssignToken(
                new Tokens.FocusToken(),
                delegate {
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                });
        }
    }
}
