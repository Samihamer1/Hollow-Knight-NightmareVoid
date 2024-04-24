using System.Collections;
using Vasi;

namespace NightmareVoid
{
    public class NightmareVoid : Mod
    {
        internal static NightmareVoid Instance;

        public NightmareVoid() : base("Nightmare & Void") { }
        public override string GetVersion() => "v1.0.0.1";

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hollow_Knight","Battle Scene/HK Prime")
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;
            ResourceLoader.Initialise(preloadedObjects);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += CheckScene;
            ModHooks.LanguageGetHook += ChangeText;
        }

        private string ChangeText(string key, string sheetTitle, string orig)
        {
            if (key == "NAME_NIGHTMARE_GRIMM")
            {
                return "Nightmare and Void";
            }
            if (key == "GG_S_NIGHTMARE_KING")
            {
                return "Twin gods of mayhem";
            }
            if (key == "NIGHTMARE_GRIMM_MAIN")
            {
                return "And";
            }
            if (key == "NIGHTMARE_GRIMM_SUB")
            {
                return "Void";
            }
            if (key == "NIGHTMARE_GRIMM_SUPER")
            {
                return "Nightmare";
            }
            return orig;
        }

        private void CheckScene(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            if (arg1.name == "GG_Grimm_Nightmare")
            {
                GameManager.instance.StartCoroutine(CheckGrimm());
            }
        }
        private IEnumerator CheckGrimm()
        {
            yield return new WaitWhile(() => GameObject.Find("Grimm Control") == null);
            GameObject grimmControl = GameObject.Find("Grimm Control");
            GameObject ngb = grimmControl.Child("Nightmare Grimm Boss");
            ngb.AddComponent<Boss>();
        }

    }
}