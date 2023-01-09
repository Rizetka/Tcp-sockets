using System;

namespace ClientTcp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите IP сервера: ");
            //Convert.ToString(Console.Read());
            Client cl = new Client("26.157.55.224");

            Console.WriteLine("/gc/ - Сгенерировать ссылку \n/gf/[ссылка] - начать скачку файла");

            while (true)
            {
                Console.Write(">>");
                string cmnd = Convert.ToString(Console.ReadLine());

                if (cmnd.Length >= 4)
                {
                    cl.HandleCommand(cmnd);
                }
                else
                {
                    Console.WriteLine("Пустая команда...");
                }
            }
        }
    }
}