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
            string authenticateUri = this.configuration.GetValue("DvlaConnection:AuthenticateUri");
            string userName = this.configuration.GetValue("DvlaConnection:UserName");
            string password = this.configuration.GetValue("DvlaConnection:Password");
            string apiKey = this.configuration.GetValue("DvlaConnection:ApiKey");
            string retrieveUri = this.configuration.GetValue("DvlaConnection:RetrieveUri");
            string testDriverNumber = this.configuration.GetValue("DvlaConnection:TestDriverNumber");

            if (!string.IsNullOrWhiteSpace(testDriverNumber)) driverNumber = testDriverNumber;

            try
            {
                LicensePhoto licensePhoto = null;

                using (var client = new HttpClient())
                {
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

                        string body = "{\"driverNumber\":\"" + driverNumber + "\"}";
                        StringContent content = new StringContent(body, Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authToken.IdToken);
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
                    else
                    {
                        licensePhoto = new LicensePhoto()
                        {
                            DriverNumber = driverNumber,
                            ErrorReason = authenticateResponse.ReasonPhrase
                        };
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