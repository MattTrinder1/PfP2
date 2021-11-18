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
            //TODO: Get all these from config file.
            const string authenticateUri = "https://uat.driver-vehicle-licensing.api.gov.uk/thirdparty-access/v1/authenticate";
            const string pwd = "viWUCJJD4YgAlUXpaVJ@R54C";
            const string userName = "cumbriapolice";
            const string apiKey = "OeJHkYTswJK8Rlun5T5X6pIequYj0lC5XlsaT1xj";
            const string retrieveUri = "https://uat.driver-vehicle-licensing.api.gov.uk/driver-photo-at-the-roadside/v1/drivers/driver-details/s";

            try
            {
                LicensePhoto licensePhoto = null;

                using (var client = new HttpClient())
                {
                    string authBody = "{\"userName\":\"" + userName + "\",\"password\":\"" + pwd + "\"}";
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
                            //TODO: Exception
                        }
                    }
                    else
                    {
                        //TODO: Exception
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