using System;
using System.Collections;
using SimpleJson;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = JsonReader.ReadFile("_.txt");
            Console.WriteLine(json["access_token"]);
        }
    }
}
