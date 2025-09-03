using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Pictyping.UnityClient.Examples.Advanced
{
    /// <summary>
    /// Advanced example demonstrating comprehensive Pictyping API integration
    /// with proper error handling, UI updates, and authentication management
    /// </summary>
    public class PictypingGameManager : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string apiBaseUrl = "http://localhost:5000";
        [SerializeField] private bool useHttpsInProduction = true;
        [SerializeField] private int requestTimeoutSeconds = 30;

        [Header("UI References")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Text statusText;
        [SerializeField] private Text userInfoText;
        [SerializeField] private Button updateRatingButton;
        [SerializeField] private InputField newRatingInput;
        [SerializeField] private Button getRankingButton;
        [SerializeField] private Text rankingText;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        // API clients
        private AuthApi authApi;
        private PictypingAPIApi gameApi;
        private RankingApi rankingApi;

        // Authentication state
        private string currentJwtToken = "";
        private bool isAuthenticated = false;
        private string currentUsername = "";

        // Coroutine references for cleanup
        private Coroutine currentLoginCoroutine;
        private Coroutine currentRatingUpdateCoroutine;
        private Coroutine currentRankingCoroutine;

        private void Start()
        {
            InitializeAPIClients();
            SetupUIEvents();
            UpdateUIState();
            
            // Try to restore saved authentication
            LoadSavedAuthentication();
        }

        private void OnDestroy()
        {
            // Clean up running coroutines
            StopAllCoroutines();
            
            // Save authentication state
            SaveAuthentication();
        }

        private void InitializeAPIClients()
        {
            try
            {
                var config = new Configuration();
                config.BasePath = GetApiUrl();
                config.Timeout = requestTimeoutSeconds * 1000;

                // Add default headers
                config.DefaultHeaders.Add("User-Agent", $"Unity-PictypingGame/{Application.version}");
                
                // Set JWT token if available
                if (!string.IsNullOrEmpty(currentJwtToken))
                {
                    config.AccessToken = currentJwtToken;
                }

                authApi = new AuthApi(config);
                gameApi = new PictypingAPIApi(config);
                rankingApi = new RankingApi(config);

                LogDebug("API clients initialized successfully");
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to initialize API clients: {ex.Message}");
                UpdateStatus("API初期化エラー", true);
            }
        }

        private string GetApiUrl()
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                return apiBaseUrl;
            }
            
            // In production, enforce HTTPS
            if (useHttpsInProduction && apiBaseUrl.StartsWith("http://"))
            {
                return apiBaseUrl.Replace("http://", "https://");
            }
            
            return apiBaseUrl;
        }

        private void SetupUIEvents()
        {
            loginButton?.onClick.AddListener(OnLoginButtonClicked);
            logoutButton?.onClick.AddListener(OnLogoutButtonClicked);
            updateRatingButton?.onClick.AddListener(OnUpdateRatingButtonClicked);
            getRankingButton?.onClick.AddListener(OnGetRankingButtonClicked);
        }

        private void UpdateUIState()
        {
            if (loginButton != null)
                loginButton.interactable = !isAuthenticated;
            
            if (logoutButton != null)
                logoutButton.interactable = isAuthenticated;
            
            if (updateRatingButton != null)
                updateRatingButton.interactable = isAuthenticated;
            
            if (getRankingButton != null)
                getRankingButton.interactable = isAuthenticated;

            if (userInfoText != null)
            {
                userInfoText.text = isAuthenticated ? 
                    $"ログイン中: {currentUsername}" : 
                    "未ログイン";
            }
        }

        #region UI Event Handlers

        private void OnLoginButtonClicked()
        {
            if (usernameInput == null || passwordInput == null)
            {
                LogError("Username or password input field is missing");
                return;
            }

            string username = usernameInput.text.Trim();
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                UpdateStatus("ユーザー名とパスワードを入力してください", true);
                return;
            }

            if (currentLoginCoroutine != null)
            {
                StopCoroutine(currentLoginCoroutine);
            }

            currentLoginCoroutine = StartCoroutine(LoginCoroutine(username, password));
        }

        private void OnLogoutButtonClicked()
        {
            PerformLogout();
        }

        private void OnUpdateRatingButtonClicked()
        {
            if (!isAuthenticated)
            {
                UpdateStatus("ログインが必要です", true);
                return;
            }

            if (newRatingInput == null)
            {
                LogError("Rating input field is missing");
                return;
            }

            if (!int.TryParse(newRatingInput.text, out int newRating))
            {
                UpdateStatus("有効なレーティング値を入力してください", true);
                return;
            }

            if (currentRatingUpdateCoroutine != null)
            {
                StopCoroutine(currentRatingUpdateCoroutine);
            }

            currentRatingUpdateCoroutine = StartCoroutine(UpdateRatingCoroutine(newRating));
        }

        private void OnGetRankingButtonClicked()
        {
            if (!isAuthenticated)
            {
                UpdateStatus("ログインが必要です", true);
                return;
            }

            if (currentRankingCoroutine != null)
            {
                StopCoroutine(currentRankingCoroutine);
            }

            currentRankingCoroutine = StartCoroutine(GetRankingCoroutine());
        }

        #endregion

        #region API Operations

        private IEnumerator LoginCoroutine(string username, string password)
        {
            UpdateStatus("ログイン中...", false);
            loginButton.interactable = false;

            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            bool completed = false;
            string errorMessage = null;
            string token = null;

            authApi.LoginAsync(loginRequest, (response, exception) =>
            {
                if (exception == null)
                {
                    // Note: Adjust based on actual API response structure
                    // token = response.Token;
                    LogDebug("Login successful");
                }
                else
                {
                    errorMessage = GetUserFriendlyErrorMessage(exception);
                }
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                UpdateStatus($"ログイン失敗: {errorMessage}", true);
                LogError($"Login failed: {errorMessage}");
            }
            else
            {
                // Authentication successful
                currentJwtToken = token ?? "dummy_token"; // Remove dummy token when API is ready
                currentUsername = username;
                isAuthenticated = true;
                
                // Update API clients with new token
                UpdateApiClientsToken();
                
                UpdateStatus("ログイン成功", false);
                LogDebug($"User {username} logged in successfully");
            }

            loginButton.interactable = true;
            UpdateUIState();
            currentLoginCoroutine = null;
        }

        private IEnumerator UpdateRatingCoroutine(int newRating)
        {
            UpdateStatus("レーティング更新中...", false);
            updateRatingButton.interactable = false;

            var updateRequest = new UpdateRatingRequest
            {
                NewRating = newRating
            };

            bool completed = false;
            string errorMessage = null;

            gameApi.UpdateRatingAsync(updateRequest, (response, exception) =>
            {
                if (exception == null)
                {
                    LogDebug("Rating updated successfully");
                }
                else
                {
                    errorMessage = GetUserFriendlyErrorMessage(exception);
                }
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                UpdateStatus($"レーティング更新失敗: {errorMessage}", true);
            }
            else
            {
                UpdateStatus($"レーティングを{newRating}に更新しました", false);
            }

            updateRatingButton.interactable = true;
            currentRatingUpdateCoroutine = null;
        }

        private IEnumerator GetRankingCoroutine()
        {
            UpdateStatus("ランキング取得中...", false);
            getRankingButton.interactable = false;

            bool completed = false;
            string errorMessage = null;
            string rankingData = null;

            rankingApi.GetRankingAsync((response, exception) =>
            {
                if (exception == null)
                {
                    rankingData = ProcessRankingData(response);
                    LogDebug("Ranking data retrieved successfully");
                }
                else
                {
                    errorMessage = GetUserFriendlyErrorMessage(exception);
                }
                completed = true;
            });

            yield return new WaitUntil(() => completed);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                UpdateStatus($"ランキング取得失敗: {errorMessage}", true);
                if (rankingText != null)
                    rankingText.text = "ランキングデータを取得できませんでした";
            }
            else
            {
                UpdateStatus("ランキング取得完了", false);
                if (rankingText != null)
                    rankingText.text = rankingData ?? "ランキングデータなし";
            }

            getRankingButton.interactable = true;
            currentRankingCoroutine = null;
        }

        #endregion

        #region Helper Methods

        private void PerformLogout()
        {
            currentJwtToken = "";
            currentUsername = "";
            isAuthenticated = false;
            
            // Clear saved authentication
            DeleteSavedAuthentication();
            
            // Reinitialize API clients without token
            InitializeAPIClients();
            
            UpdateStatus("ログアウトしました", false);
            UpdateUIState();
            
            // Clear UI fields
            if (usernameInput != null) usernameInput.text = "";
            if (passwordInput != null) passwordInput.text = "";
            if (newRatingInput != null) newRatingInput.text = "";
            if (rankingText != null) rankingText.text = "";
        }

        private void UpdateApiClientsToken()
        {
            try
            {
                var config = new Configuration();
                config.BasePath = GetApiUrl();
                config.AccessToken = currentJwtToken;
                config.Timeout = requestTimeoutSeconds * 1000;
                config.DefaultHeaders.Add("User-Agent", $"Unity-PictypingGame/{Application.version}");

                authApi = new AuthApi(config);
                gameApi = new PictypingAPIApi(config);
                rankingApi = new RankingApi(config);

                LogDebug("API clients updated with new token");
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to update API clients: {ex.Message}");
            }
        }

        private string GetUserFriendlyErrorMessage(System.Exception exception)
        {
            if (exception is ApiException apiException)
            {
                switch (apiException.ErrorCode)
                {
                    case 400:
                        return "リクエストが無効です";
                    case 401:
                        return "認証に失敗しました";
                    case 403:
                        return "アクセス権限がありません";
                    case 404:
                        return "要求されたリソースが見つかりません";
                    case 429:
                        return "リクエスト回数が上限に達しました。しばらく待ってから再試行してください";
                    case 500:
                        return "サーバーエラーが発生しました";
                    case 503:
                        return "サービスが一時的に利用できません";
                    default:
                        return $"エラーが発生しました (コード: {apiException.ErrorCode})";
                }
            }
            else
            {
                return "ネットワークエラーが発生しました";
            }
        }

        private string ProcessRankingData(object response)
        {
            // TODO: Process actual ranking response when API is ready
            // This is a placeholder implementation
            return "ランキング:\n1. Player1 - 2500\n2. Player2 - 2400\n3. Player3 - 2300";
        }

        private void UpdateStatus(string message, bool isError)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = isError ? Color.red : Color.black;
            }

            LogDebug($"Status: {message}");
        }

        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[PictypingGameManager] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[PictypingGameManager] {message}");
        }

        #endregion

        #region Authentication Persistence

        private void SaveAuthentication()
        {
            if (isAuthenticated && !string.IsNullOrEmpty(currentJwtToken))
            {
                // Note: In production, use a more secure method than PlayerPrefs
                PlayerPrefs.SetString("PictypingToken", currentJwtToken);
                PlayerPrefs.SetString("PictypingUsername", currentUsername);
                PlayerPrefs.Save();
                
                LogDebug("Authentication saved");
            }
        }

        private void LoadSavedAuthentication()
        {
            if (PlayerPrefs.HasKey("PictypingToken"))
            {
                currentJwtToken = PlayerPrefs.GetString("PictypingToken");
                currentUsername = PlayerPrefs.GetString("PictypingUsername", "");
                
                if (!string.IsNullOrEmpty(currentJwtToken))
                {
                    isAuthenticated = true;
                    UpdateApiClientsToken();
                    UpdateUIState();
                    UpdateStatus("保存された認証情報を復元しました", false);
                    
                    LogDebug("Saved authentication restored");
                }
            }
        }

        private void DeleteSavedAuthentication()
        {
            PlayerPrefs.DeleteKey("PictypingToken");
            PlayerPrefs.DeleteKey("PictypingUsername");
            PlayerPrefs.Save();
            
            LogDebug("Saved authentication deleted");
        }

        #endregion
    }
}