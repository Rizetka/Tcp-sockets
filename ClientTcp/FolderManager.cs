    using System;
    using System.IO;
    using System.Text;
    using System.Net.Sockets;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    namespace Tcp
    {
    //[Serializable]
    //public class Path
    //{
    //    public string path { get; set; }
    //    public Path()
    //    { }

    //    public Path(string path)
    //    {
    //        path = this.path;
    //    }
    //}
    class AllPathes
    {
        public List<string> pathes = new List<string>();
        public void DeserializAllPathes()
        {
            using (FileStream fs = new FileStream("pathes.xml", FileMode.Open))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
                pathes = (List<string>)formatter.Deserialize(fs);
            }
        }
        public void showAllPathes()
        {
            for(int i = 0; i < pathes.Count; i++)
            {
                Console.WriteLine($"{i}. {pathes[i]}");
            }   
        }
        public void AddPath(string newPath)
        {
            pathes.Add(newPath);

            using (FileStream fs = new FileStream("pathes.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
                formatter.Serialize(fs, pathes);
            }
        }
        public string this[int index]
        {
            get => pathes[index];
            set => pathes[index] = value;
        }
        public int Lenght()
        {
            return pathes.Count;
        }
    }
    class FolderManager
    {
        private string pathToDescktop;
        private FileStream ostream;
        private FileStream instream;
        public FolderManager()
        {
            pathToDescktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Console.WriteLine(pathToDescktop);
        }
        public string getFormatedFolderList() // рабочего стола folder1 | folder2...
        {
            DirectoryInfo DescDirectry = new DirectoryInfo(pathToDescktop);

            DirectoryInfo[] dirs = DescDirectry.GetDirectories();
            FileInfo[] files = DescDirectry.GetFiles();

            string formFoldList = "|";

            for (int i = 0; i < dirs.Length; i++)
            {
                formFoldList = formFoldList + dirs[i].Name + "|";
            }
            for (int i = 0; i < files.Length; i++)
            {
                formFoldList = formFoldList + files[i].Name + "|";
            }

            Console.WriteLine($"Получатель ищет по пути: {pathToDescktop}");

            return formFoldList;
        }
        public string[] getListFromFormatedFolderList(string formList)
        {
            string[] list;

            int counter = 0;

            for (int i = 0; i < formList.Length; i++)
            {
                if (formList[i] == '|')
                {
                    counter++;
                }
            }

            list = new string[counter];

            int listPos = 0;

            for (int i = 0; i < formList.Length; i++)
            {
                StringBuilder tmpStr = new StringBuilder();

                while (formList[i] != '|')
                {
                    tmpStr.Append(formList[i]);
                    i++;
                }
                list[listPos] = tmpStr.ToString();

                listPos++;
            }
            return list;
        }
        public byte[] getBytesFolderList(string foldList) // принимает форматированную строку и преобр. в массив байтов
        {
            byte[] byteList;
            byteList = Encoding.Unicode.GetBytes(foldList);

            return byteList;
        }
        //public void getFolderContent(string folder)
        //{
        //    pathToDescktop = pathToDescktop + @"\" + folder;

        //    DirectoryInfo DescDirectry = new DirectoryInfo(pathToDescktop);
        //    DirectoryInfo[] dd = new DirectoryInfo(pathToDescktop).GetDirectories();

        //    FileInfo[] fi;
        //    DirectoryInfo[] di;

        //    for (int i = 0; i < dd.Length; i++)
        //    {
        //        fi = DescDirectry.GetFiles();
        //        di = DescDirectry.GetDirectories();

        //        if (fi == null)
        //        {
        //            Console.WriteLine("Файлов нет");
        //        }
        //        if (di == null)
        //        {
        //            Console.WriteLine("Папок нет");
        //        }

        //        for (int j = 0; j < di.Length; j++)
        //        {
        //            Console.WriteLine(di[j].Name);
        //        }
        //        for (int k = 0; k < fi.Length; k++)
        //        {
        //            Console.WriteLine(fi[k].Name);
        //        }
        //    }

        //    Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");
        //} // просто вывод папок и файлов
        public bool isFileExist(string fileToCheck)
        {
            DirectoryInfo di = new DirectoryInfo(pathToDescktop);

            FileInfo[] files = di.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name == fileToCheck)
                {
                    return true;
                }
            }

            return false;
        }
        public bool addToPass(string path)
        {
            string tmpPath = pathToDescktop + @"\" + path;

            DirectoryInfo di = new DirectoryInfo(tmpPath);

            if (di.Exists)
            {
                pathToDescktop += @"\" + path;

                Console.WriteLine(pathToDescktop);

                return true;
            }
            else
            {
                return false;
            }
        }
        public bool back()
        {
            if (!pathToDescktop.EndsWith("Desktop"))
            {
                while (!pathToDescktop.EndsWith(@"\"))
                {
                    pathToDescktop = pathToDescktop.Remove(pathToDescktop.Length - 1);
                    Console.WriteLine(pathToDescktop);
                }
                pathToDescktop = pathToDescktop.Remove(pathToDescktop.Length - 1);
                Console.WriteLine(pathToDescktop);

                return true;
            }
            else
            {
                return false;
            }
        }
        public void sendFileByPackets(string fileToDownload, Socket FileSendSocket)
        {
            FileSendSocket.SendBufferSize = 128;
            FileSendSocket.ReceiveBufferSize = 128;
            string pathToFile = pathToDescktop + @"\" + fileToDownload;

            FileInfo file = new FileInfo(pathToFile);

            long fileLength = file.Length;
            string fileInfo = file.Name + @"|" + fileLength; // полное имя файла|кол-во байтов

            byte[] bytesFileInfoToSend = new byte[1024];
            byte[] bytesForConvertedFileInfo = Encoding.Unicode.GetBytes(fileInfo);//

            for (int i = 0; i < bytesForConvertedFileInfo.Length; i++)
            {
                bytesFileInfoToSend[i] = bytesForConvertedFileInfo[i];
            }

            Console.WriteLine($"Получатель скачивает файл [{ fileToDownload }]");
            Console.WriteLine($"Передается информация о файле [{ Encoding.Unicode.GetString(bytesFileInfoToSend) }]");

            FileSendSocket.Send(Encoding.Unicode.GetBytes("/st/"));

            for (int i = 0; i < 8; i++)
            {
                byte[] checkBytes = new byte[8];

                FileSendSocket.Receive(checkBytes);

                if (Encoding.Unicode.GetString(checkBytes) == "/nx/")
                {
                    byte[] tmp = new byte[128];

                    for (int j = 0; j < 128; j++)
                    {
                        tmp[j] = bytesFileInfoToSend[(i * 128) + j];
                    }
                    FileSendSocket.Send(tmp);
                }
            }
            Console.WriteLine("Информация передана");

            ostream = File.OpenRead(pathToFile);

            for (int offset = 0; offset < ostream.Length; offset += 128)
            {
                byte[] checkBytes = new byte[8];

                FileSendSocket.Receive(checkBytes);

                if (Encoding.Unicode.GetString(checkBytes) == "/nx/")
                {
                    byte[] filePakcetBytes = new byte[128];

                    ostream.Seek(offset, SeekOrigin.Begin);
                    ostream.Read(filePakcetBytes, 0, filePakcetBytes.Length);

                    FileSendSocket.Send(filePakcetBytes);
                }
            }

            Console.WriteLine($"Файл [{ file.Name }] отправлен.");
        }
        public void acceptFileByPackets(Socket FileRecieveSocket)
        {
            FileRecieveSocket.ReceiveBufferSize = 128;
            FileRecieveSocket.SendBufferSize = 128;

            byte[] bytesStart = new byte[8];

            StringBuilder fileInfo = new StringBuilder();

            AllPathes allPathes = new AllPathes();

            string pathToload = null;

            string fileName = null;
            
            int fileLength = 0;

            int option = 0;

            Console.WriteLine("Выбериет путь для скачки [1 - выбрать из прошлых][2 - ввести новый путь]");

            while ((option != 1) && (option != 2))
            {
                Console.Write(">> ");
                option = Convert.ToInt32(Console.ReadLine());

                if(option == 1)
                {
                    allPathes.DeserializAllPathes();
                    allPathes.showAllPathes();

                    while (!Directory.Exists(pathToload))
                    {
                        int index = -1;

                        Console.Write("Индекс пути: ");
                        index = Convert.ToInt32(Console.ReadLine());

                        if ((index < allPathes.Lenght()) &&  (index >= 0))
                        {
                            pathToload = allPathes[index];
                        }
                    }
                }
                if(option == 2)
                {
                    while (!Directory.Exists(pathToload))
                    {
                        Console.Write("Путь: ");
                        pathToload = Convert.ToString(Console.ReadLine());
                    }

                    allPathes.AddPath(pathToload);
                }
                else
                {
                    Console.WriteLine("Ввод от от 1 до 2");
                }             
            }
                    
            for (int i = 0; i < 8; i++)
            {
                FileRecieveSocket.Send(Encoding.Unicode.GetBytes("/nx/"));

                byte[] bytesRecievedFileInfo = new byte[128];

                FileRecieveSocket.Receive(bytesRecievedFileInfo);

                fileInfo.Append(Encoding.Unicode.GetString(bytesRecievedFileInfo));      
            }
            Console.WriteLine(fileInfo.ToString());

            string flInf = fileInfo.ToString();
            int devIndex = flInf.IndexOf('|');

            fileName = flInf.Substring(0, devIndex);
            fileLength = Convert.ToInt32(flInf.Substring(devIndex + 1));

            instream = new FileStream($"{ pathToload }\\{ fileName }", FileMode.Append);

            for (int i = 0; i < fileLength / 128; i++)
            {
                FileRecieveSocket.Send(Encoding.Unicode.GetBytes("/nx/"));

                byte[] bytesFileContent = new byte[128];

                FileRecieveSocket.Receive(bytesFileContent);

                instream.Write(bytesFileContent, 0, bytesFileContent.Length);
            }
            instream.Close();

            Console.WriteLine($"Файл [{ fileName }] скачан.");
        }
    }
}

        //-----------------------------------------------------------------------
        //      файл разбивается на пакеты в 8 байт и собирается во втором
        //-----------------------------------------------------------------------

        //string path = @"C:\Users\jagac\Desktop\SomeDir2";
        //DirectoryInfo dirInfo = new DirectoryInfo(path);
        //if (!dirInfo.Exists)
        //{
        //    dirInfo.Create();
        //}

        //using (FileStream fstream = File.OpenRead($"{path}\\note.txt"))
        //{
        //    for (int offset = 0; offset<fstream.Length; offset += 16)
        //    {
        //        fstream.Seek(offset, SeekOrigin.Begin); //+ 5 символов с начала потока
        //        byte[] array = new byte[16];
        //        fstream.Read(array, 0, array.Length);

        //        string textFromFile = System.Text.Encoding.UTF8.GetString(array);
        //        Console.WriteLine($"Текст файла: {textFromFile}");

        //        using (FileStream instream = new FileStream($"{path}\\noteIn.txt", FileMode.Append))
        //        {
        //            // запись массива байтов в файл
        //            instream.Write(array, 0, array.Length);
        //            Console.WriteLine("Текст записан в файл");
        //        }
        //    }
        //}


