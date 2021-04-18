using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PdfCreationDemo
{
    public class Config
    {
        public string ApiKey { get; set; }
        public string Email { get; set; }
        public string Pw { get; set; }

        public static Config Read()
        {
            using var stream = new FileStream("config.xml", FileMode.Open);
            return new XmlSerializer(typeof(Config)).Deserialize(stream) as Config;
        }

        public void Save()
        {
            using var stream = new FileStream("config.xml", FileMode.OpenOrCreate);
            new XmlSerializer(typeof(Config)).Serialize(stream, this);
        }


    }
}
