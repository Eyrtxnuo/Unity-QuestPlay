using System;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Events;

namespace MetaPlatformStuff
{
    public class MetaLeaderboardManager : MonoBehaviour
    {

        public UnityEvent onInitSuccess;
        
        public static MetaLeaderboardManager Instance { get; private set; }
        public ulong CurrentUserId { get; private set; } = 0;

        [Header("Configurazione Meta")]

        // Indica se i servizi online sono riusciti a connettersi
        public bool IsOnlineReady { get; private set; } = false;


        private void Awake()
        {
            // Singleton per mantenerlo attivo tra le scene
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitMetaPlatform();
        }

        private void InitMetaPlatform()
        {
            try
            {
                Core.AsyncInitialize().OnComplete(OnInitComplete);
            }
            catch (UnityException e)
            {
                Debug.LogWarning($"[Meta SDK] Non attivo ({e.Message}). Continuo in modalità locale.");
                IsOnlineReady = false;
            }
        }

        private void OnInitComplete(Message<PlatformInitialize> msg)
        {
            if (msg.IsError)
            {
                // Errore (o offline o utente non possiede l'app)
                Debug.LogWarning($"[Meta SDK] Inizializzazione fallita: {msg.GetError().Message}. Uso salvataggio locale.");
                IsOnlineReady = false;
                return;
            }

            Debug.Log("[Meta SDK] Piattaforma pronta! Leaderboard online attive.");
            IsOnlineReady = true;
            
            Users.GetLoggedInUser().OnComplete(userMsg =>
            {
                if (!userMsg.IsError)
                {
                    CurrentUserId = userMsg.Data.ID;
                    Debug.Log($"[Meta SDK] Utente loggato: {userMsg.Data.OculusID} (ID: {CurrentUserId})");
                }
                else
                {
                    Debug.LogWarning($"[Meta SDK] Impossibile leggere l'ID utente: {userMsg.GetError().Message}");
                }
            });


            onInitSuccess?.Invoke();
            
            
        }
        
        private void FetchEntries(DataSave.DataKeys scoreboardKey, int limit, LeaderboardStartAt startAt, Action<LeaderboardEntryList> onSuccess, Action<string> onError)
        {
            if (!IsOnlineReady)
            {
                onError?.Invoke("Piattaforma Meta offline.");
                return;
            }

            Leaderboards.GetEntries(DataSave.GetScoreboardKey(scoreboardKey), limit, LeaderboardFilterType.None, startAt)
                .OnComplete(msg =>
                {
                    if (msg.IsError)
                    {
                        Debug.LogWarning($"[Meta SDK] Errore lettura ({startAt}): {msg.GetError().Message}");
                        onError?.Invoke(msg.GetError().Message);
                        return;
                    }
                    onSuccess?.Invoke(msg.Data);
                });
        }

        /////// Metodi pubblici /////////////

        public int GetLocalHighScore(DataSave.DataKeys scoreboardKey)
        {
            return DataSave.GetVal(scoreboardKey);
        }

        public void SubmitScoreToMeta(DataSave.DataKeys scoreboardKey, int score)
        {
            Leaderboards.WriteEntry(DataSave.GetScoreboardKey(scoreboardKey), score).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.LogWarning($"[Meta SDK] Impossibile inviare il punteggio online: {msg.GetError().Message}");
                }
                else
                {
                    Debug.Log($"[Meta SDK] Punteggio {score} sincronizzato sulla Leaderboard!");
                }
            });
        }
        
        public void FetchTopScores(DataSave.DataKeys scoreboardKey, int limit, Action<LeaderboardEntryList> onSuccess, Action<string> onError = null)
        {
            FetchEntries(scoreboardKey, limit, LeaderboardStartAt.Top, onSuccess, onError);
        }
        
        /// <summary>
        /// Scarica la classifica centrata sull'utente loggato (es. 2 sopra, l'utente, 2 sotto).
        /// </summary>
        public void FetchScoresAroundPlayer(DataSave.DataKeys scoreboardKey, int limit, Action<LeaderboardEntryList> onSuccess, Action<string> onError = null)
        {
            FetchEntries(scoreboardKey, limit, LeaderboardStartAt.CenteredOnViewer, onSuccess, onError);
        }

        /// <summary>
        /// Scarica i primi N punteggi della classifica globale + il player.
        /// </summary>
        public void FetchTopScoresAndPlayer(DataSave.DataKeys scoreboardKey,int topLimit, Action<List<LeaderboardEntry>,LeaderboardEntry> onSuccess, Action<string> onError = null)
        {
            FetchTopScores(scoreboardKey, topLimit, topEntries =>
            {
                List<LeaderboardEntry> combined = new List<LeaderboardEntry>(topEntries.Data);

                // solo il player
                FetchScoresAroundPlayer(scoreboardKey, 1, userEntries =>
                    {
                        onSuccess?.Invoke(combined, userEntries[0]);
                    },
                    // Se fallisce la lettura dell'utente (es. non ha ancora un punteggio) return null
                    e =>
                    {
                        Debug.LogWarning(e);
                        onSuccess?.Invoke(combined, null);
                    });

            }, onError);
        }
    }
}
