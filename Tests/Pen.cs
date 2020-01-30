using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tests
{
    public class Pen : IXmlSerializable
    {
        public string color;
        public float price;
        public string brand;

        public override string ToString()
        {
            return string.Format("A {0} pen, which costs {1} and was made by {2}.", color, price, brand);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            color = reader.GetAttribute("color");
            price = float.Parse(reader.GetAttribute("price"));
            brand = reader.GetAttribute("brand");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("color", color);
            writer.WriteAttributeString("price", price.ToString());
            writer.WriteAttributeString("brand", brand);
        }
    }
}
