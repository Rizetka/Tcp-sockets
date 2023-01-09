using System;
using System.Net.Sockets;
using System.Text;
using Tcp;

namespace ClientTcp
{
    class FileHandler
    {
        public Socket FileReciverSocket;
        public FolderManager fm;
        private bool isActive = false;
        private const int packetLength = 16;
        public FileHandler(Socket FileReciverSocket)
        {
            this.FileReciverSocket = FileReciverSocket;
            fm = new FolderManager();
            isActive = true;
        }
        public void startReceivingFile()
        {

            byte[] response = new byte[1024]; // буфер для ответа, для списка папок рабочего стола
            int bytes = 0; // количество полученных байт
            string[] list;
            StringBuilder builder = new StringBuilder();

            do
            {
                bytes = FileReciverSocket.Receive(response);
                builder.Append(Encoding.Unicode.GetString(response, 0, bytes));
            }
            while (FileReciverSocket.Available > 0);

            string respMsg = builder.ToString();

            if (respMsg.StartsWith("/fl/"))
            {
                Console.WriteLine(respMsg.Remove(0, 4));
                FileReciverSocket.Shutdown(SocketShutdown.Both);
                FileReciverSocket.Close();
            }
            if (respMsg.StartsWith("/ok/"))
            {
                byte[] getListMsg = new byte[8];
                getListMsg = Encoding.Unicode.GetBytes("/gl/");
                FileReciverSocket.Send(getListMsg);

                StringBuilder folderList = new StringBuilder();
                do // прием содержимого раб стола
                {
                    bytes = FileReciverSocket.Receive(response, response.Length, 0);
                    folderList.Append(Encoding.Unicode.GetString(response, 0, bytes));
                }
                while (FileReciverSocket.Available > 0);

                Console.WriteLine("\n------------------------------ Содеражимое рабочего стола отправителя ------------------------------");
                list = fm.getListFromFormatedFolderList(folderList.ToString());
                for (int i = 0; i < list.Length; i++)
                {
                    Console.WriteLine(list[i]);
                }


                while (isActive == true) // работает пока соединения не разорвано. Разрыв соединения тогда, когда получатель отказывается от пересылки файла. Код отказа - /br/
                {
                    string pathCommand = null;

                    Console.Write("[/go/][папка] - выбрать | [/ld/] - скачать | [/bk/](Назад) | [/br/](Отмена): "); pathCommand = Convert.ToString(Console.ReadLine());

                    if (pathCommand.StartsWith("/go/")) // Перейти в след папку /go/ [папка] полное название 
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes(pathCommand));

                        StringBuilder nextFolderContent = new StringBuilder();
                        do
                        {
                            bytes = FileReciverSocket.Receive(response, response.Length, 0);
                            nextFolderContent.Append(Encoding.Unicode.GetString(response, 0, bytes));
                        }
                        while (FileReciverSocket.Available > 0);

                        if (!nextFolderContent.ToString().StartsWith("/wp/"))
                        {
                            Console.WriteLine($"\n------------------------------ Содеражание папки [{ pathCommand.Substring(4) }] отправителя ------------------------------");
                            list = fm.getListFromFormatedFolderList(nextFolderContent.ToString());
                            for (int i = 0; i < list.Length; i++)
                            {
                                Console.WriteLine(list[i]);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Папки { pathCommand.Substring(4) } у отправителя не существует");
                        }
                    }
                    if (pathCommand.StartsWith("/ld/")) // Скачка /ld/[файл/папка] полное название, для файла [название].[расширение]
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes(pathCommand));

                        StringBuilder FileName = new StringBuilder();

                        do
                        {
                            bytes = FileReciverSocket.Receive(response, response.Length, 0);
                            FileName.Append(Encoding.Unicode.GetString(response, 0, bytes));
                        }
                        while (FileReciverSocket.Available > 0);

                        if (FileName.ToString().StartsWith("/st/"))
                        {
                            Console.WriteLine("Начианаем прием файла...");
                            fm.acceptFileByPackets(FileReciverSocket);
                        }
                        else
                        {
                            Console.WriteLine("Такого файла нет...");
                        }
                        // принять полное название фала/папки кол-во пакетов, откарыть поток для записи файла/файлов папки
                    }
                    if (pathCommand.StartsWith("/bk/")) // назад в предыдущую папку
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes("/bk/"));

