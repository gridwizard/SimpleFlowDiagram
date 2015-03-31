using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LearningMahesh.DynamicIOStream.Xml
{
	public class DynamicXmlStream : 
		DynamicObject,
		IEnumerable<DynamicXmlStream>
	{
		#region Constants

		public const string Value = "Value";
		public const string Count = "Count";

		#endregion

		#region Fields/Properties

		private List<XElement> _XmlElementCollection;
		public List<XElement> XmlElementsCollection
		{
			get { return _XmlElementCollection; }
			set { _XmlElementCollection = value; }
		}

		#endregion

		#region Constructors

		public DynamicXmlStream()
		{
			_XmlElementCollection = new List<XElement>();
		}

		public DynamicXmlStream(string xmlString)
        {
            XDocument xDocument = XDocument.Parse(xmlString);
			XElement xElement = XmlNamespaceRemover.RemoveAllNamespaces(xDocument);
			_XmlElementCollection = new List<XElement> { xElement };
        }

		public DynamicXmlStream(DynamicXmlStream dynamicXmlStream)
		{
			this._XmlElementCollection = dynamicXmlStream._XmlElementCollection;
		}

        internal DynamicXmlStream(XElement xElement)
        {
            _XmlElementCollection = new List<XElement> { XmlNamespaceRemover.RemoveAllNamespaces(xElement) };
        }

		internal DynamicXmlStream(IEnumerable<XElement> xElementCollection)
        {
            _XmlElementCollection = new List<XElement>(xElementCollection);
        }

		#endregion

		#region Public Static Load/Parse/Create/Save Methods

		public static DynamicXmlStream Load(Stream fileStream, LoadOptions options = LoadOptions.None)
		{
			return new DynamicXmlStream(XElement.Load(fileStream, options));
		}

		public static DynamicXmlStream Load(string uri, LoadOptions options = LoadOptions.None)
		{
			return new DynamicXmlStream(XElement.Load(uri, options));
		}

		public static DynamicXmlStream Load(TextReader textReader, LoadOptions options = LoadOptions.None)
		{
			return new DynamicXmlStream(XElement.Load(textReader, options));
		}

		public static DynamicXmlStream Load(XmlReader xmlReader, LoadOptions options = LoadOptions.None)
		{
			return new DynamicXmlStream(XElement.Load(xmlReader, options));
		}

		public static DynamicXmlStream Parse(string xmlString, LoadOptions options = LoadOptions.None)
		{
			return new DynamicXmlStream(XElement.Parse(xmlString, options));
		}

		public static DynamicXmlStream Create(string rootElementName)
		{
			DynamicXmlStream dynamicXmlStream = new DynamicXmlStream();
			dynamicXmlStream.XmlElementsCollection.Add(new XElement(rootElementName));
			return dynamicXmlStream;
		}

		#endregion

		#region DynamicObject Overrides

		public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

			if (binder.Name == DynamicXmlStream.Value)
			{
				var items = _XmlElementCollection[0].Descendants(XName.Get("Value"));
				if (items == null || items.Count() == 0)
				{
					result = _XmlElementCollection[0].Value;
				}
				else
				{
					result = new DynamicXmlStream(items);
				}
			}
			else if (binder.Name == DynamicXmlStream.Count)
			{
				result = _XmlElementCollection.Count;
			}
			else
			{
				XAttribute xAttribute = _XmlElementCollection[0].Attribute(XName.Get(binder.Name));
				if (null != xAttribute)
				{
					result = xAttribute;
				}
				else
				{
					IEnumerable<XElement> xElementItems = _XmlElementCollection[0].DescendantsAndSelf(XName.Get(binder.Name));
					if (xElementItems == null || xElementItems.Count() == 0)
					{
						return false;
					}
					result = new DynamicXmlStream(xElementItems);
				}
			}

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
			if (binder.Name == DynamicXmlStream.Value)
			{
				_XmlElementCollection[0].Value = value.ToString();
			}
			else
			{
				XElement setNode = _XmlElementCollection[0].Element(binder.Name);
				if (setNode != null)
				{
					setNode.SetValue(value);
				}
				else
				{
					if (value.GetType() == typeof(DynamicXmlStream))
					{
						_XmlElementCollection[0].Add(new XElement(binder.Name));
					}
					else
					{
						_XmlElementCollection[0].Add(new XElement(binder.Name, value));
					}
				}
			}
			return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
			result = null;

			if (null != indexes[0])
			{
				if (typeof(int) == indexes[0].GetType())
				{
					int index = (int)indexes[0];
					result = new DynamicXmlStream(_XmlElementCollection[index]);
					return true;
				}
				else if (typeof(string) == indexes[0].GetType())
				{
					string attributeName = (string)indexes[0];
					result = _XmlElementCollection[0].Attribute(XName.Get(attributeName)).Value;
					return true;
				}
			}

			return false;
        }

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (null != indexes[0])
			{
				if (typeof(string) == indexes[0].GetType())
				{
					_XmlElementCollection[0].SetAttributeValue((string)indexes[0], value);
					return true;
				}
			}
			return false;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (binder.Type == typeof(XElement))
			{
				result = _XmlElementCollection[0];
			}
			else if (binder.Type == typeof(List<XElement>) || (binder.Type.IsArray && binder.Type.GetElementType() == typeof(XElement)))
			{
				result = _XmlElementCollection;
			}
			else if (binder.Type == typeof(String))
			{
				result = _XmlElementCollection[0].Value;
			}
			else
			{
				result = false;
				return false;
			}
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			Type xmlType = typeof(XElement);
			try
			{
				result = xmlType.InvokeMember(
					binder.Name, 
					BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
					null, _XmlElementCollection[0], args
				);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		#endregion

		#region IEnumerable<DynamicXmlReader> Members

		IEnumerator<DynamicXmlStream> IEnumerable<DynamicXmlStream>.GetEnumerator()
		{
			foreach (XElement xElement in _XmlElementCollection)
			{
				yield return new DynamicXmlStream(xElement);
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (XElement xElement in _XmlElementCollection)
			{
				yield return new DynamicXmlStream(xElement);
			}
		}

		#endregion

		#region Public Methods

		public void Save(string fileName)
		{
			FileStream fStream = new FileStream(fileName, FileMode.Create);
			_XmlElementCollection[0].Save(fStream);
		}

		public IEnumerable<dynamic> AsDynamicEnumerable()
		{
			return (this as IEnumerable<DynamicXmlStream>).Cast<dynamic>();
		}

		#endregion

		public override string ToString()
		{
			return (_XmlElementCollection != null && _XmlElementCollection.Count > 0)
				? _XmlElementCollection[0].ToString()
				: string.Empty;
		}
	}
}
