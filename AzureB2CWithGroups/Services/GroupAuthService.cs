using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AzureB2CWithGroups.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace AzureB2CWithGroups.Services
{
    public class GroupAuthService: IAuthorizationService
    {
        private static string aadInstance = "https://login.microsoftonline.com/";
        private static string aadGraphEndpoint = "https://graph.windows.net/";
        private static string aadGraphVersion = "api-version=1.6";

        private GraphConfig config;
        private AuthenticationContext authContext;
        private ClientCredential credential;
        private ILogger log;
        //private ActiveDirectoryClient client;
        public GroupAuthService(IOptions<GraphConfig> graphConfig, ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger("GroupAuthService");
            config = graphConfig.Value;
            authContext = new AuthenticationContext(aadInstance + config.Tenant);
            credential = new ClientCredential(config.ClientId,config.ClientSecret);
//            client = new ActiveDirectoryClient(new Uri("https://login.microsoftonline.com/" + config.Tenant),
//                async () =>
//                {
//                    AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.windows.net", credential);
//                    return result.AccessToken;
//                });

        }


        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            //first thing's first -- kick 'em out if they're not authenticated!
            if (!user.Identity.IsAuthenticated) return AuthorizationResult.Failed();

            var userId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            /*
             * Basically we're going to go through the list of role requirements and compare it to each group,
             * and give the green light to the first match we find.
             *
             * Your business/application logic may vary.
             */
            AuthorizationResult authorize = AuthorizationResult.Failed();
            foreach (var requirement in requirements)
            {
                if (!(requirement is RolesAuthorizationRequirement)) continue;
                var req = requirement as RolesAuthorizationRequirement;
                var groupResp = await SendGraphGetRequest("/groups", "");
                log.LogInformation(groupResp);
                var root = JsonConvert.DeserializeObject<GroupRoot>(groupResp);

                foreach (var item in root.value)
                {
                    if (!req.AllowedRoles.Any(o => String.Equals(o, item.displayName,
                        StringComparison.CurrentCultureIgnoreCase))) continue;

                    if (await IsMemberOf(userId, item.objectId)) return AuthorizationResult.Success();

                }

            }

            return authorize;
        }

        //this one doesn't get called in our current application scope ¯\_(ツ)_/¯
        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
        {
            var groups = await SendGraphGetRequest("/groups", "");
            log.LogInformation(groups);
            return AuthorizationResult.Success();
        }

        /// <summary>
        /// See if the object is a member of the AD group.
        /// </summary>
        /// <param name="objectId">The object (typically a user but it could be group, contact or service principal).</param>
        /// <param name="groupId">The AD group ID.</param>
        /// <returns><c>true</c> if it exists, otherwise, <c>false</c>.</returns>
        private async Task<bool> IsMemberOf(string objectId, string groupId)
        {
            AuthenticationResult result = await authContext.AcquireTokenAsync(aadGraphEndpoint, credential);

            var query = new MemberQuery {groupId = groupId, memberId = objectId};

            HttpClient http = new HttpClient();
            string url = $"{aadGraphEndpoint}{config.Tenant}/isMemberOf?{aadGraphVersion}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");

            var resp = await http.SendAsync(request);

            var queryResponse =
                JsonConvert.DeserializeObject<MemberQueryResponse>(await resp.Content.ReadAsStringAsync());

            return queryResponse?.value ?? false;

        }

        private async Task<string> SendGraphGetRequest(string api, string query)
        {
            // First, use ADAL to acquire a token using the app's identity (the credential)
            // The first parameter is the resource we want an access_token for; in this case, the Graph API.
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://graph.windows.net", credential);

            // For B2C user management, be sure to use the 1.6 Graph API version.
            HttpClient http = new HttpClient();
            string url = "https://graph.windows.net/" + config.Tenant + api + "?" + aadGraphVersion;
            if (!string.IsNullOrEmpty(query))
            {
                url += "&" + query;
            }

            log.LogInformation("GET " + url);
            log.LogInformation("Authorization: Bearer " + result.AccessToken.Substring(0, 80) + "...");
            log.LogInformation("");

            // Append the access token for the Graph API to the Authorization header of the request, using the Bearer scheme.
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            HttpResponseMessage response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                object formatted = JsonConvert.DeserializeObject(error);
                throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted, Formatting.Indented));
            }

            log.LogInformation((int)response.StatusCode + ": " + response.ReasonPhrase);
            log.LogInformation("");

            return await response.Content.ReadAsStringAsync();
        }

    }
}
