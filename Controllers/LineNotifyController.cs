using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LineNotifyPractice.Enums;
using LineNotifyPractice.Models.DB;
using LineNotifyPractice.Utilitys;
using Microsoft.AspNetCore.Mvc;
using static LineNotifyPractice.Controllers.LineLogInController;

namespace LineNotifyPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineNotifyController : ControllerBase
    {
        public LineNotifyController()
        {
        }

        /// <summary>
        /// Notification callback api
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        [HttpGet("Callback")]
        public async Task<ActionResult> GetCallback(string code, string state, [FromServices] SubscriberContext db)
        {
            try
            {
                TokenRequest tokenRequest = new TokenRequest()
                {
                    GrantType = "authorization_code",
                    Code = code,
                    ClientId = AppSettings.LineNotifyConfig.ClientId,
                    ClientSecret = AppSettings.LineNotifyConfig.ClientSecret,
                    RedirectUri = AppSettings.LineNotifyConfig.RedirectUrl
                };


                var tokenResponse = await HttpClientUtility.PostAsync<TokenRequest, TokenResponse>(AppSettings.LineNotifyConfig.TokenUrl, tokenRequest, RequestType.FormUrlEncoded);
                var userId = HttpContext.Session.GetString("userId");

                if (tokenResponse is not null && userId is not null)
                {
                    var subscriber = db.Subscribers.FirstOrDefault(c => c.LINEUserId == userId);
                    if (subscriber is not null)
                    {
                        subscriber.LINENotifyAccessToken = tokenResponse.AccessToken;
                    }
                }

                db.SaveChanges();
                return Redirect("/");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        /// <summary>
        /// Subscribe notification 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Subscribe")]
        public ActionResult Subscribe()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (userId is null)
            {
                return Content("Session expire please reLogin");
            }

            string authUrl = $"{AppSettings.LineNotifyConfig.AuthUrl}?response_type=code&client_id={AppSettings.LineNotifyConfig.ClientId}&redirect_uri={AppSettings.LineNotifyConfig.RedirectUrl}&state=12345&scope=notify";
            return Redirect(authUrl);
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        [HttpGet("Notify")]
        public async Task<ActionResult> Notify(string message, [FromServices] SubscriberContext db)
        {
            try
            {
                var userId = HttpContext.Session.GetString("userId");

                var request = new Dictionary<string, string>();
                request.Add("message", message);

                var subscribers = db.Subscribers;
                foreach (var subscribe in subscribers)
                {
                    if (subscribe.LINENotifyAccessToken is not null)
                    {
                        await HttpClientUtility.PostAsync<Dictionary<string, string>, TokenResponse>(AppSettings.LineNotifyConfig.NotifyUrl, request, RequestType.FormUrlEncoded, subscribe.LINENotifyAccessToken);
                    }
                }
                return Redirect("/");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Revoke subscribe
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        [HttpGet("Revoke")]
        public async Task<ActionResult> Revoke([FromServices] SubscriberContext db)
        {
            try
            {
                var userId = HttpContext.Session.GetString("userId");
                var request = new Dictionary<string, string>();
                var subscriber = db.Subscribers.FirstOrDefault(c => c.LINEUserId == userId);
                if (subscriber is not null && subscriber.LINENotifyAccessToken is not null)
                {
                    await HttpClientUtility.PostAsync<Dictionary<string, string>, TokenResponse>(AppSettings.LineNotifyConfig.RevokeUrl, request, RequestType.FormUrlEncoded, subscriber.LINENotifyAccessToken);
                    subscriber.LINENotifyAccessToken = null;
                    db.SaveChanges();
                    return Redirect("/");
                }

                return BadRequest();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }

    public partial class TokenRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }
    }

    public partial class TokenResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}