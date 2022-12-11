using System.Net;

namespace HttpServer.ServerLogic;

class Server
{
    private const string ServerAlreadyStarted = "Сервер уже запущен!";
    private const string ServerOnStart = "Запуск сервера...";
    private const string ServerAfterStart = "Сервер запущен";

    private const string ServerNotStarted = "Сервер еще не запущен!";
    private const string ServerOnStop = "Остановка сервера...";
    private const string ServerAfterStop = "Сервер остановлен";

    private readonly HttpListener _listener;
    private readonly RequestHandler _handler;
    private ServerSettings _settings;
    public ServerStatus Status { get; private set; } = ServerStatus.Stopped;

    public Server()
    {
        _listener = new HttpListener();
        _handler = new RequestHandler();
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        if (_listener.IsListening)
            throw new Exception("Stop the server before changing the settings.");

        _settings = ServerSettings.ReadFromJson("settings.json");
        var listeningUrl = $"http://{_settings.IP}:{_settings.Port}/";
        _listener.Prefixes.Clear();
        _listener.Prefixes.Add(listeningUrl);
        _handler.DataDirectory = _settings.DataDirectory;
        R.SetRoutingBase(listeningUrl);
    }

    public void Start()
    {
        if (Status == ServerStatus.Started)
        {
            ConsoleHandler.LogM(ServerAlreadyStarted);
            return;
        }

        ConsoleHandler.LogM(ServerOnStart);
        _listener.Start();
        ConsoleHandler.LogM(ServerAfterStart);
        Status = ServerStatus.Started;

        Listen();
    }

    public void Stop()
    {
        if (Status == ServerStatus.Stopped)
        {
            ConsoleHandler.LogM(ServerNotStarted);
            return;
        }

        ConsoleHandler.LogM(ServerOnStop);
        _listener.Stop();
        ConsoleHandler.LogM(ServerAfterStop);
        Status = ServerStatus.Stopped;
    }

    public void Restart()
    {
        if (Status == ServerStatus.Started)
            Stop();
        Start();
    }

    private async Task Listen()
    {
        while (_listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                await _handler.Handle(context);
            }
            catch (Exception ex)
            {
                ConsoleHandler.LogE(ex);
            }
        }
    }
}