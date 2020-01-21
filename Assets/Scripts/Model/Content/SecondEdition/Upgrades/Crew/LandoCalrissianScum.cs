using Ship;
using SubPhases;
using System;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandoCalrissianScumCrew : GenericUpgrade
    {
        public LandoCalrissianScumCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando Calrissian",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumAbility),
                seImageNumber: 159
            );

            NameCanonical = "landocalrissian-crew";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2,
                payAbilityCost: PayAbilityCost
            );
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            if (HostShip.Tokens.HasGreenTokens())
            {
                LandoCalrissianScumAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<LandoCalrissianScumAbilityDecisionSubPhase>(
                    "Lando Calrissian: Select green token to spend",
                    delegate { callback(true); }
                );

                subphase.DescriptionShort = "Lando Calrissian";
                subphase.DescriptionLong = "Select a green token to spend";
                subphase.ImageSource = HostUpgrade;

                subphase.HostShip = HostShip;
                subphase.DecisionOwner = HostShip.Owner;
                subphase.Start();
            }
            else
            {
                Messages.ShowError("No green token to spend");
                callback(false);
            }
        }

        private int GetAiPriority()
        {
            return (Combat.DiceRollAttack.Successes - Combat.DiceRollDefence.Successes > 0 && Combat.DiceRollDefence.Failures > 0) ? 10 : 0;
        }

        private bool IsAvailable()
        {
            return HostShip.Tokens.HasGreenTokens();
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}

namespace SubPhases
{
    public class LandoCalrissianScumAbilityDecisionSubPhase : SpendGreenTokenDecisionSubPhase {}
}
