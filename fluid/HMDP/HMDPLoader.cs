using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace fluid.HMDP
{
    class HMDPLoader
    {
        public string FileName { get; set; }

        public string UID
        {
            get { return Info.UID; }
        }

        public HMDPInformation Info { get { if (_info == null) return getHDMPInformation(); else return _info; } }
        private HMDPInformation _info = null;

        public HMDP2D Data2D { get { if (_data2d == null && Info.is2D) return getHDMP2D(); else return _data2d; } }
        private HMDP2D _data2d = null;

        private static readonly XNamespace XMLNamespace = "http://tempuri.org/HMDP-topLevel.xsd";
        private HMDPInformation getHDMPInformation()
        {
            //throw new NotImplementedException();

            using (ZipArchive zip = ZipFile.Open(FileName, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.Name == "hmdp.xml")
                    {
                        Stream s = entry.Open();
                        XmlReader reader = XmlReader.Create(s);
                        XDocument doc = XDocument.Load(reader);

                        ValidateXml(doc);
                        _info = ExtractInformation(doc);
                        return _info;
                    }
                }
                return null;
            }
        }

        private bool ValidateXml(XDocument doc)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(XMLNamespace.NamespaceName, @"HMDP\HMDP-topLevel.xsd");

            string msg = "";
            doc.Validate(schemas, (o, e) =>
            {
                msg += e.Message + Environment.NewLine;
            });
            return msg == "";
        }

        private HMDPInformation ExtractInformation(XDocument doc)
        {
            var q0 = (from b in doc.Descendants()
                      select new
                      {
                          uid = (string)b.Element(XMLNamespace + "UID")
                      }).First();

            var q1 = (from b in doc.Descendants(XMLNamespace + "GlobalHardwareData")
                      select new
                      {
                          name = (string)b.Element(XMLNamespace + "name"),
                          hwtype = (string)b.Element(XMLNamespace + "hwtype"),
                          description = (string)b.Element(XMLNamespace + "description"),
                          manufacturer = (string)b.Element(XMLNamespace + "manufacturer"),
                          url = (string)b.Element(XMLNamespace + "url")
                      }).First();

            var q2 = (from b in doc.Descendants(XMLNamespace + "Creator")
                      select new
                      {
                          cuid = (string)b.Element(XMLNamespace + "creatorUID"),
                          name = (string)b.Element(XMLNamespace + "name"),
                          description = (string)b.Element(XMLNamespace + "description"),
                          email = (string)b.Element(XMLNamespace + "email"),
                          url = (string)b.Element(XMLNamespace + "url")
                      }).First();

            HMDPInformation info = new HMDPInformation()
            {
                is2D = true,
                is3D = false,
                UID = q0.uid,
                HardwareName = q1.name,
                HardwareType = q1.hwtype,
                HardwareDescription = q1.description,
                HardwareManufacture = q1.manufacturer,
                HardwareUrl = q1.url,
                CreatorUID = q2.cuid,
                CreatorName = q2.name,
                CreatorDescription = q2.description,
                CreatorEmail = q2.email,
                CreatorUrl = q2.url
            };

            return info;
        }

        private HMDP2D getHDMP2D()
        {
            using (ZipArchive zip = ZipFile.Open(FileName, ZipArchiveMode.Read))
            {
                var data = new
                {
                    width = 0,
                    height = 0,
                    icon = "",
                    heat = "",
                    throughput = "",
                    solid = ""
                };
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.Name == "hmdp.xml")
                    {
                        Stream s = entry.Open();
                        XmlReader reader = XmlReader.Create(s);
                        XDocument doc = XDocument.Load(reader);

                        ValidateXml(doc);
                        var q1 = (from b in doc.Descendants(XMLNamespace + "Model2D")
                                  select new
                                  {
                                      width = (int)b.Element(XMLNamespace + "width"),
                                      height = (int)b.Element(XMLNamespace + "height"),
                                      icon = (string)b.Element(XMLNamespace + "icon-file"),
                                      heat = (string)b.Element(XMLNamespace + "heat-file"),
                                      throughput = (string)b.Element(XMLNamespace + "throughput-file"),
                                      solid = (string)b.Element(XMLNamespace + "solid-file")
                                  }).First();
                        data = q1;
                    }
                }
                HMDP2D ret = new HMDP2D();
                ret.x = data.width;
                ret.y = data.height;
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.FullName == @"2D/" + data.icon)
                    {
                        Stream s = entry.Open();
                        ret.Icon = Image.FromStream(s);
                    }
                    if (entry.FullName == @"2D/" + data.heat)
                    {
                        Stream s = entry.Open();
                        ret.Heat = Image.FromStream(s);
                    }
                    if (entry.FullName == @"2D/" + data.throughput)
                    {
                        Stream s = entry.Open();
                        ret.Throughput = Image.FromStream(s);
                    }
                    if (entry.FullName == @"2D/" + data.solid)
                    {
                        Stream s = entry.Open();
                        ret.Solid = Image.FromStream(s);
                    }
                }



                return ret;
            }
        }
    }
}
