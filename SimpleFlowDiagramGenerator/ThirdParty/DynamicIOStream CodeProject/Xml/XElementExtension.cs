using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LearningMahesh.DynamicIOStream.Xml
{
	public static class XElementExtension
	{
		public static DynamicXmlStream ToDynamicXmlStream(this XElement xElement)
		{
			DynamicXmlStream dynamicXmlStream = new DynamicXmlStream(xElement);
			return dynamicXmlStream;
		}
	}
}
