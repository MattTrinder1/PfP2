using API.DataverseAccess;
using Common.Models.Dataverse;
using Common.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Headers;

namespace API.Controllers
{
    public class DvlaAuthToken
    {
        [JsonPropertyName("id-token")]
        public string IdToken { get; set; }
    }

    public class LicensePhoto
    {
        [JsonPropertyName("driverNumber")]
        public string DriverNumber { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        public string ErrorReason { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class VehicleTicketsController : ControllerBase
    {
        public VehicleTicketsController(
            ApiConfiguration configuration,
            DVDataAccessFactory dataAccessFactory,
            CacheOrchestrator cache,
            ILogger<PNBController> log) : base(configuration, dataAccessFactory, cache, log)
        {
        }

        [HttpGet("licensephoto/{driverNumber}")]
        public ActionResult<LicensePhoto> GetLicensePhoto(string driverNumber)
        {
            const string idTokenCacheKey = "DvlaIdToken";
            const string idTokenCachedOnCacheKey = "DvlaIdTokenCachedOn";

            int tokenValidMinutes = 30;
            string tokenValidMinutesAsString = this.configuration.GetValue("DvlaConnection:TokenValidMinutes");
            if (!string.IsNullOrWhiteSpace(tokenValidMinutesAsString))
            {
                if (!int.TryParse(tokenValidMinutesAsString, out tokenValidMinutes))
                {
                    throw new ArgumentException($"{tokenValidMinutesAsString} is not a valid TokenValidMinutes value.");
                }
            }

            string testDriverNumber = this.configuration.GetValue("DvlaConnection:TestDriverNumber");
            if (!string.IsNullOrWhiteSpace(testDriverNumber)) driverNumber = testDriverNumber;

            try
            {
                LicensePhoto licensePhoto = null;

                using (var client = new HttpClient())
                {
                    bool gotValidIdToken = false;
                    string idToken = null;
                    if (cache.TryGetValue(idTokenCacheKey, out idToken))
                    {
                        if (!string.IsNullOrWhiteSpace(idToken))
                        {
                            DateTime idTokenCachedOn;
                            if (cache.TryGetValue(idTokenCachedOnCacheKey, out idTokenCachedOn))
                            {
                                gotValidIdToken = (idTokenCachedOn > DateTime.Now - TimeSpan.FromMinutes(tokenValidMinutes));
                            }
                        }
                    }

                    if (!gotValidIdToken)
                    {
                        string authenticateUri = this.configuration.GetValue("DvlaConnection:AuthenticateUri", true);
                        string userName = this.configuration.GetValue("DvlaConnection:UserName", true);
                        string password = this.configuration.GetValue("DvlaConnection:Password", true);

                        string authBody = "{\"userName\":\"" + userName + "\",\"password\":\"" + password + "\"}";
                        StringContent authContent = new StringContent(authBody, Encoding.UTF8, "application/json");

                        var taskAuthenticate = Task.Run(() => client.PostAsync(authenticateUri, authContent));
                        taskAuthenticate.Wait();

                        HttpResponseMessage authenticateResponse = taskAuthenticate.Result;
                        if (authenticateResponse != null && authenticateResponse.IsSuccessStatusCode)
                        {
                            var taskReadAuthenticateResponse = Task.Run(() => authenticateResponse.Content.ReadAsStringAsync());
                            taskReadAuthenticateResponse.Wait();
                            string authTokenJson = taskReadAuthenticateResponse.Result;

                            DvlaAuthToken authToken = JsonSerializer.Deserialize<DvlaAuthToken>(authTokenJson);

                            idToken = authToken.IdToken;
                            gotValidIdToken = true;

                            cache.Set(idTokenCacheKey, idToken);
                            cache.Set(idTokenCachedOnCacheKey, DateTime.Now);
                        }
                        else
                        {
                            licensePhoto = new LicensePhoto()
                            {
                                DriverNumber = driverNumber,
                                ErrorReason = authenticateResponse.ReasonPhrase
                            };
                        }
                    }

                    if (gotValidIdToken)
                    {
                        string apiKey = this.configuration.GetValue("DvlaConnection:ApiKey", true);
                        string retrieveUri = this.configuration.GetValue("DvlaConnection:RetrieveUri", true);

                        string body = "{\"driverNumber\":\"" + driverNumber + "\"}";
                        StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(idToken);
                        content.Headers.Add("x-api-key", apiKey);
                        content.Headers.Add("timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffK"));

                        var taskGetPhoto = Task.Run(() => client.PostAsync(retrieveUri, content));
                        taskGetPhoto.Wait();

                        HttpResponseMessage getResponse = taskGetPhoto.Result;
                        if (getResponse != null && getResponse.IsSuccessStatusCode)
                        {
                            var taskReadGetResponse = Task.Run(() => getResponse.Content.ReadAsStringAsync());
                            taskReadGetResponse.Wait();
                            string responseJson = taskReadGetResponse.Result;

                            licensePhoto = JsonSerializer.Deserialize<LicensePhoto>(responseJson);
                        }
                        else
                        {
                            licensePhoto = new LicensePhoto()
                            {
                                DriverNumber = driverNumber,
                                ErrorReason = getResponse.ReasonPhrase
                            };
                        }
                    }
                }

                return licensePhoto;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiError(e.Message));
            }
        }
    }
}