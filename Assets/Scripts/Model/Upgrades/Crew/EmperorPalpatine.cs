using Upgrade;
using Ship;
using SubPhases;
using Abilities;

namespace UpgradesList
{
    public class EmperorPalpatine : GenericUpgrade
    {
        public EmperorPalpatine() : base()
        {
            Types.Add(UpgradeType.Crew);
            Types.Add(UpgradeType.Crew);
            Name = "Emperor Palpatine";
            Cost = 8;
                        
            UpgradeAbilities.Add(new EmperorPalpatineCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class EmperorPalpatineCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {            
            GenericShip.OnDiceAboutToBeRolled += CheckEmperorPalpatineAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDiceAboutToBeRolled -= CheckEmperorPalpatineAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckEmperorPalpatineAbility()
        {
            if (!IsAbilityUsed)
            {
                if ((HostShip.Owner.Id == Combat.Attacker.Owner.Id && Combat.AttackStep == CombatStep.Attack) || // We're attacking
                   ((HostShip.Owner.Id == Combat.Defender.Owner.Id && Combat.AttackStep == CombatStep.Defence))) // or we're defending
                {
                    RegisterAbilityTrigger(TriggerTypes.OnDiceAboutToBeRolled, StartQuestionSubphase);
                }
            }            
        }

        private void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            // TODO: This only works for Palp's ship.. need to make it work for any friendly
            EmperorPalpatineDecisionSubPhase.SquadRole squadRole = EmperorPalpatineDecisionSubPhase.SquadRole.Other;

            if (HostShip.Owner.Id == Combat.Attacker.Owner.Id && Combat.AttackStep == CombatStep.Attack) // We're attacking
            {
                squadRole = EmperorPalpatineDecisionSubPhase.SquadRole.Attacking;
            }
            else
            {
                if (HostShip.Owner.Id == Combat.Defender.Owner.Id && Combat.AttackStep == CombatStep.Defence) // We're defending
                {
                    squadRole = EmperorPalpatineDecisionSubPhase.SquadRole.Defending;
                }
                else
                {
                    // We're doing something else - for future non-combat Palpatine
                    //newSubPhase.Role = EmperorPalpatineDecisionSubPhase.SquadRole.Other;
                    Triggers.FinishTrigger();
                    return;
                }
            }


            EmperorPalpatineDecisionSubPhase newSubPhase = (EmperorPalpatineDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(EmperorPalpatineDecisionSubPhase),
                Triggers.FinishTrigger
            );

            newSubPhase.Role = squadRole;

            newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
            newSubPhase.InfoText = "Use " + Name + "?";

            newSubPhase.AddDecision("No", DontUseEmperorPalpatine);
            newSubPhase.DefaultDecisionName = GetDefaultDecision();


            if (newSubPhase.Role == EmperorPalpatineDecisionSubPhase.SquadRole.Attacking)
            {
                newSubPhase.AddDecision("Critical Hit", ChoiceCriticalHit);
                newSubPhase.AddDecision("Hit", ChoiceHit);
                // TODO: Set default
            } else
            {
                if (newSubPhase.Role == EmperorPalpatineDecisionSubPhase.SquadRole.Defending)
                {
                    newSubPhase.AddDecision("Evade", ChoiceEvade);
                    // TODO: Set default
                }
            }

            newSubPhase.AddDecision("Focus", ChoiceFocus);
            newSubPhase.AddDecision("Blank", ChoiceBlank);

            newSubPhase.Start();
        }

        private void ChoiceCriticalHit(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Messages.ShowInfo("Emperor Palpatine chooses 'Critical Hit'");
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChoiceHit(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Messages.ShowInfo("Emperor Palpatine chooses 'Hit'");
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChoiceEvade(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Messages.ShowInfo("Emperor Palpatine chooses 'Evade'");
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChoiceFocus(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Messages.ShowInfo("Emperor Palpatine chooses 'Focus'");
            DecisionSubPhase.ConfirmDecision();
        }

        private void ChoiceBlank(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            Messages.ShowInfo("Emperor Palpatine chooses 'Blank'");
            DecisionSubPhase.ConfirmDecision();
        }

        private void DontUseEmperorPalpatine(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Emperor Palpatine not used");
            DecisionSubPhase.ConfirmDecision();
        }

        private string GetDefaultDecision()
        {
            string result = "No";

            // TODO: set default logic

            return result;
        }
    }
}

namespace SubPhases
{
    public class EmperorPalpatineDecisionSubPhase : DecisionSubPhase
    {
        public enum SquadRole
        {
            Attacking,
            Defending,
            Other
        }

        public EmperorPalpatineCrewAbility EmperorPalpatineCrew;
        public SquadRole Role;
        //public override void PrepareDecision(System.Action callBack)
        //{
        //    InfoText = "Use Emperor Palpatine?";
        //    RequiredPlayer = EmperorPalpatineCrew.HostShip.Owner.PlayerNo;

        //    AddDecision("Yes", UseEmperorPalpatine);
        //    AddDecision("No", DontUseEmperorPalpatine);

        //    DefaultDecisionName = "No";

        //    callBack();
        //}

        //private void UseEmperorPalpatine(object sender, System.EventArgs e)
        //{
        //    Messages.ShowInfo("Emperor Palpatine Ability");

        //    EmperorPalpatineDiceDecisionSubPhase newSubPhase = (EmperorPalpatineDiceDecisionSubPhase)Phases.StartTemporarySubPhaseNew("Emperor Palpatine's dice decision",
        //                                                                                typeof(SubPhases.EmperorPalpatineDiceDecisionSubPhase), Triggers.FinishTrigger);
        //    newSubPhase.EmperorPalpatineCrew = EmperorPalpatineCrew;
        //    newSubPhase.Role = Role;
        //    newSubPhase.Start();
        //}


    }

    //public class EmperorPalpatineDiceDecisionSubPhase : DecisionSubPhase
    //{
    //    public EmperorPalpatineCrewAbility EmperorPalpatineCrew;
    //    public EmperorPalpatineDecisionSubPhase.SquadRole Role;
    //    public override void PrepareDecision(System.Action callBack)
    //    {
    //        // Need to make this work for different dice types
    //        InfoText = "Choose desired dice result";
    //        RequiredPlayer = EmperorPalpatineCrew.HostShip.Owner.PlayerNo;

    //        if (Role == EmperorPalpatineDecisionSubPhase.SquadRole.Attacking)
    //        {
    //            AddDecision("Critical Hit", ChoiceCriticalHit);
    //            AddDecision("Hit", ChoiceHit);
    //            DefaultDecisionName = "Critical Hit";
    //        }
    //        else
    //        {
    //            if (Role == EmperorPalpatineDecisionSubPhase.SquadRole.Defending)
    //            {
    //                AddDecision("Evade", ChoiceEvade);
    //                DefaultDecisionName = "Evade";
    //            }
    //        }            
    //        AddDecision("Focus", ChoiceFocus);
    //        AddDecision("Blank", ChoiceBlank);

    //        callBack();
    //    }


    //}

}