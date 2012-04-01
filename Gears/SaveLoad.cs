using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Gears
{

    public class SaveLoad
    {
        public static void saveControl(IControlSave cs, Control c)
        {
            if (c is TextBox)
            {
                cs.SaveTextBox(c.Name, c.Text);
            }
            else if (c is ComboBox)
            {
                cs.SaveComboBox(c.Name, c.Text);
            }
            else if (c is CheckBox)
            {
                cs.SaveCheckBox(c.Name, (c as CheckBox).Checked);
            }
        }

        public static void loadControl(IControlLoad xl, Control c)
        {
            string sVal = "";
            bool bVal = false;
            if (c is TextBox && xl.LoadTextBox(c.Name, ref sVal))
            {
                c.Text = sVal;
            }
            else if (c is ComboBox && xl.LoadComboBox(c.Name, ref sVal))
            {
                c.Text = sVal;
            }
            else if (c is CheckBox && xl.LoadCheckBox(c.Name, ref bVal))
                {
                (c as CheckBox).Checked = bVal;
            }
        }

    }

    public interface IControlSave
    {
        void SaveTextBox(string name, string value);
        void SaveComboBox(string name, string value);
        void SaveCheckBox(string name, bool value);
    }

    public interface IControlLoad
    {
        bool LoadTextBox(string name, ref string value);
        bool LoadComboBox(string name, ref string value);
        bool LoadCheckBox(string name, ref bool value);
    }

    public class RegistrySave : IControlSave
    {
        public RegistrySave(string prefix)
        {
            p = prefix;
        }
        string p;

        public void SaveTextBox(string name, string value)
        {
            Microsoft.Win32.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                value);
        }

        public void SaveComboBox(string name, string value)
        {
            Microsoft.Win32.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                value);
        }

        public void SaveCheckBox(string name, bool value)
        {
            Microsoft.Win32.Registry.SetValue(Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                value);
        }
    }


    public class RegistryLoad : IControlLoad
    {
        public RegistryLoad(string prefix)
        {
            p = prefix;
        }
        string p;

        public bool LoadTextBox(string name, ref string value)
        {
            string s = Microsoft.Win32.Registry.GetValue(
                Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                null) as string;
            if (s != null)
            {
                value = s;
                return true;
            }
            return false;
        }

        public bool LoadComboBox(string name, ref string value)
        {
            string s = Microsoft.Win32.Registry.GetValue(
                Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                null) as string;
            if (s != null)
            {
                value = s;
                return true;
            }
            return false;
        }

        public bool LoadCheckBox(string name, ref bool value)
        {
            object o = Microsoft.Win32.Registry.GetValue(
                Microsoft.Win32.Registry.CurrentUser.Name + p,
                name,
                null);
            if (o != null && o is bool)
            {
                value = (bool)o;
                return true;
            }
            return false;
        }
    }

    public class XmlSave : IControlSave
    {
        public XmlSave(StreamWriter writer)
        {
            sw = writer;
        }
        StreamWriter sw;

        public void SaveTextBox(string name, string value)
        {
            sw.WriteLine("<setting name='{0}'>{1}</setting>", name, value);
        }

        public void SaveComboBox(string name, string value)
        {
            sw.WriteLine("<setting name='{0}'>{1}</setting>", name, value);
        }

        public void SaveCheckBox(string name, bool value)
        {
            sw.WriteLine("<setting name='{0}'>{1}</setting>", name, value);
        }
    }

    public class XmlLoad : IControlLoad
    {
        public XmlLoad(XmlDocument doc)
        {
            xd = doc;
        }
        XmlDocument xd;

        public bool LoadTextBox(string name, ref string value)
        {
            XmlNode xe = xd.SelectSingleNode("/settings/setting[@name='" + name + "']");
            if (xe is XmlElement)
            {
                value = (xe as XmlElement).InnerText;
                return true;
            }
            return false;
        }

        public bool LoadComboBox(string name, ref string value)
        {
            XmlNode xe = xd.SelectSingleNode("/settings/setting[@name='" + name + "']");
            if (xe is XmlElement)
            {
                value = (xe as XmlElement).InnerText;
                return true;
            }
            return false;
        }

        public bool LoadCheckBox(string name, ref bool value)
        {
            XmlNode xe = xd.SelectSingleNode("/settings/setting[@name='" + name + "']");
            if (xe is XmlElement)
            {
                value = Boolean.Parse((xe as XmlElement).InnerText);
                return true;
            }
            return false;
        }
    }

}
