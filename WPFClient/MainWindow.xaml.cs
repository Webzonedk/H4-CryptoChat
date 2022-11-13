using WPFClient.CryptoAndHash;
using WPFClient.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hubConnection;
        CryptoRSA rsa = new();
        CryptoAES cryptoAes = new();
        AESKey aes = new();

        public string name = "Annonymous";
        public byte[] byteKey;
        public byte[] byteIv;
        public string apiUrl = "https://localhost:7273";
        public List<string> messageArchive = new(); //DEBUG I know i wouldn't keep a log of encrypted chat

        public MainWindow()
        {
            InitializeComponent();
            //Connecting to server
            hubConnection = new HubConnectionBuilder()

                .WithUrl(url: $"{apiUrl}/chathub")
                .WithAutomaticReconnect()
                .Build();

            // popping message if server is trying to reconnect
            hubConnection.Reconnecting += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    string? newMessage = "Atempting to reconnect to server...";
                    messages.Items.Add(newMessage);
                });
                return Task.CompletedTask;
            };



            hubConnection.Reconnected += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    string? newMessage = "Reconnected to server";
                    messages.Items.Clear();
                    messages.Items.Add(newMessage);
                });
                return Task.CompletedTask;
            };



            hubConnection.Closed += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    string? newMessage = "Connection is now closed";
                    messages.Items.Add(newMessage);
                    openConnection.IsEnabled = true;
                    sendMessage.IsEnabled = false;
                });
                return Task.CompletedTask;
            };
        }



        /// <summary>
        /// Method to open connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenConnection_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                RSAParameters publicByteKey = rsa.AssignNewKey();
                string json = JsonSerializer.Serialize(publicByteKey.Modulus);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = client.PostAsJsonAsync("/Encrypt", json).Result;

                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    //AESKey obj = new();
                    aes = JsonSerializer.Deserialize<AESKey>(result);
                    byte[] encryptedByteKey = Convert.FromBase64String(aes.Key);
                    byte[] encryptedByteIv = Convert.FromBase64String(aes.Iv);

                    byteKey = rsa.DecryptRSA(encryptedByteKey);
                    byteIv = rsa.DecryptRSA(encryptedByteIv);
                }

            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }



            //Receiving messages, decrypting them using the stored AES key an IV
            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        byte[] newMessage = Convert.FromBase64String(message);
                        string decryptedMessage = cryptoAes.DecryptAes(newMessage, byteKey, byteIv);
                        messages.Items.Add(decryptedMessage);
                    });

                }
                catch (Exception ex)
                {
                    messages.Items.Add(ex.Message);
                }
            });

            //Protection if server can not connect
            try
            {
                await hubConnection.StartAsync();
                messages.Items.Add(newItem: "Connection Started");
                openConnection.IsEnabled = false;
                sendMessage.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }


        /// <summary>
        /// Sending message after encrypting is first anstoring the encrypted message to debug messageArchive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] encryptedMessage = cryptoAes.EncryptAES($"{name}: {messageInput.Text}", byteKey, byteIv);
                string message = Convert.ToBase64String(encryptedMessage);
                messageArchive.Add(message);
                await hubConnection.InvokeAsync(methodName: "SendMessage", message);
            }
            catch (Exception ex)
            {
                messages.Items.Add(ex.Message);
            }
        }
    }
}
