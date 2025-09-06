# Pictyping.UnityClient.Api.PictypingAPIApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**HealthGet**](PictypingAPIApi.md#healthget) | **GET** /health |  |

<a id="healthget"></a>
# **HealthGet**
> void HealthGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Pictyping.UnityClient.Api;
using Pictyping.UnityClient.Client;
using Pictyping.UnityClient.Model;

namespace Example
{
    public class HealthGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure API key authorization: Bearer
            config.AddApiKey("Authorization", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("Authorization", "Bearer");

            var apiInstance = new PictypingAPIApi(config);

            try
            {
                apiInstance.HealthGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PictypingAPIApi.HealthGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the HealthGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.HealthGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PictypingAPIApi.HealthGetWithHttpInfo: " + e.Message);
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

