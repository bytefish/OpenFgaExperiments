﻿using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using Microsoft.JSInterop;

namespace RebacExperiments.Blazor.Infrastructure
{
    public class CacheStorageAccessor : JSModule
    {
        private const string JAVASCRIPT_FILE = "./RebacExperiments.Blazor/js/CacheStorageAccessor.js";

        public CacheStorageAccessor(IJSRuntime js)
            : base(js, JAVASCRIPT_FILE)
        {
        }

        public async ValueTask PutAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            string requestMethod = requestMessage.Method.Method;
            string requestBody = await GetRequestBodyAsync(requestMessage);
            string responseBody = await responseMessage.Content.ReadAsStringAsync();

            await InvokeVoidAsync("put", requestMessage.RequestUri!, requestMethod, requestBody, responseBody);
        }

        public async ValueTask<string> PutAndGetAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            string requestMethod = requestMessage.Method.Method;
            string requestBody = await GetRequestBodyAsync(requestMessage);
            string responseBody = await responseMessage.Content.ReadAsStringAsync();

            await InvokeVoidAsync("put", requestMessage.RequestUri!, requestMethod, requestBody, responseBody);

            return responseBody;
        }

        public async ValueTask<string> GetAsync(HttpRequestMessage requestMessage)
        {
            string requestMethod = requestMessage.Method.Method;
            string requestBody = await GetRequestBodyAsync(requestMessage);
            string result = await InvokeAsync<string>("get", requestMessage.RequestUri!, requestMethod, requestBody);

            return result;
        }

        public async ValueTask RemoveAsync(HttpRequestMessage requestMessage)
        {
            string requestMethod = requestMessage.Method.Method;
            string requestBody = await GetRequestBodyAsync(requestMessage);

            await InvokeVoidAsync("remove", requestMessage.RequestUri!, requestMethod, requestBody);
        }

        public async ValueTask RemoveAllAsync()
        {
            await InvokeVoidAsync("removeAll");
        }
        private static async ValueTask<string> GetRequestBodyAsync(HttpRequestMessage requestMessage)
        {
            string requestBody = string.Empty;
            if (requestMessage.Content is not null)
            {
                requestBody = await requestMessage.Content.ReadAsStringAsync();
            }

            return requestBody;
        }
    }
}
