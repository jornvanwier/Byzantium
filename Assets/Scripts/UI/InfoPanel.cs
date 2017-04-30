using System;
using System.Collections.Generic;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Controllers;
using Assets.Scripts.Game.Units.Formation;
using Assets.Scripts.Game.Units.Formation.ContuberniumFormation;
using Assets.Scripts.Game.Units.Formation.LegionFormation;
using Assets.Scripts.Game.Units.Groups;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class InfoPanel : MonoBehaviour
    {
        private Text commanderText;
        private Image miniMap;

        private GameObject panel;
        private float posX;
        private float posY;

        private RectTransform rectTransform;

        private float sizeX;

        private float sizeY;
        private Text titleText;

        public float PosX
        {
            get { return posX; }
            set
            {
                posX = value;
                panel.transform.position = new Vector3(PosX, PosY);
            }
        }

        public float PosY
        {
            get { return posY; }
            set
            {
                posY = value;
                panel.transform.position = new Vector3(PosX, PosY);
            }
        }

        public float SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
                rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        public float SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
                rectTransform.sizeDelta = new Vector2(SizeX, SizeY);
            }
        }

        public string Title
        {
            get { return titleText.text; }
            set { titleText.text = value; }
        }

        public string Commander
        {
            get { return commanderText.text; }
            set { commanderText.text = value; }
        }

        public bool IsVisible { get; private set; }


        private UnitController army;

        public void AttachArmy(UnitController army)
        {
            this.army = army;
        }

        private readonly Dictionary<string, FormationBase> buttonToFormation = new Dictionary<string, FormationBase>()
        {
            {"Mars", new MarchingFormation()},
            {"Standaard", new StandardFormation()},
            {"Orb", new OrbFormation()},
            {"Skirmish", new SkirmisherFormation()},
            {"Vierkant", new SquareFormation()},
        };


        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        private void SetLegionFormation(FormationBase formation)
        {
            if (army.AttachedUnit is Legion)
            {
                army.AttachedUnit.Formation = formation;
            }
        }

        private void SetContuberniumFormation(IFormation formation)
        {
            foreach (Contubernium contubernium in army.AttachedUnit.Contubernia)
            {
                contubernium.Formation = formation;
            }
        }

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            legionFormation = GameObject.Find("LegioenFormatie");
            contuberniumFormation = GameObject.Find("ContuberniumFormatie");

            GameObject.Find("Mars")
                ?
                .GetComponent<Button>()
                .onClick.AddListener(() => SetLegionFormation(new MarchingFormation()));
            GameObject.Find("Standard")
                ?
                .GetComponent<Button>()
                .onClick.AddListener(() => SetLegionFormation(new StandardFormation()));
            GameObject.Find("Square")
                ?
                .GetComponent<Button>()
                .onClick.AddListener(() => SetContuberniumFormation(new SquareFormation()));
            GameObject.Find("Orb")
                ?
                .GetComponent<Button>()
                .onClick.AddListener(() => SetContuberniumFormation(new OrbFormation()));
            GameObject.Find("Skirmish")
                ?
                .GetComponent<Button>()
                .onClick.AddListener(() => SetContuberniumFormation(new SkirmisherFormation()));

            titleText = GameObject.Find("InfoText").GetComponent<Text>();
            commanderText = GameObject.Find("CommanderText").GetComponent<Text>();
            panel = GameObject.Find("InfoPanel");
            rectTransform = panel.GetComponent<RectTransform>();
            miniMap = GameObject.Find("MiniMapBorder").GetComponent<Image>();

            GameObject.Find("WorldManager")?.GetComponent<WorldManager>().AttachInfoPanel(this);

            UpdatePositionAndSize();
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            UpdatePositionAndSize();
        }

        public void Hide()
        {
            panel.SetActive(false);
            IsVisible = false;
        }
        public bool Contains(Vector2 position)
        {
            var spawnPanelHitBox = new Rect(0, 0, SizeX, SizeY);
            return IsVisible && spawnPanelHitBox.Contains(position);
        }

        private GameObject contuberniumFormation;
        private GameObject legionFormation;

        private void SetFormationPanelActive(bool flag)
        {
            contuberniumFormation.SetActive(flag);
            legionFormation.SetActive(flag);
        }

        public void Show(UnitController army)
        {
            this.army = army;
            Commander =
                $"{army.AttachedUnit.Commander.Name} ({(army.IsAi ? "AI" : "Player")}){Environment.NewLine}{army.Faction.Name}";
            panel.SetActive(true);
            IsVisible = true;

            if (army is InputController)
            {
                SetFormationPanelActive(true);
            }
            else
            {
                SetFormationPanelActive(false);
            }
        }
    }
}