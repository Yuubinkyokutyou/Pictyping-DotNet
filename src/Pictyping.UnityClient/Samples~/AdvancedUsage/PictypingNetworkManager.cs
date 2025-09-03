using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Pictyping.UnityClient.Examples.Advanced
{
    /// <summary>
    /// Network manager for handling connectivity, retries, and offline support
    /// Demonstrates best practices for robust API communication in Unity
    /// </summary>
    public class PictypingNetworkManager : MonoBehaviour
    {
        [Header("Network Configuration")]
        [SerializeField] private string apiBaseUrl = "http://localhost:5000";
        [SerializeField] private int maxRetries = 3;
        [SerializeField] private float retryDelaySeconds = 2.0f;
        [SerializeField] private float networkTimeoutSeconds = 10.0f;
        [SerializeField] private bool enableOfflineMode = true;

        [Header("Health Check")]
        [SerializeField] private float healthCheckInterval = 30.0f;
        [SerializeField] private bool enableHealthCheck = true;

        // Network state
        public bool IsOnline { get; private set; } = true;
        public bool IsServerReachable { get; private set; } = false;
        
        // Events
        public System.Action<bool> OnNetworkStatusChanged;
        public System.Action<bool> OnServerStatusChanged;
        
        // API clients
        private AuthApi authApi;
        private PictypingAPIApi gameApi;
        private RankingApi rankingApi;
        
        // Internal state
        private Coroutine healthCheckCoroutine;
        private Queue<NetworkRequest> offlineRequestQueue;

        private void Start()
        {
            offlineRequestQueue = new Queue<NetworkRequest>();
            InitializeAPIClients();
            
            if (enableHealthCheck)
            {
                StartHealthCheck();
            }
            
            StartCoroutine(MonitorNetworkStatus());
        }

        private void OnDestroy()
        {
            StopHealthCheck();
        }

        private void InitializeAPIClients()
        {
            try
            {
                var config = new Configuration();
                config.BasePath = apiBaseUrl;
                config.Timeout = (int)(networkTimeoutSeconds * 1000);

                authApi = new AuthApi(config);
                gameApi = new PictypingAPIApi(config);
                rankingApi = new RankingApi(config);

                Debug.Log("[NetworkManager] API clients initialized");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NetworkManager] Failed to initialize API clients: {ex.Message}");
            }
        }

        #region Network Status Monitoring

        private IEnumerator MonitorNetworkStatus()
        {
            while (true)
            {
                bool wasOnline = IsOnline;
                IsOnline = Application.internetReachability != NetworkReachability.NotReachable;

                if (wasOnline != IsOnline)
                {
                    Debug.Log($"[NetworkManager] Network status changed: {(IsOnline ? "Online" : "Offline")}");
                    OnNetworkStatusChanged?.Invoke(IsOnline);

                    if (IsOnline && offlineRequestQueue.Count > 0)
                    {
                        StartCoroutine(ProcessOfflineQueue());
                    }
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void StartHealthCheck()
        {
            if (healthCheckCoroutine == null)
            {
                healthCheckCoroutine = StartCoroutine(HealthCheckCoroutine());
            }
        }

        private void StopHealthCheck()
        {
            if (healthCheckCoroutine != null)
            {
                StopCoroutine(healthCheckCoroutine);
                healthCheckCoroutine = null;
            }
        }

        private IEnumerator HealthCheckCoroutine()
        {
            while (true)
            {
                yield return StartCoroutine(CheckServerHealth());
                yield return new WaitForSeconds(healthCheckInterval);
            }
        }

        private IEnumerator CheckServerHealth()
        {
            bool wasReachable = IsServerReachable;
            
            // Simple health check - try to make a lightweight API call
            bool completed = false;
            bool success = false;

            try
            {
                // Use a simple API endpoint for health check
                // This is a placeholder - adjust based on your actual API
                var www = new WWW($"{apiBaseUrl}/health");
                yield return www;
                
                success = string.IsNullOrEmpty(www.error);
                completed = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[NetworkManager] Health check failed: {ex.Message}");
                success = false;
                completed = true;
            }

            yield return new WaitUntil(() => completed);

            IsServerReachable = success;

            if (wasReachable != IsServerReachable)
            {
                Debug.Log($"[NetworkManager] Server status changed: {(IsServerReachable ? "Reachable" : "Unreachable")}");
                OnServerStatusChanged?.Invoke(IsServerReachable);
            }
        }

        #endregion

        #region API Request Methods with Retry Logic

        public void LoginWithRetry(LoginRequest loginRequest, System.Action<object, System.Exception> callback)
        {
            StartCoroutine(ExecuteWithRetry(
                () => authApi.LoginAsync(loginRequest, callback),
                "Login",
                callback
            ));
        }

        public void UpdateRatingWithRetry(UpdateRatingRequest ratingRequest, System.Action<object, System.Exception> callback)
        {
            StartCoroutine(ExecuteWithRetry(
                () => gameApi.UpdateRatingAsync(ratingRequest, callback),
                "UpdateRating",
                callback
            ));
        }

        public void GetRankingWithRetry(System.Action<object, System.Exception> callback)
        {
            StartCoroutine(ExecuteWithRetry(
                () => rankingApi.GetRankingAsync(callback),
                "GetRanking",
                callback
            ));
        }

        private IEnumerator ExecuteWithRetry(System.Action apiCall, string operationName, System.Action<object, System.Exception> callback)
        {
            if (!IsOnline)
            {
                if (enableOfflineMode)
                {
                    QueueRequestForLater(apiCall, operationName, callback);
                    Debug.Log($"[NetworkManager] {operationName} queued for offline processing");
                    return null;
                }
                else
                {
                    callback?.Invoke(null, new System.Exception("No internet connection"));
                    yield break;
                }
            }

            int attempts = 0;
            bool success = false;
            System.Exception lastException = null;

            while (attempts < maxRetries && !success)
            {
                attempts++;
                Debug.Log($"[NetworkManager] {operationName} attempt {attempts}/{maxRetries}");

                bool operationCompleted = false;
                bool operationSuccess = false;
                System.Exception operationException = null;

                // Wrap the callback to capture the result
                System.Action<object, System.Exception> wrappedCallback = (response, exception) =>
                {
                    operationSuccess = exception == null;
                    operationException = exception;
                    operationCompleted = true;
                    
                    if (operationSuccess || attempts >= maxRetries)
                    {
                        callback?.Invoke(response, exception);
                    }
                };

                try
                {
                    apiCall?.Invoke();
                }
                catch (System.Exception ex)
                {
                    operationException = ex;
                    operationCompleted = true;
                }

                // Wait for operation to complete
                float timeout = networkTimeoutSeconds;
                float elapsed = 0;
                
                while (!operationCompleted && elapsed < timeout)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (!operationCompleted)
                {
                    operationException = new System.TimeoutException($"{operationName} timed out after {timeout} seconds");
                }

                success = operationSuccess;
                lastException = operationException;

                if (!success && attempts < maxRetries)
                {
                    Debug.LogWarning($"[NetworkManager] {operationName} failed, retrying in {retryDelaySeconds} seconds. Error: {lastException?.Message}");
                    yield return new WaitForSeconds(retryDelaySeconds);
                    
                    // Exponential backoff
                    retryDelaySeconds *= 1.5f;
                }
            }

            if (!success)
            {
                Debug.LogError($"[NetworkManager] {operationName} failed after {maxRetries} attempts. Last error: {lastException?.Message}");
            }
        }

        #endregion

        #region Offline Queue Management

        private void QueueRequestForLater(System.Action apiCall, string operationName, System.Action<object, System.Exception> callback)
        {
            var request = new NetworkRequest
            {
                ApiCall = apiCall,
                OperationName = operationName,
                Callback = callback,
                Timestamp = System.DateTime.Now
            };

            offlineRequestQueue.Enqueue(request);
            
            // Limit queue size to prevent memory issues
            while (offlineRequestQueue.Count > 100)
            {
                var oldestRequest = offlineRequestQueue.Dequeue();
                Debug.LogWarning($"[NetworkManager] Dropping old offline request: {oldestRequest.OperationName}");
                oldestRequest.Callback?.Invoke(null, new System.Exception("Request expired in offline queue"));
            }
        }

        private IEnumerator ProcessOfflineQueue()
        {
            Debug.Log($"[NetworkManager] Processing {offlineRequestQueue.Count} offline requests");

            while (offlineRequestQueue.Count > 0 && IsOnline)
            {
                var request = offlineRequestQueue.Dequeue();
                
                // Check if request is too old (e.g., older than 10 minutes)
                if ((System.DateTime.Now - request.Timestamp).TotalMinutes > 10)
                {
                    Debug.LogWarning($"[NetworkManager] Skipping expired request: {request.OperationName}");
                    request.Callback?.Invoke(null, new System.Exception("Request expired"));
                    continue;
                }

                Debug.Log($"[NetworkManager] Processing offline request: {request.OperationName}");
                
                yield return StartCoroutine(ExecuteWithRetry(
                    request.ApiCall,
                    request.OperationName,
                    request.Callback
                ));

                // Small delay between requests to avoid overwhelming the server
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("[NetworkManager] Offline queue processing completed");
        }

        public void ClearOfflineQueue()
        {
            int count = offlineRequestQueue.Count;
            
            while (offlineRequestQueue.Count > 0)
            {
                var request = offlineRequestQueue.Dequeue();
                request.Callback?.Invoke(null, new System.Exception("Request cancelled"));
            }
            
            Debug.Log($"[NetworkManager] Cleared {count} requests from offline queue");
        }

        #endregion

        #region Public Methods

        public bool IsNetworkAvailable()
        {
            return IsOnline && IsServerReachable;
        }

        public int GetOfflineQueueSize()
        {
            return offlineRequestQueue.Count;
        }

        public void SetApiBaseUrl(string newBaseUrl)
        {
            if (apiBaseUrl != newBaseUrl)
            {
                apiBaseUrl = newBaseUrl;
                InitializeAPIClients();
                Debug.Log($"[NetworkManager] API base URL updated to: {newBaseUrl}");
            }
        }

        public void ForceServerHealthCheck()
        {
            StartCoroutine(CheckServerHealth());
        }

        #endregion

        private class NetworkRequest
        {
            public System.Action ApiCall;
            public string OperationName;
            public System.Action<object, System.Exception> Callback;
            public System.DateTime Timestamp;
        }
    }
}