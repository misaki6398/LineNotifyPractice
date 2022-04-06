using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LineNotifyPractice.Enums;
using LineNotifyPractice.Models;
using LineNotifyPractice.Models.DB;
using LineNotifyPractice.Utilitys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using LineNotifyPractice.Models;

namespace LineNotifyPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineLogInController : ControllerBase
    {
        public LineLogInController()
        {

        }

        /// <summary>
        /// Line login call back api
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        [HttpGet("Callback")]
        public async Task<ActionResult> GetLogInCallback(string code, string state, [FromServices] SubscriberContext db)
        {
            try
            {
                TokenRequest tokenRequest = new TokenRequest()
                {
                    Code = code,
                    ClientId = AppSettings.LineLoginConfig.ClientId,
                    ClientSecret = AppSettings.LineLoginConfig.ClientSecret,
                    GrantType = "authorization_code",
                    RedirectUri = AppSettings.LineLoginConfig.RedirectUrl
                };
                var tokenResponse = await HttpClientUtility.PostAsync<TokenRequest, TokenResponse>(AppSettings.LineLoginConfig.TokenUrl, tokenRequest, RequestType.FormUrlEncoded);
                var lineProfile = await HttpClientUtility.GetAsync<LineProfile>(AppSettings.LineLoginConfig.ProfileUrl, tokenResponse.AccessToken);
                HttpContext.Session.SetString("userId", lineProfile.UserId);

                var subscriberEntity = db.Subscribers.FirstOrDefault(c => c.LINEUserId == lineProfile.UserId);
                if (subscriberEntity is null)
                {
                    Subscriber subscriber = new Subscriber()
                    {
                        Username = lineProfile.DisplayName,
                        Photo = lineProfile.PictureUrl,
                        LINEUserId = lineProfile.UserId,
                        LINELoginAccessToken = tokenResponse.AccessToken,
                        LINELoginRefreshToken = tokenResponse.RefreshToken
                    };
                    db.Subscribers.Add(subscriber);
                }
                else
                {
                    subscriberEntity.Username = lineProfile.DisplayName;
                    subscriberEntity.Photo = lineProfile.PictureUrl;
                    subscriberEntity.LINEUserId = lineProfile.UserId;
                    subscriberEntity.LINELoginAccessToken = tokenResponse.AccessToken;
                    subscriberEntity.LINELoginRefreshToken = tokenResponse.RefreshToken;
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
        /// Line login auth
        /// </summary>
        /// <returns></returns>
        [HttpGet("Login")]
        public ActionResult Login()
        {
            string authUrl = $"{AppSettings.LineLoginConfig.AuthUrl}?response_type=code&client_id={AppSettings.LineLoginConfig.ClientId}&redirect_uri={AppSettings.LineLoginConfig.RedirectUrl}&state=12345&scope=profile";
            return Redirect(authUrl);
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
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }

        public partial class LineProfile
        {

            [JsonPropertyName("userId")]
            public string UserId { get; set; }
            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }
            [JsonPropertyName("statusMessage")]
            public string StatusMessage { get; set; }
            [JsonPropertyName("pictureUrl")]
            public string PictureUrl { get; set; }
        }


    }
}
