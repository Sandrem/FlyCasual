using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class BombletGenerator : GenericTimedBombSE
    {
        public BombletGenerator() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bomblet Generator",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Bomb,
                    UpgradeType.Bomb
                },
                cost: 5,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.BombletGeneratorAbility),
                seImageNumber: 63
            );

            bombPrefabPath = "Prefabs/Bombs/Bomblet";
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            var sufferBombletDamageSubphase = Phases.StartTemporarySubPhaseNew("Damage from " + UpgradeInfo.Name, typeof(SubPhases.SecondEdition.BombletCheckSubPhase), () =>
            {
                Phases.FinishSubPhase(typeof(SubPhases.SecondEdition.BombletCheckSubPhase));
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

            GameManagerScript.Wait(1, delegate { callBack(); });
        }

    }

}

namespace SubPhases.SecondEdition
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
            Messages.ShowInfo("Bomblet: The attacked ship suffered damage");

            DamageSourceEventArgs bombletDamage = new DamageSourceEventArgs()
            {
                Source = "Bomblet",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.Successes, bombletDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo("Bomblet: The attacked ship suffered no damage");
            CallBack();
        }
    }

}

namespace Abilities.SecondEdition
{
    public class BombletGeneratorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += AskAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= AskAbility;
        }

        private void AskAbility()
        {
            if (HostUpgrade.State.Charges < HostUpgrade.UpgradeInfo.Charges
                && HostShip.State.ShieldsCurrent > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToRechargeBombletGenerator);
            }
        }

        private void AskToRechargeBombletGenerator(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                NeverUseByDefault,
                RechargeBombletGenerator,
                infoText: "Do you want to spend 1 shield to recover 2 charges of " + HostUpgrade.UpgradeInfo.Name + "?"
            );
        }

        private void RechargeBombletGenerator(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.LoseShield();
            for (int i = 0; i < 2; i++)
            {
                if (HostUpgrade.State.Charges < HostUpgrade.UpgradeInfo.Charges) HostUpgrade.State.RestoreCharge();
            }
            Triggers.FinishTrigger();
        }
    }
}
