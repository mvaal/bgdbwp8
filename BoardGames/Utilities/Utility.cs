using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BoardGames.Utilities
{
    public class Utility
    {
        public static string GetAttributeFromManifest(string attribute)
        {
            try
            {
                return XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute(attribute).Value;
            }
            catch (FileNotFoundException)
            {
            }
            return "Not Available";
        }
    }
}
