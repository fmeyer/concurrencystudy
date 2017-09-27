using System;
using System.Net;
using AddServer.Server;


/* For brevety I'm creating a logger class, which in a real project should be delecated 
to an external library - For now it makes the code more readeable*/
internal class Logger
{
    public static void Info(string message)
    {
        Console.WriteLine(message);
    }

    public static void Error(string message) 
    {
        Console.Error.WriteLine(message);
    }
}

namespace Server
{
    internal class App
    {
        private static void Main(string[] args)
        {
            new CalculatorService(IPAddress.Any, 1233).Listen();
        }
    }
}