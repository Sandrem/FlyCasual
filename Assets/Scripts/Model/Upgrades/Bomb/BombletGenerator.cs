using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

    public class BombletGenerator : GenericTimedBomb
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
