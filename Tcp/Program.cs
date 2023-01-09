using System;

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
    //class AllPathes
    //{
    //    public List<string> pathes = new List<string>();
    //    public AllPathes()
    //    {
    //        using (FileStream fs = new FileStream("pathes.xml", FileMode.Open))
    //        {
    //            XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
    //            pathes = (List<string>)formatter.Deserialize(fs);

    //            foreach (string p in pathes)
    //            {
    //                Console.WriteLine(p);
    //            }
    //        }
    //    }
    //    public void AddPath(string newPath)
    //    {
    //        pathes.Add(newPath);

    //        using (FileStream fs = new FileStream("pathes.xml", FileMode.OpenOrCreate))
    //        {
    //            XmlSerializer formatter = new XmlSerializer(typeof(List<string>));
    //            formatter.Serialize(fs, pathes);
    //        }
    //    }
    //    public string this[int index]
    //    {
    //        get => pathes[index];
    //        set => pathes[index] = value;
    //    }
    //}

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        AllPathes pathes = new AllPathes();

    //        Console.ReadLine();
    //    }
    //}
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: Program name.exe [port] [IP]");
            }
            else
            {
                new ServerMain(Convert.ToInt32(args[0]), Convert.ToString(args[1]));
            }
        }
    }
}
