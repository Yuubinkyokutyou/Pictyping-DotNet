# Pictyping.UnityClient.Api.AuthApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiAuthCrossDomainLoginGet**](AuthApi.md#apiauthcrossdomainloginget) | **GET** /api/Auth/cross-domain-login |  |
| [**ApiAuthGoogleCallbackGet**](AuthApi.md#apiauthgooglecallbackget) | **GET** /api/Auth/google/callback |  |
| [**ApiAuthLoginPost**](AuthApi.md#apiauthloginpost) | **POST** /api/Auth/login |  |
| [**ApiAuthMeGet**](AuthApi.md#apiauthmeget) | **GET** /api/Auth/me |  |
| [**ApiAuthRedirectToLegacyGet**](AuthApi.md#apiauthredirecttolegacyget) | **GET** /api/Auth/redirect-to-legacy |  |

<a id="apiauthcrossdomainloginget"></a>
# **ApiAuthCrossDomainLoginGet**
> void ApiAuthCrossDomainLoginGet (string token = null, string returnUrl = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiAuthCrossDomainLoginGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new AuthApi(config);
            var token = "token_example";  // string |  (optional) 
            var returnUrl = "returnUrl_example";  // string |  (optional) 

            try
            {
                apiInstance.ApiAuthCrossDomainLoginGet(token, returnUrl);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthApi.ApiAuthCrossDomainLoginGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAuthCrossDomainLoginGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAuthCrossDomainLoginGetWithHttpInfo(token, returnUrl);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthApi.ApiAuthCrossDomainLoginGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **token** | **string** |  | [optional]  |
| **returnUrl** | **string** |  | [optional]  |

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

<a id="apiauthgooglecallbackget"></a>
# **ApiAuthGoogleCallbackGet**
> void ApiAuthGoogleCallbackGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiAuthGoogleCallbackGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new AuthApi(config);

            try
            {
                apiInstance.ApiAuthGoogleCallbackGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthApi.ApiAuthGoogleCallbackGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAuthGoogleCallbackGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAuthGoogleCallbackGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthApi.ApiAuthGoogleCallbackGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
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

<a id="apiauthloginpost"></a>
# **ApiAuthLoginPost**
> void ApiAuthLoginPost (LoginRequest body = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiAuthLoginPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new AuthApi(config);
            var body = new LoginRequest(); // LoginRequest |  (optional) 

            try
            {
                apiInstance.ApiAuthLoginPost(body);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthApi.ApiAuthLoginPost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAuthLoginPostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAuthLoginPostWithHttpInfo(body);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthApi.ApiAuthLoginPostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **body** | [**LoginRequest**](LoginRequest.md) |  | [optional]  |

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

<a id="apiauthmeget"></a>
# **ApiAuthMeGet**
> void ApiAuthMeGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiAuthMeGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new AuthApi(config);

            try
            {
                apiInstance.ApiAuthMeGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthApi.ApiAuthMeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAuthMeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAuthMeGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthApi.ApiAuthMeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
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

<a id="apiauthredirecttolegacyget"></a>
# **ApiAuthRedirectToLegacyGet**
> void ApiAuthRedirectToLegacyGet (string targetPath = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class ApiAuthRedirectToLegacyGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new AuthApi(config);
            var targetPath = "targetPath_example";  // string |  (optional) 

            try
            {
                apiInstance.ApiAuthRedirectToLegacyGet(targetPath);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuthApi.ApiAuthRedirectToLegacyGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiAuthRedirectToLegacyGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiAuthRedirectToLegacyGetWithHttpInfo(targetPath);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuthApi.ApiAuthRedirectToLegacyGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **targetPath** | **string** |  | [optional]  |

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

