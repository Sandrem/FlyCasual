using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;
using System.Linq;

namespace UpgradesList
{

    public class ProximityMines : GenericContactMine
    {

        public ProximityMines() : base()
        {
            Types.Add(UpgradeType.Bomb);
            Name = "Proximity Mines";
            Cost = 3;

            bombPrefabPath = "Prefabs/Bombs/ProximityMine";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            Selection.ActiveShip = ship;
            var phase = Phases.StartTemporarySubPhaseNew<SubPhases.ProximityMinesCheckSubPhase>(
                "Damage from " + Name,
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.ProximityMinesCheckSubPhase));
                    callBack();
                });
            phase.HostUpgrade = this;
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

    public class ProximityMinesCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade { get; set; }

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 3;

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
            Messages.ShowError("Proximity Mines: ship suffered damage");

            DamageSourceEventArgs proximityDamage = new DamageSourceEventArgs()
            {
                Source = HostUpgrade.Host,
                SourceDescription = "Proximity Mines",
                DamageType = DamageTypes.BombDetonation
            };

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, proximityDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfoToHuman("No damage");
            CallBack();
        }
    }

}
