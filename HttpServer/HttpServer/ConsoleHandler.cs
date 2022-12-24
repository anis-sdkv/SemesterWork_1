using HttpServer.ServerLogic;
using HttpServer.Sessions;

namespace HttpServer;

internal static class ConsoleHandler
{
    private const string ConsoleCommands = "Введите команду:\n" + "1. start - запустить сервер\n" +
                                           "2. stop - остановить сервер\n" +
                                           "3. restart - перезапустить сервер\n" +
                                           "4. status - показать статус сервера\n" +
                                           "5. update - обновить настройки сервера\n" +
                                           "6. exit - завершить выполнение программы\n";

    private const string CommandNotFound = "Команда не найдена!";
    private const string AtTermination = "Программа завершена.";
    private const string AfterUpdate = "Настройки обновлены.";
    private const string Pointer = "   >>>   ";

    private static bool _isRunning = true;
    private static Server? _server;

    public static void Run(Server? server)
    {
        _server = server;
        HandleConsole("start", server);
        while (_isRunning)
            try
            {
                HandleConsole(Console.ReadLine()?.ToLower(), server);
            }
            catch (Exception e)
            {
                LogE(e);
            }
    }

    private static void HandleConsole(string? command, Server? server)
    {
        LogM(ConsoleCommands);
        switch (command)
        {
            case ("start"):
                server.Start();
                break;

            case ("stop"):
                server.Stop();
                break;

            case ("restart"):
                server.Restart();
                break;

            case ("status"):
                LogM(server.Status.ToString());
                break;

            case ("update"):
                server.UpdateSettings();
                LogM(AfterUpdate);
                break;

            case ("exit"):
                _isRunning = false;
                Console.Clear();
                LogM(AtTermination);
                break;

            default:
                LogM(CommandNotFound);
                break;
        }
    }

    public static void LogM(string message)
    {
        Console.WriteLine(DateTime.Now + Pointer + message);
    }

    public static void LogE(Exception ex)
    {
        Console.WriteLine(DateTime.Now + Pointer + "Error was caught:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine("Server status:   " + _server?.Status.ToString());
    }
}