using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace JWTTool
{
    public partial class Form1 : Form
    {
        private readonly string _customSchema;

        public Form1()
        {
            InitializeComponent();
            _customSchema = "jwttool";
            new RegistryConfig(_customSchema).Configure();
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            string redirectUri = string.Format(_customSchema + "://callback");

            var options = new OidcClientOptions
            {
                Authority = "https://demo.duendesoftware.com",
                ClientId = "interactive.public",
                Scope = "openid profile email api offline_access",
                RedirectUri = redirectUri,
            };

            var client = new OidcClient(options);
            var state = await client.PrepareLoginAsync();

            var callbackManager = new CallbackManager(state.State);

            // open system browser to start authentication
            Process.Start(new ProcessStartInfo
            {
                FileName = state.StartUrl,
                UseShellExecute = true
            });

            Log("Running callback manager");

            var response = await callbackManager.RunServer();

            var result = await client.ProcessResponseAsync(response, state);

            if (result.IsError)
            {
                Log(string.Format("\n\nError:\n{0}", result.Error));
            }
            else
            {
                Log("\n\nClaims:");
                foreach (var claim in result.User.Claims)
                {
                    Log(string.Format("{0}: {1}", claim.Type, claim.Value));
                }


                Log(string.Format("Access token:\n{0}", result.AccessToken));

                if (!string.IsNullOrWhiteSpace(result.RefreshToken))
                {
                    Log(string.Format("Refresh token:\n{0}", result.RefreshToken));
                }
            }

        }

        private void Log(string msg)
        {
            Output.Text += string.Format($"{msg}{Environment.NewLine}");
        }
    }
}
