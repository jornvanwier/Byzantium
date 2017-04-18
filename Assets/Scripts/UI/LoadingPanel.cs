using System;
using System.IO;
using Assets.Scripts.Game;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LoadingPanel : MonoBehaviour
    {
        private string dots = ".";
        private Text faction1Text;
        private Text faction2Text;

        private AsyncOperation loadProgress;
        private Text statusText;

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            loadProgress = StartLoadGame();

            faction1Text = GameObject.Find("Faction1Text").GetComponent<Text>();
            faction2Text = GameObject.Find("Faction2Text").GetComponent<Text>();
            statusText = GameObject.Find("StatusText").GetComponent<Text>();

            FactionManager.Init(2);
            faction1Text.text = FactionManager.Factions[0].Name;
            faction2Text.text = FactionManager.Factions[1].Name;
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            if (loadProgress.isDone)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));
            }
            else
            {
                statusText.text = "Loading" + dots;

                dots = dots.Length > 1 ? "." : dots += ".";
            }
        }

        private static AsyncOperation StartLoadGame()
        {
            return SceneManager.LoadSceneAsync("Base");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}