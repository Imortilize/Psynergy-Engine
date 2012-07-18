using System;
using System.Reflection;

namespace Middleware
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            AppDomain ad = AppDomain.CreateDomain("Test");
            ad.AssemblyResolve += MyHandler;

            using (Game game = new Game())
            {
                game.Run();
            }
        }

        static Assembly MyHandler(object source, ResolveEventArgs e)
        {
            Console.WriteLine("Resolving {0}", e.Name);
            return Assembly.Load(e.Name);
        }
    }
#endif
}

