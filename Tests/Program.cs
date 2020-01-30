using System;
using System.IO;
using System.Xml.Serialization;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Serialization();
            Console.ReadKey();
        }

        static void Serialization()
        {
            Pen pen = new Pen()
            {
                color = "blue",
                price = 1.0f,
                brand = "Bic"
            };

            XmlSerializer serializer = new XmlSerializer(typeof(Pen));
            using (TextWriter writer = new StreamWriter("pen.xml"))
            {
                serializer.Serialize(writer, pen);
            }

            using (Stream fs = new FileStream("pen.xml", FileMode.Open))
            {
                Pen theSamePen = (Pen)serializer.Deserialize(fs);
                Console.WriteLine(theSamePen.ToString());
            }
        }
    }
}
