using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LoadingPanel : MonoBehaviour
    {
        private GameObject panel;
        private GameObject faction1Text;
        private GameObject faction2Text;

        // Use this for initialization
        [UsedImplicitly]
        private void Start()
        {
            panel = GameObject.Find("LoadingPanel");
            faction1Text = GameObject.Find("Faction1");
            faction2Text = GameObject.Find("Faction2");
        }

        // Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {
            faction1Text.GetComponent<Text>().text = "soep";
        }

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}
