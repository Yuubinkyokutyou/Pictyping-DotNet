# Pictyping.UnityClient.Api.RankingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiRankingGet**](RankingApi.md#apirankingget) | **GET** /api/Ranking |  |
| [**ApiRankingUserUserIdGet**](RankingApi.md#apirankinguseruseridget) | **GET** /api/Ranking/user/{userId} |  |
| [**ApiRankingUserUserIdPut**](RankingApi.md#apirankinguseruseridput) | **PUT** /api/Ranking/user/{userId} |  |

<a id="apirankingget"></a>
# **ApiRankingGet**
> void ApiRankingGet (int? count = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiRankingGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new RankingApi(config);
            var count = 100;  // int? |  (optional)  (default to 100)

            try
            {
                apiInstance.ApiRankingGet(count);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingApi.ApiRankingGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRankingGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiRankingGetWithHttpInfo(count);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingApi.ApiRankingGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **count** | **int?** |  | [optional] [default to 100] |

### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apirankinguseruseridget"></a>
# **ApiRankingUserUserIdGet**
> void ApiRankingUserUserIdGet (int userId)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiRankingUserUserIdGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new RankingApi(config);
            var userId = 56;  // int | 

            try
            {
                apiInstance.ApiRankingUserUserIdGet(userId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingApi.ApiRankingUserUserIdGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRankingUserUserIdGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiRankingUserUserIdGetWithHttpInfo(userId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingApi.ApiRankingUserUserIdGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **int** |  |  |

### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apirankinguseruseridput"></a>
# **ApiRankingUserUserIdPut**
> void ApiRankingUserUserIdPut (int userId, UpdateRatingRequest body = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiRankingUserUserIdPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new RankingApi(config);
            var userId = 56;  // int | 
            var body = new UpdateRatingRequest(); // UpdateRatingRequest |  (optional) 

            try
            {
                apiInstance.ApiRankingUserUserIdPut(userId, body);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingApi.ApiRankingUserUserIdPut: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiRankingUserUserIdPutWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiRankingUserUserIdPutWithHttpInfo(userId, body);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingApi.ApiRankingUserUserIdPutWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **int** |  |  |
| **body** | [**UpdateRatingRequest**](UpdateRatingRequest.md) |  | [optional]  |

### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

