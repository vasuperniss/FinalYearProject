using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class FormManager
    {
        private static FormManager instance = new FormManager();

        public static FormManager Manager { get { return instance; } }

        private readonly object syncLock = new object();
        private Queue<string> formPool;
        private Dictionary<string, FormComponent> forms;
        private int MAX_SIZE = 10;

        private FormManager()
        {
            this.formPool = new Queue<string>();
            this.forms = new Dictionary<string, FormComponent>();
        }

        public FormComponent Load(string filepath)
        {
            lock (this.syncLock) {
                if (!forms.ContainsKey(filepath))
                {
                    this.forms[filepath] = LoadNotPooled(filepath);
                    this.formPool.Enqueue(filepath);
                    if (forms.Count > MAX_SIZE)
                    {
                        var keyToRemove = this.formPool.Dequeue();
                        this.forms.Remove(keyToRemove);
                    }
                }
            }
            return forms[filepath];
        }

        private FormComponent LoadNotPooled(string filepath)
        {
            string xmlNode = File.ReadAllText(filepath);
            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlNode));
            FormComponent loadedForm = new FormComponent(xmlReader);
            xmlReader.Close();
            return loadedForm;
        }
    }
}