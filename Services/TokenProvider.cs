//using Microsoft.AspNetCore.Authentication.BearerToken;
//using Microsoft.JSInterop;
//using System.Net.Http;
//using System.Text.Json;

//namespace TaskerBCAPI.Services
//{
//    public class TokenProvider
//    {
//        private static readonly string _localStorageKey = "taskerAuthToken";

//        private readonly HttpClient _http;
//        private readonly IJSRuntime _js;

//        private AccessTokenResponse? _cachedToken;

//        public TokenProvider(IHttpClientFactory httpClientFactory, IConfiguration config, IJSRuntime js)
//        {
//            _http = httpClientFactory.CreateClient();
//            _js = js;

//            _http.BaseAddress = new Uri(config["ApiUrl"]!);

//        }

//        public async Task StoreToken(AccessTokenResponse accessToken)
//        {
//            _cachedToken = accessToken;

//            _cachedToken.ExpiresAt ??= DateTime.Now.AddSeconds(_cachedToken.ExpiresIn);

//            string json = JsonSerializer.Serialize(_cachedToken);
//            await _js.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
//        }

//    }
//}
