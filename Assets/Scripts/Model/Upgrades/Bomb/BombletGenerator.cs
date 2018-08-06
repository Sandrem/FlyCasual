using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using RuleSets;
using SubPhases;

namespace UpgradesList
{

    public class BombletGenerator : GenericTimedBomb, ISecondEditionUpgrade
    {

        public BombletGenerator() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Types.Add(UpgradeType.Bomb);
            Name = "Bomblet Generator";
            Cost = 3;
            isUnique = true;

            bombPrefabPath = "Prefabs/Bombs/Bomblet";
        }

        public void AdaptUpgradeToSecondEdition()
        {
            IsDiscardedAfterDropped = false;
            UsesCharges = true;

            UpgradeAbilities.Add(new Abilities.SecondEdition.BombletGeneratorAbilitySE());

            MaxCharges = 2;
            Cost = 5;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            var sufferBombletDamageSubphase = Phases.StartTemporarySubPhaseNew("Damage from " + Name, typeof(SubPhases.BombletCheckSubPhase), () =>
            {
                Phases.FinishSubPhase(typeof(SubPhases.BombletCheckSubPhase));
                callBack();
            });
            sufferBombletDamageSubphase.Start();
        }

        public override void PlayDetonationAnimSound(GameObject bombObject, Action callBack)
        {
            int random = UnityEngine.Random.Range(1, 8);
            Sounds.PlayBombSound(bombObject, "Explosion-" + random);
            bombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            bombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1, delegate { callBack(); });
        }

    }

}

namespace SubPhases
{

    public class BombletCheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 2;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }
        }

        private void SufferDamage()
        {
            Messages.ShowError("Bomblet: ship suffered damage");

            DamageSourceEventArgs bombletDamage = new DamageSourceEventArgs()
            {
                Source = "Bomblet",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, bombletDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BombletGeneratorAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
                Phases.Events.OnActivationPhaseStart += RegisterAskToRecoverBombletGeneratorCharges;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterAskToRecoverBombletGeneratorCharges;
        }

        private void RegisterAskToRecoverBombletGeneratorCharges()
        {
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, StartQuestionSubphase);
        }

        protected void StartQuestionSubphase(object sender, System.EventArgs e)
        {
            if (HostUpgrade.Charges < HostUpgrade.MaxCharges && HostShip.Shields > 0)
            {
                BombletGeneratorDecisionSubPhase reloadBombletGeneratorSubPhase = (BombletGeneratorDecisionSubPhase) Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(BombletGeneratorDecisionSubPhase),
                    Triggers.FinishTrigger
                );

                reloadBombletGeneratorSubPhase.InfoText = "Use " + Name + "?";

                reloadBombletGeneratorSubPhase.AddDecision("Spend 1 shield to recover two charges", RegisterRecoverTwoCharges);
                reloadBombletGeneratorSubPhase.AddTooltip("Spend 1 shield to recover two charges", HostShip.ImageUrl);

                reloadBombletGeneratorSubPhase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); });

                reloadBombletGeneratorSubPhase.DefaultDecisionName = GetDefaultDecision();

                reloadBombletGeneratorSubPhase.ShowSkipButton = true;

                reloadBombletGeneratorSubPhase.Start();
            }
        }

        protected void RegisterRecoverTwoCharges(object sender, System.EventArgs e)
        {
            IsAbilityUsed = true;
            HostUpgrade.SetChargesToMax();
            HostShip.LoseShield();

            DecisionSubPhase.ConfirmDecision();
        }

        protected string GetDefaultDecision()
        {
            string result = "No";

            return result;
        }

        protected class BombletGeneratorDecisionSubPhase : DecisionSubPhase { }
    }
}