                        StringBuilder prevFolderList = new StringBuilder(); // принимаем новую папку с файлами
                        do
                        {
                            bytes = FileReciverSocket.Receive(response, response.Length, 0);
                            prevFolderList.Append(Encoding.Unicode.GetString(response, 0, bytes));
                        }
                        while (FileReciverSocket.Available > 0);

                        if (!prevFolderList.ToString().StartsWith("/en/"))
                        {
                            Console.WriteLine("------------------------------Содеражание предыдущей папки отправителя------------------------------");
                            list = fm.getListFromFormatedFolderList(prevFolderList.ToString());
                            for (int i = 0; i < list.Length; i++)
                            {
                                Console.WriteLine(list[i]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Назад нельзя. Вы на рабочем столе отправителя");
                        }
                    }
                    if (pathCommand.StartsWith("/br/")) // отмена передачи
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes("/br/"));
                        //FileReciverSocket.Shutdown(SocketShutdown.Both);
                        FileReciverSocket.Close();
                        isActive = false;

                        Console.WriteLine("Передача отменена");
                    }
                }
            }
        }
        public void startSendingFile()
        {
            byte[] response = new byte[256]; // буфер для ответа, для кода, отдачи файла, и сигналов
            int bytes = 0; // количество полученных байт

            StringBuilder Key = new StringBuilder();

            do
            {
                bytes = FileReciverSocket.Receive(response, response.Length, 0);
                Key.Append(Encoding.Unicode.GetString(response, 0, bytes));
            }
            while (FileReciverSocket.Available > 0);

            Console.WriteLine("Ключ: " + Key.ToString());

            StringBuilder getListCmnd = new StringBuilder();

            do
            {
                bytes = FileReciverSocket.Receive(response, response.Length, 0);
                getListCmnd.Append(Encoding.Unicode.GetString(response, 0, bytes));
            }
            while (FileReciverSocket.Available > 0);

            Console.WriteLine(getListCmnd.ToString());

            if (getListCmnd.ToString() == "/gl/")
            {
                FileReciverSocket.Send(Encoding.Unicode.GetBytes(fm.getFormatedFolderList()));
            }

            //==========================================================================================================

            while (isActive == true) // работает пока соединения не разорвано. Разрыв соединения тогда, когда получатель отказывается от пересылки файла либо все файлы отправлены. Код прерывания  - /br/
            {
                StringBuilder finalCmndFromReciever = new StringBuilder();
                byte[] bytesCmnd = new byte[512];

                do
                {
                    bytes = FileReciverSocket.Receive(bytesCmnd, bytesCmnd.Length, 0);
                    finalCmndFromReciever.Append(Encoding.Unicode.GetString(bytesCmnd, 0, bytes));
                }
                while (FileReciverSocket.Available > 0);

                if (finalCmndFromReciever.ToString().StartsWith("/br/"))
                {
                    FileReciverSocket.Shutdown(SocketShutdown.Both);
                    FileReciverSocket.Close();

                    isActive = false;

                    Console.WriteLine("Клиент отказался от передачи");
                }
                if (finalCmndFromReciever.ToString().StartsWith("/bk/"))
                {
                    if (fm.back())
                    {
                        FileReciverSocket.Send(fm.getBytesFolderList(fm.getFormatedFolderList()));
                    }
                    else
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes("/en/"));
                    }

                }
                if (finalCmndFromReciever.ToString().StartsWith("/go/"))
                {
                    if (fm.addToPass(finalCmndFromReciever.ToString().Remove(0, 4)))
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes(fm.getFormatedFolderList()));
                    }
                    else
                    {
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes("/wp/"));
                    }
                }
                if (finalCmndFromReciever.ToString().StartsWith("/ld/")) // начинаем передаччу фала либо всей папки (пока файл)
                {
                    if (fm.isFileExist(finalCmndFromReciever.ToString().Substring(4)))
                    {
                        fm.sendFileByPackets(finalCmndFromReciever.ToString().Substring(4), FileReciverSocket);
                    }
                    else
                    {
                        Console.WriteLine("Получатель попробовал скачать несуществующий файл...");
                        FileReciverSocket.Send(Encoding.Unicode.GetBytes("/nf/"));
                    }
                }
            }
            //clientSocket.Shutdown(SocketShutdown.Both);
            //clientSocket.Close();
        }
    }
}
