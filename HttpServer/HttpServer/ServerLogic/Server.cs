using System.Net;
using HttpServer.DB;
using HttpServer.DB.ORM;

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
    private readonly int _accepts;
    public ServerStatus Status { get; private set; } = ServerStatus.Stopped;

    public Server(int accepts = 4)
    {
        _accepts = accepts;
        _listener = new HttpListener();
        _handler = new RequestHandler();
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        if (_listener.IsListening)
            throw new Exception("Stop the server before changing the settings.");

        var settings = ServerSettings.ReadFromJson("settings.json");
        var listeningUrl = $"http://{settings.IP}:{settings.Port}/";
        _listener.Prefixes.Clear();
        _listener.Prefixes.Add(listeningUrl);
        _handler.DataDirectory = settings.DataDirectory;
        R.SetRoutingBase(listeningUrl);

        MyORM.Init(settings.ConnectionString);
    }

    public void Start()
    {
        if (Status == ServerStatus.Started)
        {
            ConsoleHandler.LogM(ServerAlreadyStarted);
            return;
        }

        ConsoleHandler.LogM(ServerOnStart);
        try
        {
            _listener.Start();
            ConsoleHandler.LogM(ServerAfterStart);
            Status = ServerStatus.Started;
            Listen();
        }
        catch (HttpListenerException e)
        {
            ConsoleHandler.LogE(e);
        }
    }

    public void Stop()
    {
        if (Status == ServerStatus.Stopped)
        {
            ConsoleHandler.LogM(ServerNotStarted);
            return;
        }

        ConsoleHandler.LogM(ServerOnStop);
        try
        {
            _listener.Stop();
            ConsoleHandler.LogM(ServerAfterStop);
            Status = ServerStatus.Stopped;
        }
        catch (HttpListenerException e)
        {
            ConsoleHandler.LogE(e);
        }
    }

    public void Restart()
    {
        if (Status == ServerStatus.Started)
            Stop();
        Start();
    }

    private void Listen()
    {
        var sem = new Semaphore(_accepts, _accepts);

        while (_listener.IsListening)
        {
            sem.WaitOne();

            _listener.GetContextAsync().ContinueWith(async (t) =>
            {
                try
                {
                    sem.Release();
                    var context = await t;
                    await _handler.Handle(context);
                }
                catch (Exception ex)
                {
                    ConsoleHandler.LogE(ex);
                }
            });
        }
        
        // try
        // {
        //     context = await _listener.GetContextAsync();
        //     _handler.Handle(context);
        // }
        // catch (Exception e)
        // {
        //     ConsoleHandler.LogE(e);
        // }
    }

}
