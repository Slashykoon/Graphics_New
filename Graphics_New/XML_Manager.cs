using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Graphics_New
{
    public static class XML_Manager
    {
        public static XDocument Parameters { get; private set; }

        //If new run we load from xml file
        public static Record LoadSignalsConf(Record rec)
        {
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
            Parameters = new XDocument(XDocument.Load(currentPath));
   
            foreach (XElement tab in Parameters.Root.Elements("Signals").Elements())
            {
                //Signal sig = new Signal(int.Parse(tab.Attribute("index")?.Value),tab.Attribute("name")?.Value, tab.Attribute("color")?.Value, false);
                rec.AddSignal(int.Parse(tab.Attribute("index")?.Value), tab.Attribute("name")?.Value, tab.Attribute("color")?.Value, bool.Parse(tab.Attribute("isdetector")?.Value));
            }

            return rec;
        }
    }
}
