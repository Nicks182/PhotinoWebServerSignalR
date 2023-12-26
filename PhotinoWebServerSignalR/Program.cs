

using PhotinoNET;

namespace PhotinoWebServerSignalR
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CustomFileServer
            .CreateStaticFileServer(new CustomFileServerOptions
            {
                Args = args,
                PortStart = 8000,
                PortRange = 100,
                WebRootFolder = "wwwroot",
                AllowLocalAccess = true,
            },
            out string baseURL)
            .RunAsync();


            PhotinoWindow Wind = new PhotinoWindow();
            Wind.Load(baseURL);

            #region Addition functionality
            Wind.RegisterWindowCreatedHandler(Win_WindowCreated);

            // This will not work if you access the app from a normal browser.
            // We are using SignalR and don't need this anymore.
            //Wind.RegisterWebMessageReceivedHandler((object sender, string message) =>
            //{
            //    var window = (PhotinoWindow)sender;

            //    // Send a message back the to JavaScript event handler.
            //    window.SendWebMessage("C# reveived the following message from the browser: " + message);
            //});
            #endregion Addition functionality

            Wind.WaitForClose();
        }

        private static void Win_WindowCreated(object? sender, EventArgs e)
        {
            // Using this event seems to be the only way to get Maximized to work.
            // Trying to set it before the window is created is a bug in Photino 2.5.2 which will be fixed in newer versions.
            (sender as PhotinoWindow).SetMaximized(true);
        }
    }
}
