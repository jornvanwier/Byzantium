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
    public class InfoPanel : MonoBehaviour, IPointerClickHandler
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


        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject button = eventData.rawPointerPress;
            GameObject parent = button.transform.parent.gameObject;
            SetFormation(button.name, parent.name);
        }

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

        private void SetFormation(string button, string parent)
        {
            Debug.Log($"{button} {parent}");
            switch (parent)
            {
                case "LegioenFormatie":
                    if (army.AttachedUnit is Legion)
                    {
                        army.AttachedUnit.Formation = buttonToFormation[button];
                    }
                    break;
                case "ContuberniumFormatie":
                    FormationBase formation = buttonToFormation[button];

                    foreach (Contubernium contubernium in army.AttachedUnit.Contubernia)
                    {
                        contubernium.Formation = formation;
                    }
                    break;
            }
        }

        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        private GameObject legionFormation;

        private GameObject contuberniumFormation;

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            legionFormation = GameObject.Find("LegioenFormatie");
            contuberniumFormation = GameObject.Find("ContuberniumFormatie");

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