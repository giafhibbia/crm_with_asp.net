using Microsoft.AspNetCore.Mvc;
using MyAuthDemo.Data;
using MyAuthDemo.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;

namespace MyAuthDemo.Controllers
{
    [Route("Notification")]
    public class NotificationController : Controller
    {
        private readonly AppDbContext _db;

        public NotificationController(AppDbContext db)
        {
            _db = db;
        }

        // POST: /Notification/SaveFcmToken
        [HttpPost("SaveFcmToken")]
        public IActionResult SaveFcmToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { message = "Token is required." });
            }

            // Simpan ke database (opsional)
            var token = new FcmToken { Token = request.Token };
            _db.FcmTokens.Add(token);
            _db.SaveChanges();

            return Ok(new { message = "Token saved successfully" });
        }

        public class TokenRequest
        {
            public string Token { get; set; }
        }

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification()
        {
            var token = _db.FcmTokens
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Token)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "No FCM token found." });
            }

            // 🔐 Load credentials from your service account file
            var credential = GoogleCredential
                .FromFile("firebase-service-account.json") // pastikan path benar
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            var payload = new
            {
                message = new
                {
                    token = token,
                    notification = new
                    {
                        title = "Hello from ASP.NET",
                        body = "This is a test notification from Firebase HTTP v1"
                    }
                }
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            // Ganti project ID sesuai milikmu
            var projectId = "aromacology-cc25d";
            var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

            var response = await client.PostAsync(
                url,
                new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }


    }
}
