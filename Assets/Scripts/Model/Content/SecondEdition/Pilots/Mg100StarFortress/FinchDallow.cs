using BoardTools;
using Bombs;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class FinchDallow : Mg100StarFortress
        {
            public FinchDallow() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Finch Dallow",
                    4,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.FinchDallowAbility)
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/43d9a0555f719a4cbe1ffe905fd38c46.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FinchDallowAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnBombWillBeDropped += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnBombWillBeDropped -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnBombWillBeDropped, AskToUseFinchDallowAbility);
        }

        private void AskToUseFinchDallowAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                NeverUseByDefault,
                PlaceBombInstead,
                infoText: "Do you want to place bombe touching your ship instead?"
            );
        }

        private void PlaceBombInstead(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo("TEST");
            BombsManager.IsOverriden = true;

            PlaceBombTokenSubphase subphase = Phases.StartTemporarySubPhaseNew<PlaceBombTokenSubphase>("Place a bomb", Triggers.FinishTrigger);
            subphase.AbilityName = HostShip.PilotName;
            subphase.Description = "Place the bomb touching your ship";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}

namespace SubPhases
{
    public class PlaceBombTokenSubphase : GenericSubPhase
    {
        GameObject BombGO;

        public string AbilityName;
        public string Description;
        public IImageHolder ImageSource;

        private static bool IsInReposition;

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            UpdateHelpInfo();

            base.Start();
        }

        public override void Prepare()
        {

        }

        public override void Initialize()
        {
            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            ShowDescription();

            CreateBombObject(Selection.ThisShip.GetPosition(), Selection.ThisShip.GetRotation());

            StartDrag();
        }

        private void CreateBombObject(Vector3 bombPosition, Quaternion bombRotation)
        {
            GameObject prefab = (GameObject)Resources.Load(BombsManager.CurrentBomb.bombPrefabPath, typeof(GameObject));
            BombGO = (MonoBehaviour.Instantiate(prefab, bombPosition, bombRotation, Board.GetBoard()));
        }

        public void ShowDescription()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageSource);
        }

        public override void Next()
        {
            FinishSubPhase();
        }

        private void FinishSubPhase()
        {
            HideSubphaseDescription();

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
            callback();
        }

        public override void Update()
        {
            if (IsInReposition)
            {
                if (CameraScript.InputMouseIsEnabled) PerformDrag();
                /*if (CameraScript.InputTouchIsEnabled) PerformTouchDragRotate();
                CheckLimits();*/
            }
            //CheckPerformRotation();
        }

        private void PerformDrag()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                BombGO.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
            }
        }

        public void StartDrag()
        {
            Roster.SetRaycastTargets(false);

            IsInReposition = true;
        }

        public override void ProcessClick()
        {
            

            IsInReposition = false;

            Roster.SetRaycastTargets(true);

            BombsManager.CurrentBomb.ActivateBombs(new List<GameObject>() { BombGO }, Next);
        }
    }
}