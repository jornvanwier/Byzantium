using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.Controllers;
using Assets.Scripts.Game.Units.Groups;
using Assets.Scripts.Map;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SpawnPanel : MonoBehaviour, IPointerClickHandler
    {
        private readonly Dictionary<string, Func<Faction, SoldierType, UnitBase>> typeToAction =
            new Dictionary<string, Func<Faction, SoldierType, UnitBase>>
            {
                {"LegionSpawnerPanel", Legion.CreateCustomUnit},
                {"CohortSpawnerPanel", Cohort.CreateCustomUnit},
                {"CenturySpawnerPanel", Century.CreateCustomUnit},
                {"ContuberniumSpawnerPanel", Contubernium.CreateCustomUnit}
            };

        private readonly Dictionary<string, SoldierType> typeToSoldier = new Dictionary<string, SoldierType>
        {
            {"BowHorse", SoldierType.HorseBow},
            {"SpearHorse", SoldierType.HorseSpear},
            {"SwordHorse", SoldierType.HorseSword},
            {"BowSoldier", SoldierType.Bow},
            {"SpearSoldier", SoldierType.Spear},
            {"SwordSoldier", SoldierType.Sword}
        };


        private GameObject centurySpawnerPanel;
        private GameObject cohortSpawnerPanel;
        private GameObject contuberniumSpawnerPanel;
        private GameObject legionSpawnerPanel;
        private Image miniMap;

        private GameObject panel;
        private float posX;
        private float posY;

        private RectTransform rectTransform;

        private UnitController selectedArmy;

        private float sizeX;

        private float sizeY;

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

        public bool IsVisible { get; private set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject button = eventData.rawPointerPress;
            GameObject parent = button.transform.parent.gameObject;
            Spawn(button.name, parent.name);
        }

        private void UpdatePositionAndSize()
        {
            PosX = SizeX / 2;
            PosY = SizeY / 2;

            SizeX = Screen.width - miniMap.rectTransform.sizeDelta.x;
            SizeY = miniMap.rectTransform.sizeDelta.y;
        }

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            panel = GameObject.Find("SpawnPanel");
            rectTransform = panel.GetComponent<RectTransform>();
            panel.GetComponent<Image>();
            miniMap = GameObject.Find("MiniMapBorder").GetComponent<Image>();

            GameObject.Find("WorldManager")?.GetComponent<WorldManager>().AttachSpawnPanel(this);

            legionSpawnerPanel = GameObject.Find("LegionSpawnerPanel");
            cohortSpawnerPanel = GameObject.Find("CohortSpawnerPanel");
            centurySpawnerPanel = GameObject.Find("CenturySpawnerPanel");
            contuberniumSpawnerPanel = GameObject.Find("ContuberniumSpawnerPanel");

            UpdatePositionAndSize();
            Hide();
        }

        public bool Contains(Vector2 position)
        {
            var spawnPanelHitBox = new Rect(0, 0, SizeX, SizeY);
            return IsVisible && spawnPanelHitBox.Contains(position);
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

        public void Show(UnitController army)
        {
            panel.SetActive(true);
            IsVisible = true;
            selectedArmy = army;

            contuberniumSpawnerPanel.SetActive(true);
            centurySpawnerPanel.SetActive(true);
            cohortSpawnerPanel.SetActive(true);
            legionSpawnerPanel.SetActive(true);

            switch (army.AttachedUnit.UnitName)
            {
                case "Contubernium":
                    contuberniumSpawnerPanel.SetActive(false);
                    goto case "Century";
                case "Century":
                    centurySpawnerPanel.SetActive(false);
                    goto case "Cohort";
                case "Cohort":
                    cohortSpawnerPanel.SetActive(false);
                    goto case "Legion";
                case "Legion":
                    legionSpawnerPanel.SetActive(false);
                    break;
            }
        }

        private void Spawn(string soldierName, string groupName)
        {
            if (!typeToSoldier.ContainsKey(soldierName) || !typeToAction.ContainsKey(groupName)) return;

            SoldierType type = typeToSoldier[soldierName];
            UnitBase unit = typeToAction[groupName](selectedArmy.Faction, type);
            selectedArmy.AddUnit(unit);

            UnitController.Teleport(selectedArmy.AttachedUnit.Position, unit);
        }
    }
}