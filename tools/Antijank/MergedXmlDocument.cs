using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Antijank {

  public sealed class MergedXmlDocument : XmlDocument {

    private readonly LinkedList<XmlDocument> _sources;

    public IEnumerable<XmlDocument> SourcesLastToFirst {
      get {
        var node = _sources.Last;
        while (node != null) {
          yield return node.Value;

          node = node.Previous;
        }
      }
    }

    public IEnumerable<XmlDocument> SourcesFirstToLast {
      get {
        var node = _sources.First;
        while (node != null) {
          yield return node.Value;

          node = node.Next;
        }
      }
    }

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

    public override XmlNode Clone() {
      var doc = new XmlDocument();
      var src = _sources.First;
      doc.AppendChild(doc.ImportNode(src.Value.DocumentElement!, true));
      var docElem = doc.DocumentElement;
      for (;;) {
        src = src.Next;
        if (src == null) break;

        foreach (XmlNode child in src.Value.DocumentElement!.ChildNodes)
          docElem!.AppendChild(doc.ImportNode(child, true));
      }

      return doc;
    }

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

    public override XmlNodeList GetElementsByTagName(string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlElement GetElementById(string elementId)
      => SourcesFirstToLast
        .Select(x => x.GetElementById(elementId))
        .FirstOrDefault(x => x != null);

    public override XmlNode ImportNode(XmlNode node, bool deep)
      => throw new NotImplementedException();

    public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceUri)
      => throw new NotImplementedException();

    protected override XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(string nodeTypeString, string name, string namespaceUri)
      => throw new NotImplementedException();

    public override XmlNode CreateNode(XmlNodeType type, string name, string namespaceUri)
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
      => SourcesLastToFirst.Select(x => x.GetNamespaceOfPrefix(prefix)).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

    public override string GetPrefixOfNamespace(string namespaceUri)
      => SourcesLastToFirst.Select(x => x.GetPrefixOfNamespace(namespaceUri)).FirstOrDefault(x => !string.IsNullOrEmpty(x)) ?? "";

    public override string Name
      => SourcesLastToFirst.FirstOrDefault()?.Name ?? "";

    public override string Value {
      get => SourcesLastToFirst.FirstOrDefault()?.Value ?? "";
      set => throw new NotImplementedException();
    }

    public override bool HasChildNodes
      => SourcesLastToFirst.Any(x => x.HasChildNodes);

    public override string NamespaceURI
      => SourcesLastToFirst.FirstOrDefault()?.NamespaceURI ?? "";

    public override string Prefix {
      get => SourcesLastToFirst.FirstOrDefault()?.Prefix ?? "";
      set => throw new NotImplementedException();
    }

    public override string LocalName
      => SourcesLastToFirst.FirstOrDefault()?.LocalName ?? "";

    public override bool IsReadOnly
      => true;

    public override string InnerText {
      get => string.Concat(SourcesFirstToLast.Select(x => x.InnerText));
      set => throw new NotImplementedException();
    }

    public override string OuterXml
      => throw new NotImplementedException();

    public override string InnerXml {
      get => string.Concat(SourcesFirstToLast.Select(x => x.InnerXml));
      set => throw new NotImplementedException();
    }

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

    public override XmlAttributeCollection Attributes
      => throw new NotImplementedException();

    public override XmlDocument OwnerDocument
      => this;

    public override XmlNode FirstChild
      => DocumentElement;

    public override XmlNode LastChild
      => DocumentElement;

    public override XmlResolver XmlResolver {
      set => throw new NotImplementedException();
    }

    public override XmlNodeType NodeType
      => throw new NotImplementedException();

    public override XmlNode ParentNode
      => null;

    public override XmlNodeList ChildNodes
      => new MergedXmlNodeList(new[] {DocumentElement});

    public override XmlNode PreviousSibling
      => null;

    public override XmlNode NextSibling
      => null;

    public override XmlDocumentType DocumentType
      => SourcesFirstToLast
        .FirstOrDefault(x => x.DocumentType != null)
        ?.DocumentType;

    public override string ToString()
      => throw new NotImplementedException();

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