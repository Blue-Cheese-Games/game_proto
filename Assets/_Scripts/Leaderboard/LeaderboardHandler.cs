﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bladiator.Leaderboard.Struct;
using Bladiator.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace Bladiator.Leaderboard
{
    [Obsolete("This class will be removed in the future because the current api is not working")]
    public class LeaderboardHandler : MonoBehaviour
    {
        public static LeaderboardHandler INSTANCE = null;
        
        public List<LeaderboardItemData> LeaderboardItem = new List<LeaderboardItemData>();

        [SerializeField] private GameObject m_LeaderboardObject;
        
        public LeaderboardHandler()
        {
            if (INSTANCE == null)
            {
                INSTANCE = this;
            }
        }

        private void Start()
        {
            GameManager.Instance.OnGameStateChange += ShowHideLeaderboard;
            
            m_LeaderboardObject.SetActive(false);
        }

        private void ShowHideLeaderboard(GameState state)
        {
            if (state == GameState.Leaderboard)
            {
                m_LeaderboardObject.SetActive(true);
                
                GetLeaderboard();
            }
            else
            {
                m_LeaderboardObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Add a player to the database with their achieved data
        /// </summary>
        /// <param name="itemData"> Data of the player </param>
        public void AddPlayerToLeaderboard(LeaderboardItemData itemData)
        {
            return; // The api used is no longer active

            const string databaseUrl = "https://bladiator.larsbeijaard.com/scripts/add_leaderboard_item.php";
            StartCoroutine(SendDatabaseRequest(databaseUrl, itemData));
        }

        /// <summary>
        /// Get all of the players that are on the leaderboard
        /// </summary>
        public void GetLeaderboard()
        {
            return; // The api used is no longer active
            
            const string databaseUrl = "https://bladiator.larsbeijaard.com/scripts/get_leaderboard.php";

            StartCoroutine(GetLeaderboardRequest(databaseUrl, (json) => {
            
                if (json != "There are currently no players on the leaderboard.")
                {
                    // Clear the current leaderboard
                    LeaderboardItem.Clear();
                    LeadboardObjectWrapper leaderboardDataWrapper = JsonUtility.FromJson<LeadboardObjectWrapper>(json);
                    
                    // Add each player from the leaderboard to a list
                    foreach (LeaderboardItemData data in leaderboardDataWrapper.LeaderboardData)
                    {
                        LeaderboardItem.Add(new LeaderboardItemData()
                        {
                            name = data.name,
                            score = data.score,
                            wave = data.wave
                        });
                    }
                }
                // This gets executed when there are no players on the leaderboard
                else
                {
                    LeaderboardItem.Add(new LeaderboardItemData()
                    {
                        name = "No man has fought yet.",
                        score = 0,
                        wave = 0
                    });
                }

                SortLeaderboard(LeaderboardSortingType.SORT_ON_SCORE);
                LeaderboardUIHandler.Instance.SetAllContentForLeaderboard();
            }));
        }
        
        /// <summary>
        /// Bubble Sort Algorithm:
        /// Sort the leaderboard from best achieved (score|wave)
        /// </summary>
        /// <param name="sortingType"> The type of sorting (score|wave) </param>
        public void SortLeaderboard(LeaderboardSortingType sortingType)
        {
            // Loop though each of the leaderboard items and sort them by score count
            for (int i = 0; i < LeaderboardItem.Count; i++)
            {
                for (int j = 0; j < LeaderboardItem.Count - 1; j++)
                {
                    // Swap the indexes
                    if (LeaderboardItem[j].score < LeaderboardItem[j + 1].score)
                    {
                        LeaderboardItemData temp = LeaderboardItem[j + 1];
                        LeaderboardItem[j + 1] = LeaderboardItem[j];
                        LeaderboardItem[j] = temp;
                    }  
                }  
            }
        }
        
        /// <summary>
        /// Send a request to the database requesting to add a player to the database
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        private IEnumerator SendDatabaseRequest(string url, LeaderboardItemData data)
        {
            string json = JsonUtility.ToJson(data);
            
            WWWForm form = new WWWForm();

            form.AddField("data", json);

            using (UnityWebRequest client = UnityWebRequest.Post(url, form))
            {
                yield return client.SendWebRequest();
            }
        }

        /// <summary>
        /// Request the players from the leaderboard from the database.
        /// </summary>
        /// <param name="url"> Url to the database </param>
        /// <returns> All the players in the database </returns>
        private IEnumerator GetLeaderboardRequest(string url, Action<string> callback)
        {
            using (UnityWebRequest client = UnityWebRequest.Get(url))
            {
                Debug.Log("UnityWebRequest created, sending request.");
                yield return client.SendWebRequest();
                Debug.Log("Request succesfully sent, response:");

                string s = Encoding.ASCII.GetString(client.downloadHandler.data);
                Debug.Log(s + "\n\n\nInvokingCallback:");
                
                callback.Invoke(s);
            }
        }
    }   
}