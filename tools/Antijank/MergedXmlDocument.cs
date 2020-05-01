using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Antijank {

  public sealed class MergedXmlDocument : XmlDocument {

    private LinkedList<XmlDocument> _sources;

    public MergedXmlDocument(params XmlDocument[] sources)
      : this((IEnumerable<XmlDocument>) sources) {
    }

    public MergedXmlDocument(IEnumerable<XmlDocument> sources)
      => _sources = sources as LinkedList<XmlDocument> ?? new LinkedList<XmlDocument>(sources);

    public override XmlNode CloneNode(bool deep)
      => throw new NotImplementedException();

    public override void Normalize() {
      foreach (var doc in _sources)
        doc.Normalize();
    }

    public override bool Supports(string feature, string version) {
      if (_sources.Count == 0)
        return base.Supports(feature, version);

      foreach (var doc in _sources)
        if (!doc.Supports(feature, version))
          return false;

      return true;
    }

    public override XmlNode Clone()
      => throw new NotImplementedException();

    public override XmlCDataSection CreateCDataSection(string data)
      => throw new NotImplementedException();

    public override XmlComment CreateComment(string data)
      => throw new NotImplementedException();

    public override XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
      => throw new NotImplementedException();

    public override XmlDocumentFragment CreateDocumentFragment()
      => throw new NotImplementedException();

    public override XmlEntityReference CreateEntityReference(string name)
      => throw new NotImplementedException();

    public override XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
      => throw new NotImplementedException();

    public override XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
      => throw new NotImplementedException();

    public override XmlText CreateTextNode(string text)
      => throw new NotImplementedException();

    public override XmlSignificantWhitespace CreateSignificantWhitespace(string text)
      => throw new NotImplementedException();

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

    protected override XPathNavigator CreateNavigator(XmlNode node)
      => throw new NotImplementedException();

    public override XmlWhitespace CreateWhitespace(string text)
      => throw new NotImplementedException();

    public override XmlNodeList GetElementsByTagName(string name)
      => throw new NotImplementedException();

    public override XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlElement GetElementById(string elementId)
      => throw new NotImplementedException();

    public override XmlNode ImportNode(XmlNode node, bool deep)
      => throw new NotImplementedException();

    public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
      => throw new NotImplementedException();

    protected override XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
      => throw new NotImplementedException();

    public override XmlNode ReadNode(XmlReader reader)
      => throw new NotImplementedException();

    public override void Load(string filename)
      => throw new NotImplementedException();

    public override void Load(Stream inStream)
      => throw new NotImplementedException();

    public override void Load(TextReader txtReader)
      => throw new NotImplementedException();

    public override void Load(XmlReader reader)
      => throw new NotImplementedException();

    public override void LoadXml(string xml)
      => throw new NotImplementedException();

    public override void Save(string filename)
      => throw new NotImplementedException();

    public override void Save(Stream outStream)
      => throw new NotImplementedException();

    public override void Save(TextWriter writer)
      => throw new NotImplementedException();

    public override void Save(XmlWriter w)
      => throw new NotImplementedException();

    public override void WriteTo(XmlWriter w)
      => throw new NotImplementedException();

    public override void WriteContentTo(XmlWriter xw)
      => throw new NotImplementedException();

    public override void RemoveAll()
      => throw new NotImplementedException();

    public override string GetNamespaceOfPrefix(string prefix)
      => throw new NotImplementedException();

    public override string GetPrefixOfNamespace(string namespaceURI)
      => throw new NotImplementedException();

    public override string Name
      => throw new NotImplementedException();

    public override string Value {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override bool HasChildNodes
      => throw new NotImplementedException();

    public override string NamespaceURI
      => throw new NotImplementedException();

    public override string Prefix {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override string LocalName
      => throw new NotImplementedException();

    public override bool IsReadOnly
      => throw new NotImplementedException();

    public override string InnerText {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override string OuterXml
      => throw new NotImplementedException();

    public override string InnerXml {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override IXmlSchemaInfo SchemaInfo
      => throw new NotImplementedException();

    public override string BaseURI
      => throw new NotImplementedException();

    public override XmlElement this[string name]
      => throw new NotImplementedException();

    public override XmlElement this[string localname, string ns]
      => throw new NotImplementedException();

    public override XmlNode PreviousText
      => throw new NotImplementedException();

    public override XmlAttributeCollection Attributes
      => throw new NotImplementedException();

    public override XmlDocument OwnerDocument
      => throw new NotImplementedException();

    public override XmlNode FirstChild
      => throw new NotImplementedException();

    public override XmlNode LastChild
      => throw new NotImplementedException();

    public override XmlResolver XmlResolver {
      set => throw new NotImplementedException();
    }

    public override XmlNodeType NodeType
      => throw new NotImplementedException();

    public override XmlNode ParentNode
      => throw new NotImplementedException();

    public override XmlNodeList ChildNodes
      => throw new NotImplementedException();

    public override XmlNode PreviousSibling
      => throw new NotImplementedException();

    public override XmlNode NextSibling
      => throw new NotImplementedException();

    public override XmlDocumentType DocumentType
      => throw new NotImplementedException();

    public override string ToString()
      => throw new NotImplementedException();

    public override bool Equals(object obj)
      => base.Equals(obj);

    public override int GetHashCode() {
      var hc = 0;

      static uint RotateLeft(uint x, int y)
        => (x << y) | (x >> (32 - y));

      foreach (var doc in _sources)
        hc = (int) RotateLeft((uint) hc, 1) ^ doc.GetHashCode();
      return hc;
    }

  }

}