using fluid.D2Draw;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace fluid.HMDP.Preset
{
    class PresetLoader
    {
        public List<PresetItem> Items { get { return ItemList; } }

        List<PresetItem> ItemList = new List<PresetItem>();

        private static readonly XNamespace XMLNamespace = "http://tempuri.org/HMDP-preset.xsd";

        public void Init(string filename)
        {
            DirSearch(Directory.GetParent(filename).FullName);

            XmlReader reader = XmlReader.Create(filename);
            XDocument doc = XDocument.Load(reader);

            ValidateXml(doc);

            processXML(doc);

            ItemList.RemoveAll(x => !x.Processed);
        }



        private void processXML(XDocument doc)
        {
            List<PresetItem> mergeList = new List<PresetItem>();
            foreach (XElement xe in doc.Descendants(XMLNamespace + "Item"))
            {
                foreach (PresetItem item in ItemList)
                {
                    if (item.Loader.UID == xe.Element(XMLNamespace + "UID").Value)
                    {
                        PresetItem tmp;
                        if (item.Processed)
                        {
                            tmp = new PresetItem() { File = item.File, Processed = false };
                            mergeList.Add(tmp);
                        }
                        else
                        {
                            tmp = item;
                        }
                        tmp.Processed = true;
                        tmp.X = Double.Parse(xe.Element(XMLNamespace + "x").Value, CultureInfo.InvariantCulture);
                        tmp.Y = Double.Parse(xe.Element(XMLNamespace + "y").Value, CultureInfo.InvariantCulture);
                        tmp.Z = Double.Parse(xe.Element(XMLNamespace + "z").Value, CultureInfo.InvariantCulture);
                        tmp.Rotation = Double.Parse(xe.Element(XMLNamespace + "rotation").Value, CultureInfo.InvariantCulture);
                    }
                }
            }
            ItemList.AddRange(mergeList);
        }

        private void DirSearch(string sDir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (f.EndsWith(".hmdp"))
                    {
                        ItemList.Add(new PresetItem() { File = f, Processed = false });
                    }
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private bool ValidateXml(XDocument doc)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(XMLNamespace.NamespaceName, @"HMDP\Preset\HMDP-preset.xsd");

            string msg = "";
            doc.Validate(schemas, (o, e) =>
            {
                msg += e.Message + Environment.NewLine;
            });
            return msg == "";
        }

        public static void buildXml(ListBox.ObjectCollection items, string fileName)
        {

            List<XElement> itemsXml = new List<XElement>();
            foreach (Object o in items)
            {
                if (o is Drawable2DHMDP)
                {
                    Drawable2DHMDP model = (Drawable2DHMDP)o;
                    itemsXml.Add(
                        new XElement(XMLNamespace + "Item",
                            new XElement(XMLNamespace + "UID", model.Info.UID),
                            new XElement(XMLNamespace + "x", model.X),
                            new XElement(XMLNamespace + "y", model.Y),
                            new XElement(XMLNamespace + "z", model.Z),
                            new XElement(XMLNamespace + "rotation", model.R)
                        )
                     );
                }
            }
            XDocument doc =
                new XDocument(
                    new XElement(XMLNamespace + "HMDP-Preset",
                        new XElement(XMLNamespace + "is2D", "true"),
                        new XElement(XMLNamespace + "is3D", "false"),
                        itemsXml
                    )
                );
            doc.Save(fileName + ".xml");
        }

    }
}
