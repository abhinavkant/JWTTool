using IdentityModel.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JWTTool
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            if (args.Any())
            {
                await ProcessCallback(args[0]);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        private static async Task ProcessCallback(string args)
        {
            var response = new AuthorizeResponse(args);
            if (!String.IsNullOrWhiteSpace(response.State))
            {
                Console.WriteLine($"Found state: {response.State}");
                var callbackManager = new CallbackManager(response.State);
                await callbackManager.RunClient(args);
            }
            else
            {
                Console.WriteLine("Error: no state on response");
            }
        }
    }
}
