using HttpServer.ServerLogic;
using System.Net;

namespace HttpServer;

class Program
{
    static void Main(string[] args)
    {
        var server = new Server();
        ConsoleHandler.Run(server);
    }
}