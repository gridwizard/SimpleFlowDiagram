using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace LearningMahesh.DynamicIOStream.Xml
{
	public class XmlNamespaceRemover
    {
        public XmlNamespaceRemover() { }
 
        public static XElement RemoveAllNamespaces(XDocument xDocumentSource)
        {
			Stream docStream = new MemoryStream();
            xDocumentSource.Save(docStream);
            docStream.Position = 0;

            Stream outputStream = new MemoryStream();
            outputStream.Position = 0;

            XPathDocument xPathDocument = new XPathDocument(docStream);
            XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();
 
            XslTransform myXslTransform;
            myXslTransform = new XslTransform();
            
            XmlReader xsltReader = 
				XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("LearningMahesh.DynamicIOStream.Xml.StripNamespace.xslt"));
            myXslTransform.Load(xsltReader);
 
            XsltArgumentList xsltArgList = new XsltArgumentList();
            myXslTransform.Transform(xPathNavigator, xsltArgList, outputStream);
 
            outputStream.Position = 0;
			XDocument finalDocument = XDocument.Load(outputStream);

			XElement root = finalDocument.Root;
            return root;
        }

		public static XElement RemoveAllNamespaces(XElement xElementSource)
		{
			Stream docStream = new MemoryStream();
			xElementSource.Save(docStream);
			docStream.Position = 0;

			Stream outputStream = new MemoryStream();
			outputStream.Position = 0;

			XPathDocument xPathDocument = new XPathDocument(docStream);
			XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();

			XslTransform myXslTransform;
			myXslTransform = new XslTransform();

			XmlReader xsltReader =
				XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("LearningMahesh.DynamicIOStream.Xml.StripNamespace.xslt"));
			myXslTransform.Load(xsltReader);

			XsltArgumentList xsltArgList = new XsltArgumentList();
			myXslTransform.Transform(xPathNavigator, xsltArgList, outputStream);

			outputStream.Position = 0;
			XDocument finalDocument = XDocument.Load(outputStream);

			XElement root = finalDocument.Root;
			return root;
		}
    }
}
