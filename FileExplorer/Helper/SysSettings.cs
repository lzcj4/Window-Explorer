using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;

namespace FileExplorer.Helper
{
    public class SysSettings : IDisposable
    {
        private const string fileName = "SysSettings.xml";
        private const string tag_settings = "Settings";
        private const string tag_settings_quick = "quick";
        private const string tag_item = "item";

        private const string attr_id = "id";
        private const string attr_name = "name";
        private const string attr_value = "value";
        private const string attr_ctrlkeys = "ctrlkeys";
        private const string attr_key = "key";


        private static SysSettings instance;
        public static SysSettings GetInstance()
        {
            instance = instance ?? new SysSettings();
            return instance;
        }

        string filePath;
        XmlDocument xmlDoc;

        private SysSettings()
        {
            this.Load();
        }

        public string Quick_FilePath
        {
            get { return this.GetValue(tag_settings_quick, "File_Path", ""); }
            set { this.SetValue(tag_settings_quick, "File_Path", value); }
        }

        public Tuple<ModifierKeys, Keys> Quick_Save_Keys
        {
            get { return this.GetKeysValue(tag_settings_quick, "Save_Keys", new Tuple<ModifierKeys, Keys>(ModifierKeys.Control, Keys.S)); }
            set { this.SetKeysValue(tag_settings_quick, "Save_Keys", value.Item1, value.Item2); }
        }

        #region Get Methods

        private string GetValue(string tagName, string id, string defaultValue)
        {
            string result = defaultValue;
            XmlNode node = this.GetNode(tagName, id);
            if (node != null)
            {
                result = node.Attributes[attr_value].Value;
            }

            return result;
        }

        private Tuple<ModifierKeys, Keys> GetKeysValue(string tagName, string id, Tuple<ModifierKeys, Keys> defaultValue)
        {
            Tuple<ModifierKeys, Keys> result = defaultValue;
            XmlNode node = this.GetNode(tagName, id);
            if (node != null)
            {
                ModifierKeys ctrlKeys = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), node.Attributes[attr_ctrlkeys].Value);
                Keys key = (Keys)Enum.Parse(typeof(Keys), node.Attributes[attr_key].Value);
                result = new Tuple<ModifierKeys, Keys>(ctrlKeys, key);
            }
            return result;
        }

        private XmlNode GetNode(string tagName, string id)
        {
            XmlNode result = null;

            XmlNodeList elements = xmlDoc.GetElementsByTagName(tagName);
            foreach (XmlNode node in elements)
            {
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    if (subNode.Attributes[attr_id].Value == id)
                    {
                        result = subNode;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Set Methods

        private XmlNode GetRootNode()
        {
            XmlNode rootNode = xmlDoc.FirstChild;
            if (null != rootNode)
            {
                return rootNode;
            }
            rootNode = xmlDoc.CreateElement(tag_settings);
            xmlDoc.AppendChild(rootNode);
            return rootNode;
        }

        private XmlNode GetNode(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return null;
            }

            string[] items = tags.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            XmlNode parent = xmlDoc;
            XmlNode node = null;
            foreach (var item in items)
            {
                node = GetNode(parent, item);
                if (node != null)
                {
                    parent = node;
                }
                else
                {
                    break;
                }
            }
            return node;
        }

        private XmlNode GetNode(XmlNode parent, string tagName)
        {
            if (null == parent)
            {
                return null;
            }

            foreach (XmlNode item in parent.ChildNodes)
            {
                if (item.Name == tagName)
                {
                    return item;
                }
            }
            return null;
        }


        private XmlNode CreateNode(string tags)
        {
            string[] items = tags.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            XmlNode parent = xmlDoc;
            XmlNode node = null;
            foreach (var item in items)
            {
                node = GetNode(parent, item);
                if (node != null)
                {
                    parent = node;
                }
                else
                {
                    node = xmlDoc.CreateElement(item);
                    parent.AppendChild(node);
                    parent = node;
                }
            }
            return node;
        }
        
        /// <summary>
        /// Set string value
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        private void SetValue(string tagName, string id, string value)
        {
            XmlNode node = this.GetNode(tagName, id);
            if (node != null)
            {
                node.Attributes[attr_value].Value = value;
                return;
            }

            XmlNode element = CreateNode(string.Format("{0}/{1}", tag_settings, tagName));
            XmlNode subElement = xmlDoc.CreateElement(tag_item);
            element.AppendChild(subElement);
            XmlAttribute attr = xmlDoc.CreateAttribute(attr_id);
            attr.Value = id;
            subElement.Attributes.Append(attr);
            attr = xmlDoc.CreateAttribute(attr_value);
            attr.Value = value;
            subElement.Attributes.Append(attr);
        }

        private void SetKeysValue(string tagName, string id, ModifierKeys ctrlKeys, Keys key)
        {
            XmlNode node = this.GetNode(tagName, id);
            if (node != null)
            {
                node.Attributes[attr_ctrlkeys].Value = ((int)ctrlKeys).ToString();
                node.Attributes[attr_key].Value = ((int)key).ToString();
                return;
            }
            XmlNode element = CreateNode(string.Format("{0}/{1}", tag_settings, tagName));
            XmlElement subElement = xmlDoc.CreateElement(tag_item);
            element.AppendChild(subElement);
            XmlAttribute attr = xmlDoc.CreateAttribute(attr_id);
            attr.Value = id;
            subElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(attr_ctrlkeys);
            attr.Value = ((int)ctrlKeys).ToString();
            subElement.Attributes.Append(attr);

            attr = xmlDoc.CreateAttribute(attr_key);
            attr.Value = ((int)key).ToString();
            subElement.Attributes.Append(attr);
        }

        #endregion
        public void Load()
        {
            string currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            filePath = Path.Combine(currentFolder, fileName);
            xmlDoc = new XmlDocument();
            if (File.Exists(filePath))
            {
                xmlDoc.Load(filePath);
            }
        }

        public void Save()
        {
            if (xmlDoc != null)
            {
                xmlDoc.Save(filePath);
            }
        }
              
        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Save();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion   

    }
}
