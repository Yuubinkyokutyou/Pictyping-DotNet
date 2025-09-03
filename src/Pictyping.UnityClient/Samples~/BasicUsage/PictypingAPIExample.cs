using System.Collections;
using UnityEngine;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Pictyping.UnityClient.Examples
{
    /// <summary>
    /// Basic example of using the Pictyping API client in Unity
    /// </summary>
    public class PictypingAPIExample : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string apiBaseUrl = "http://localhost:5000";
        [SerializeField] private string jwtToken = "";

        private AuthApi authApi;
        private PictypingAPIApi gameApi;
        private RankingApi rankingApi;

        private void Start()
        {
            InitializeAPIClients();
        }

        private void InitializeAPIClients()
        {
            // Configure API client
            var config = new Configuration();
            config.BasePath = apiBaseUrl;
            
            // Set JWT token if available
            if (!string.IsNullOrEmpty(jwtToken))
            {
                config.AccessToken = jwtToken;
            }

            // Initialize API clients
            authApi = new AuthApi(config);
            gameApi = new PictypingAPIApi(config);
            rankingApi = new RankingApi(config);

            Debug.Log("Pictyping API clients initialized");
        }

        /// <summary>
        /// Example: Login with username and password
        /// </summary>
        public void ExampleLogin()
        {
            StartCoroutine(LoginCoroutine());
        }

        private IEnumerator LoginCoroutine()
        {
            var loginRequest = new LoginRequest
            {
                Username = "example_user",
                Password = "example_password"
            };

            bool completed = false;
            string result = null;
            System.Exception error = null;

            // Perform async login
            authApi.LoginAsync(loginRequest, (response, exception) =>
            {
                if (exception == null)
                {
                    result = "Login successful";
                    // Store JWT token from response if needed
                    // jwtToken = response.Token;
                }
                else
                {
                    error = exception;
                }
                completed = true;
            });

            // Wait for completion
            yield return new WaitUntil(() => completed);

            if (error != null)
            {
                Debug.LogError($"Login failed: {error.Message}");
            }
            else
            {
                Debug.Log(result);
            }
        }

        /// <summary>
        /// Example: Update user rating
        /// </summary>
        public void ExampleUpdateRating()
        {
            StartCoroutine(UpdateRatingCoroutine());
        }

        private IEnumerator UpdateRatingCoroutine()
        {
            var updateRequest = new UpdateRatingRequest
            {
                NewRating = 1500
            };

            bool completed = false;
            System.Exception error = null;

            gameApi.UpdateRatingAsync(updateRequest, (response, exception) =>
            {
                error = exception;
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            if (error != null)
            {
                Debug.LogError($"Rating update failed: {error.Message}");
            }
            else
            {
                Debug.Log("Rating updated successfully");
            }
        }

        /// <summary>
        /// Example: Get ranking information
        /// </summary>
        public void ExampleGetRanking()
        {
            StartCoroutine(GetRankingCoroutine());
        }

        private IEnumerator GetRankingCoroutine()
        {
            bool completed = false;
            System.Exception error = null;

            rankingApi.GetRankingAsync((response, exception) =>
            {
                if (exception == null)
                {
                    Debug.Log("Ranking data retrieved successfully");
                    // Process ranking data here
                }
                else
                {
                    error = exception;
                }
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            if (error != null)
            {
                Debug.LogError($"Failed to get ranking: {error.Message}");
            }
        }
    }
}