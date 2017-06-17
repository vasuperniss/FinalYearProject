using System.Collections.Generic;
using System.Xml;

namespace DigitalIsraelFund_System.Models
{
    public class FormComponent
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public List<FormComponent> FormComponents { get; set; }

        public FormComponent(XmlReader xmlReader)
        {
            this.Type = xmlReader.Name;
            this.Properties = new Dictionary<string, string>();
            this.FormComponents = new List<FormComponent>();

            if (xmlReader.HasAttributes)
                while (xmlReader.MoveToNextAttribute())
                    this.Properties.Add(xmlReader.Name, xmlReader.Value);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                    this.FormComponents.Add(new FormComponent(xmlReader));
                else if (xmlReader.NodeType == XmlNodeType.Text)
                    this.Text = xmlReader.Value;
                else if (xmlReader.NodeType == XmlNodeType.EndElement
                    && xmlReader.Name == this.Type)
                    break;
            }
        }
    }
}