using System.Collections.Generic;
using System.Xml;

namespace DigitalIsraelFund_System.Models
{
    public class FormUnit
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<FormUnit> FormUnits { get; set; }

        public FormUnit(XmlReader xmlReader)
        {
            this.Type = xmlReader.Name;
            this.Properties = new Dictionary<string, string>();
            this.FormUnits = new List<FormUnit>();

            if (xmlReader.HasAttributes)
                while (xmlReader.MoveToNextAttribute())
                    this.Properties.Add(xmlReader.Name, xmlReader.Value);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                    this.FormUnits.Add(new FormUnit(xmlReader));
                else if (xmlReader.NodeType == XmlNodeType.Text)
                    this.Text = xmlReader.Value;
                else if (xmlReader.NodeType == XmlNodeType.EndElement
                    && xmlReader.Name == this.Type)
                    break;
            }
        }
    }
}