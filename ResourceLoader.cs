namespace NightmareVoid
{
    internal class ResourceLoader
    {
        public static GameObject hkprime;

        public static void Initialise(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            hkprime = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            UnityEngine.Object.DontDestroyOnLoad(hkprime);
        }
    }
}
