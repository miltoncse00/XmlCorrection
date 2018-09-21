using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XmlCorrection
{
    class Program
    {
        static void Main(string[] args)
        {

            var directory = new DirectoryInfo(ConfigurationManager.AppSettings["source"]);
            var files = directory.GetFiles();
            foreach (var fileInfo in files)
            {
                if (fileInfo.Extension.Contains("xml"))
                {
                    var fileStream = fileInfo.Open(FileMode.OpenOrCreate);
                    var document = XDocument.Load(fileStream);
                    document.Descendants()
                        .Where(e => (e.Attributes().All(a => a.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(a.Value))
                                     && string.IsNullOrWhiteSpace(e.Value)
                                     && e.Descendants().SelectMany(c => c.Attributes()).All(ca => ca.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(ca.Value))))
                        .Remove();
                    foreach (var element in document.Descendants())
                    {
                        foreach (var attribute in element.Attributes())
                        {
                            attribute.Value = attribute.Value.Trim();
                        }
                        foreach (var textNode in element.Nodes().OfType<XText>())
                        {
                            textNode.Value = textNode.Value.Trim();
                        }
                    }
                    string mapPath = Path.Combine(ConfigurationManager.AppSettings["output"], fileInfo.Name);

                    document.Save(mapPath);

                }
            }

        }
    }
}
