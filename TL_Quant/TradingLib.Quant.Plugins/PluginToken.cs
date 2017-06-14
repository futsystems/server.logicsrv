using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using TradingLib.Quant;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    public sealed class PluginToken : IXmlSerializable
    {
        // Methods
        public PluginToken Clone()
        {
            PluginToken token = (PluginToken)base.MemberwiseClone();
            token.ConstructorArguments = new List<ConstructorArgument>();
            if (this.ConstructorArguments != null)
            {
                foreach (ConstructorArgument argument in this.ConstructorArguments)
                {
                    token.ConstructorArguments.Add(argument.Clone());
                }
            }
            return token;
        }

        public static PluginToken Create(Type pluginType, Type interfaceType)
        {
            PluginToken token = new PluginToken
            {
                AssemblyFilename = Path.GetFileName(pluginType.Assembly.Location),
                TypeFullName = pluginType.FullName,
                InterfaceImplemented = interfaceType
            };
            if (Attribute.GetCustomAttributes(pluginType, typeof(ExcludedPluginAttribute), false).Length > 0)
            {
                return null;
            }
            token.Name = pluginType.Name;
            foreach (DisplayNameAttribute attribute in Attribute.GetCustomAttributes(pluginType, typeof(DisplayNameAttribute), false))
            {
                token.Name = attribute.DisplayName;
            }
            token.Description = token.Name;
            foreach (DescriptionAttribute attribute2 in Attribute.GetCustomAttributes(pluginType, typeof(DescriptionAttribute), false))
            {
                token.Description = attribute2.Description;
            }
            if (pluginType.GetConstructor(Type.EmptyTypes) == null)
            {
                token.ConstructorArguments = PluginFinder.GetArgumentList(pluginType);
            }
            return token;
        }

        public override bool Equals(object obj)
        {
            PluginToken other = obj as PluginToken;
            if (other == null)
            {
                return false;
            }
            return this.IsSamePlugin(other);
        }

        public override int GetHashCode()
        {
            return (((this.AssemblyFilename ?? string.Empty).GetHashCode() ^ (this.TypeFullName ?? string.Empty).GetHashCode()) ^ (this.InterfaceImplemented ?? typeof(object)).GetHashCode());
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public bool IsSamePlugin(PluginToken other)
        {
            if (other == null)
            {
                return false;
            }
            return (((this.AssemblyFilename == other.AssemblyFilename) && (this.TypeFullName == other.TypeFullName)) && (this.InterfaceImplemented == other.InterfaceImplemented));
        }

        public void ReadXml(XmlReader reader)
        {
            bool isEmptyElement = reader.IsEmptyElement;
            reader.Read();
            if (!isEmptyElement)
            {
            Label_0176:
                if ((reader.NodeType != XmlNodeType.EndElement) && (reader.NodeType != XmlNodeType.None))
                {
                    while (((reader.NodeType != XmlNodeType.EndElement) && (reader.NodeType != XmlNodeType.None)) && (reader.NodeType != XmlNodeType.Element))
                    {
                        reader.Read();
                    }
                    switch (reader.LocalName)
                    {
                        case "AssemblyFilename":
                            this.AssemblyFilename = reader.ReadElementContentAsString();
                            this.AssemblyFilename = Path.GetFileName(this.AssemblyFilename);
                            goto Label_0176;

                        case "TypeFullName":
                            this.TypeFullName = reader.ReadElementContentAsString();
                            goto Label_0176;

                        case "InterfaceImplemented":
                            {
                                string name = reader.ReadElementContentAsString();
                                this.InterfaceImplemented = typeof(IQService).Assembly.GetType(name, true);
                                goto Label_0176;
                            }
                        case "Name":
                            this.Name = reader.ReadElementContentAsString();
                            goto Label_0176;

                        case "Description":
                            this.Description = reader.ReadElementContentAsString();
                            goto Label_0176;

                        case "ConstructorArguments":
                            {
                                reader.ReadStartElement();
                                XmlSerializer serializer = new XmlSerializer(typeof(List<ConstructorArgument>));
                                this.ConstructorArguments = (List<ConstructorArgument>)serializer.Deserialize(reader);
                                reader.ReadEndElement();
                                goto Label_0176;
                            }
                    }
                    goto Label_0176;
                }
                if (reader.NodeType != XmlNodeType.None)
                {
                    reader.ReadEndElement();
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("AssemblyFilename", this.AssemblyFilename);
            writer.WriteElementString("TypeFullName", this.TypeFullName);
            writer.WriteElementString("InterfaceImplemented", this.InterfaceImplemented.FullName);
            writer.WriteElementString("Name", this.Name);
            writer.WriteElementString("Description", this.Description);
            writer.WriteStartElement("ConstructorArguments");
            new XmlSerializer(typeof(List<ConstructorArgument>)).Serialize(writer, this.ConstructorArguments);
            writer.WriteEndElement();
        }

        // Properties
        public string AssemblyFilename { get; set; }

        public List<ConstructorArgument> ConstructorArguments { get; set; }

        public string Description { get; set; }

        public Type InterfaceImplemented { get; set; }

        public string Name { get; set; }

        public bool RequiresConstructorArguments
        {
            get
            {
                return ((this.ConstructorArguments != null) && (this.ConstructorArguments.Count != 0));
            }
        }

        public string TypeFullName { get; set; }
    }

 

}
