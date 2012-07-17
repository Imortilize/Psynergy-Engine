using System;

namespace XnaGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>|
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
               // try
               // {
                    game.Run();
               // }
               // catch (Exception e)
                //{
                 //   int spug = 0;
                //}
            }
        }
    }
#endif
}

