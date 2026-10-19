#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class KeystoreUnlocker
    {
        // Chiavi salvate nel sistema operativo locale (fuori dal progetto)
        private static string KeystorePassPref => $"KeystorePass_{Application.productName}";
        private static string KeyaliasPassPref => $"KeyaliasPass_{Application.productName}";

        // Questo costruttore statico viene eseguito automaticamente all'avvio di Unity
        static KeystoreUnlocker()
        {
            ApplyPasswords();
        }

        private static void ApplyPasswords()
        {
            if (EditorPrefs.HasKey(KeystorePassPref) && EditorPrefs.HasKey(KeyaliasPassPref))
            {
                PlayerSettings.Android.keystorePass = EditorPrefs.GetString(KeystorePassPref);
                PlayerSettings.Android.keyaliasPass = EditorPrefs.GetString(KeyaliasPassPref);
            }
        }

        // Menu in alto su Unity per salvare la password la prima volta sul tuo PC
        [MenuItem("Tools/Keystore AutoFill/Salva Password Correnti in Locale")]
        public static void SaveCurrentPasswords()
        {
            if (string.IsNullOrEmpty(PlayerSettings.Android.keystorePass) || 
                string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                Debug.LogWarning("Inserisci prima le password in Project Settings -> Player -> Publishing Settings!");
                return;
            }

            EditorPrefs.SetString(KeystorePassPref, PlayerSettings.Android.keystorePass);
            EditorPrefs.SetString(KeyaliasPassPref, PlayerSettings.Android.keyaliasPass);
            Debug.Log("Password Keystore salvate in locale! Verranno caricate automaticamente ad ogni avvio.");
        }

        [MenuItem("Tools/Keystore AutoFill/Rimuovi Password Salvate")]
        public static void ClearSavedPasswords()
        {
            EditorPrefs.DeleteKey(KeystorePassPref);
            EditorPrefs.DeleteKey(KeyaliasPassPref);
            Debug.Log("Password Keystore rimosse da EditorPrefs.");
        }
    }
}
#endif