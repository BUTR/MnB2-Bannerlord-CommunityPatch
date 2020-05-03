using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Antijank {

  public sealed class MergedXmlNodeList : XmlNodeList, IEnumerable<XmlNode> {

    private readonly IEnumerable<XmlNode> _nodes;

    public MergedXmlNodeList(IEnumerable<XmlNode> nodes)
      => _nodes = nodes;

    public override XmlNode Item(int index)
      => _nodes.Skip(index).FirstOrDefault();

    IEnumerator<XmlNode> IEnumerable<XmlNode>.GetEnumerator()
      => _nodes.GetEnumerator();

    public override IEnumerator GetEnumerator()
      => _nodes.GetEnumerator();

    public override int Count
      => _nodes.Count();

  }

  public sealed class MergedXmlDocumentElement : XmlElement {

    private MergedXmlDocument _merged;

    public IEnumerable<XmlElement> SourcesLastToFirst => _merged.SourcesLastToFirst.Select(doc => doc.DocumentElement);

    public IEnumerable<XmlElement> SourcesFirstToLast => _merged.SourcesFirstToLast.Select(doc => doc.DocumentElement);

    public MergedXmlDocumentElement(string prefix, string localName, string namespaceUri, MergedXmlDocument doc) : base(prefix, localName, namespaceUri, doc)
      => _merged = doc;

    public override XPathNavigator CreateNavigator()
      => throw new NotImplementedException();

    public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
      => throw new NotImplementedException();

    public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
      => throw new NotImplementedException();

    public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
      => throw new NotImplementedException();

    public override XmlNode RemoveChild(XmlNode oldChild)
      => throw new NotImplementedException();

    public override XmlNode PrependChild(XmlNode newChild)
      => throw new NotImplementedException();

    public override XmlNode AppendChild(XmlNode newChild)
      => throw new NotImplementedException();

    public override XmlNode CloneNode(bool deep)
      => throw new NotImplementedException();

    public override void Normalize()
      => _merged.Normalize();

    public override bool Supports(string feature, string version)
      => _merged.Supports(feature, version);

    public override XmlNode Clone()
      => throw new NotImplementedException();

    public override string GetAttribute(string name)
      => SourcesLastToFirst.FirstOrDefault(x => x.HasAttribute(name))?.GetAttribute(name) ?? "";

    public override void SetAttribute(string name, string value)
      => throw new NotImplementedException();

    public override void RemoveAttribute(string name)
      => throw new NotImplementedException();

    public override XmlAttribute GetAttributeNode(string name)
      => SourcesLastToFirst.FirstOrDefault(x => x.HasAttribute(name))?.GetAttributeNode(name);

    public override XmlAttribute SetAttributeNode(XmlAttribute newAttr)
      => throw new NotImplementedException();

    public override XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
      => throw new NotImplementedException();

    public override XmlNodeList GetElementsByTagName(string name)
      => new MergedXmlNodeList(SourcesFirstToLast.SelectMany(x => x.GetElementsByTagName(name).Cast<XmlNode>()));

    public override string GetAttribute(string localName, string namespaceUri)
      => SourcesLastToFirst.FirstOrDefault(x => x.HasAttribute(localName, namespaceUri))?.GetAttribute(localName, namespaceUri) ?? "";

    public override string SetAttribute(string localName, string namespaceUri, string value)
      => throw new NotImplementedException();

    public override void RemoveAttribute(string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlAttribute GetAttributeNode(string localName, string namespaceUri)
      => SourcesLastToFirst.FirstOrDefault(x => x.HasAttribute(localName, namespaceUri))?.GetAttributeNode(localName, namespaceUri);

    public override XmlAttribute SetAttributeNode(string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlAttribute RemoveAttributeNode(string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlNodeList GetElementsByTagName(string localName, string namespaceUri)
      => new MergedXmlNodeList(SourcesFirstToLast.SelectMany(x => x.GetElementsByTagName(localName, namespaceUri).Cast<XmlNode>()));

    public override bool HasAttribute(string name)
      => SourcesLastToFirst.Any(x => x.HasAttribute(name));

    public override bool HasAttribute(string localName, string namespaceUri)
      => SourcesLastToFirst.Any(x => x.HasAttribute(localName, namespaceUri));

    public override void WriteTo(XmlWriter w)
      => throw new NotImplementedException();

    public override void WriteContentTo(XmlWriter w)
      => throw new NotImplementedException();

    public override void RemoveAll()
      => throw new NotImplementedException();

    public override string GetNamespaceOfPrefix(string prefix)
      => SourcesLastToFirst.Select(x => x.GetNamespaceOfPrefix(prefix)).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

    public override string GetPrefixOfNamespace(string namespaceUri)
      => SourcesLastToFirst.Select(x => x.GetPrefixOfNamespace(namespaceUri)).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

    public override string Name
      => SourcesLastToFirst.FirstOrDefault()?.Name ?? "";

    public override string Value {
      get => SourcesLastToFirst.FirstOrDefault()?.Value ?? "";
      set => throw new NotImplementedException();
    }

    public override string LocalName
      => SourcesLastToFirst.FirstOrDefault()?.LocalName ?? "";

    public override bool IsReadOnly
      => true;

    public override bool HasChildNodes
      => SourcesLastToFirst.Any(x => x.HasChildNodes);

    public override string NamespaceURI
      => SourcesLastToFirst.FirstOrDefault()?.NamespaceURI ?? "";

    public override string Prefix {
      get => SourcesLastToFirst.FirstOrDefault()?.Prefix ?? "";
      set => throw new NotImplementedException();
    }

    public override XmlNodeType NodeType
      => XmlNodeType.Element;

    public override XmlNode ParentNode
      => _merged;

    public override XmlNodeList ChildNodes
      => new MergedXmlNodeList(SourcesFirstToLast.SelectMany(x => x.ChildNodes.Cast<XmlNode>()));

    public override XmlNode PreviousSibling
      => null;

    public override XmlNode NextSibling
      => null;

    public override XmlAttributeCollection Attributes
      => (XmlAttributeCollection) Activator.CreateInstance(typeof(XmlAttributeCollection), BindingFlags.Public | BindingFlags.NonPublic, null, new object[] {this}, null);

    public override bool HasAttributes
      => SourcesLastToFirst.Any(x => x.HasAttributes);

    public override IXmlSchemaInfo SchemaInfo
      => throw new NotImplementedException();

    public override string BaseURI
      => SourcesLastToFirst.FirstOrDefault()?.BaseURI ?? "";

    public override XmlElement this[string name]
      => SourcesFirstToLast.Select(x => x[name]).FirstOrDefault(x => x != null);

    public override XmlElement this[string localName, string ns]
      => SourcesFirstToLast.Select(x => x[localName, ns]).FirstOrDefault(x => x != null);

    public override XmlNode PreviousText
      => null;

    public override string OuterXml
      => throw new NotImplementedException();

    public override string InnerXml {
      get => string.Concat(SourcesFirstToLast.Select(x => x.InnerXml));
      set => base.InnerXml = value;
    }

    public override string InnerText {
      get => string.Concat(SourcesFirstToLast.Select(x => x.InnerText));
      set => base.InnerText = value;
    }

    public override XmlDocument OwnerDocument
      => _merged;

    public override XmlNode FirstChild
      => SourcesFirstToLast.FirstOrDefault(x => x.HasChildNodes)?.FirstChild;

    public override XmlNode LastChild
      => SourcesFirstToLast.FirstOrDefault(x => x.HasChildNodes)?.LastChild;

    public override XmlNode RemoveAttributeAt(int i)
      => throw new NotImplementedException();

    public override void RemoveAllAttributes()
      => throw new NotImplementedException();

    public override string ToString()
      => "[Merged Document Element]";

    public override bool Equals(object obj)
      => ReferenceEquals(this, obj);

    public override int GetHashCode() {
      var hc = 0;

      static uint RotateLeft(uint x, int y)
        => (x << y) | (x >> (32 - y));

      foreach (var doc in SourcesFirstToLast)
        hc = (int) RotateLeft((uint) hc, 1) ^ doc.GetHashCode();
      return hc;
    }

  }

}