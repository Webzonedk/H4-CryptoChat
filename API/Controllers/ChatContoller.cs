using API.CryptoAndHash;
using API.Hubs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Cryptography;
using System.Text.Json;

namespace API.Controllers
{
    public class ChatContoller : Controller
    {

        public static List<string> messages = new();
        public string? userInput;
        public string? messageInput;
        public string? publicKey = null;
        public byte[]? key;
        public byte[]? iv;

        private readonly IConfiguration configuration;
        private CryptoRSA rsa = new();
        private CryptoAES aes = new();
        private Hashing hasher = new();

        private IHubContext<ChatHub> _chatHub;
        public ChatContoller(IConfiguration config, IHubContext<ChatHub> chatHub)
        {
            configuration = config;
            _chatHub = chatHub;
        }




        /// <summary>
        /// Creating Keys, Encrypting them, and sending the EAS keys to client, and saving them for other clients as well
        /// </summary>
        /// <param name="keyInput"></param>
        /// <returns></returns>
        [HttpPost("Encrypt")]
        public async Task<IActionResult> GetAESKeys([FromBody] string keyInput)
        {
            Aes tempAes = aes.CreateAES();
            byte[] byteKeyInput = JsonSerializer.Deserialize<byte[]>(keyInput);
            try
            {
                if (keyInput != null)
                {
                    AESKey aesKey = new();
                    RSAKey rsaKey = new();
                    (byte[] byteKey, byte[] byteIV) = rsa.EncryptRSA(byteKeyInput, tempAes);
                    key = byteKey;
                    iv = byteIV;
                    aesKey.Key = Convert.ToBase64String(byteKey);
                    aesKey.Iv = Convert.ToBase64String(byteIV);
                    string aesJson = JsonSerializer.Serialize(aesKey);

                    return StatusCode(200, aesJson);
                }

                return StatusCode(401, "Something went wrong");
            }
            catch (Exception)
            {

                return StatusCode(401, "Something went wrong");
            }
        }




        //[HttpPost("SendMessage")]
        //public async Task<IActionResult> SendMessage(string message)
        //{

        //    messages.Add(message);
        //    await _chatHub.Clients.All.SendAsync("SendMessage", message);
        //    return StatusCode(200);
        //}

    }
}
