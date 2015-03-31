using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LearningMahesh.DynamicIOStream.Csv
{
	public class DynamicCsvStream :
		IEnumerable<DynamicCsvRow>
	{
		#region Constant

		public const string Comment = "#";
		public const char CommaDelimiter = ',';

		#endregion

		#region Fields/Properties

		private List<string> _HeaderColumns;
		public List<string> HeaderColumns
		{
			get { return _HeaderColumns; }
		}

		private List<DynamicCsvRow> _RowCollection;

		#endregion

		#region Constructors

		public DynamicCsvStream()
		{
			_HeaderColumns = new List<string>();
			_RowCollection = new List<DynamicCsvRow>();
		}

		public DynamicCsvStream(DynamicCsvStream dynamicCsvStream)
		{
			this._HeaderColumns = dynamicCsvStream._HeaderColumns;
			this._RowCollection = dynamicCsvStream._RowCollection;
		}

		#endregion

		#region Public Static Load/Parse/Create Methods

		public static DynamicCsvStream Load(StreamReader fileStream)
		{
			DynamicCsvStream dynamicCsvStream = new DynamicCsvStream();
			string currentRow = string.Empty;
			bool isHeaderRow = default(bool);

			while (!string.IsNullOrEmpty(currentRow = fileStream.ReadLine()))
			{
				if (currentRow.StartsWith(DynamicCsvStream.Comment))
				{
					continue;
				}
				else if (!isHeaderRow)
				{
					dynamicCsvStream.HeaderColumns.AddRange(currentRow.Split(DynamicCsvStream.CommaDelimiter));
					isHeaderRow = true;
				}
				else
				{
					dynamicCsvStream._RowCollection.Add(new DynamicCsvRow(currentRow, dynamicCsvStream._HeaderColumns));
				}
			}
			return dynamicCsvStream;
		}

		public static DynamicCsvStream Parse(string fileName)
		{
			using (StreamReader fileStream = new StreamReader(fileName))
			{
				return DynamicCsvStream.Load(fileStream);
			}
		}

		#endregion

		#region IEnumerable<DynamicCsvRow> Members

		IEnumerator<DynamicCsvRow> IEnumerable<DynamicCsvRow>.GetEnumerator()
		{
			foreach (DynamicCsvRow dynamicCsvRow in _RowCollection.Where(row => row.IsEmptyRow()))
			{
				yield return dynamicCsvRow;
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (DynamicCsvRow dynamicCsvRow in _RowCollection.Where(row => row.IsEmptyRow()))
			{
				yield return dynamicCsvRow;
			}
		}

		#endregion

		#region Public Methods

		public void Save(string fileName)
		{
			using (StreamWriter sWriter = new StreamWriter(new FileStream(fileName, FileMode.Create)))
			{
				sWriter.WriteLine(string.Join(",", _HeaderColumns));
				foreach (dynamic dynamicCsvRow in _RowCollection.Where(row => row.IsEmptyRow()))
				{
					sWriter.WriteLine((string)dynamicCsvRow);
				}
			}
		}

		public IEnumerable<dynamic> AsDynamicEnumerable()
		{
			return (this as IEnumerable<DynamicCsvRow>).Cast<dynamic>();
		}

		#endregion
	}

	internal class DynamicCsvRow :
		DynamicObject,
		IEnumerable<string>
	{
		#region Properties/Fields

		private List<string> _HeaderColumns;
		private List<string> _RowColumns;

		#endregion

		#region Constructor

		public DynamicCsvRow()
		{
			_HeaderColumns = new List<string>();
			_RowColumns = new List<string>();
		}

		public DynamicCsvRow(string oneRowValue, List<string> headerColumns)
		{
			this._HeaderColumns = headerColumns;
			this._RowColumns = oneRowValue.Split(DynamicCsvStream.CommaDelimiter).ToList();
		}

		#endregion

		#region DynamicObject Overrides

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			int indexOfColumn = _HeaderColumns.IndexOf(binder.Name);

			result = null;
			if (-1 != indexOfColumn)
			{
				result = (indexOfColumn < _RowColumns.Count) 
					? _RowColumns[indexOfColumn]
					: null;
			}

			return (indexOfColumn >= 0);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			int indexOfColumn = _HeaderColumns.IndexOf(binder.Name);

			if (-1 != indexOfColumn)
			{
				_RowColumns[indexOfColumn] = value.ToString();
			}

			return (indexOfColumn >= 0);
		}

		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			result = null;

			if (null != indexes[0])
			{
				if (typeof(int) == indexes[0].GetType())
				{
					int index = (int)indexes[0];
					result = (index <= _RowColumns.Count)
						? _RowColumns[index]
						: null;
					return true;
				}
				else if (typeof(string) == indexes[0].GetType())
				{
					string attribute = (string)indexes[0];
					int index = _HeaderColumns.IndexOf(attribute);
					result = (-1 != index && index < _RowColumns.Count)
						? _RowColumns[index]
						: null;
					return true;
				}
			}
			
			return false;
		}

		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (null != indexes[0])
			{
				if (typeof(int) == indexes[0].GetType())
				{
					int indexToChange = (int)indexes[0];
					if (indexToChange <= _RowColumns.Count)
					{
						_RowColumns[indexToChange] = value.ToString();
					}
					return true;
				}
				else if (typeof(string) == indexes[0].GetType())
				{
					string attribute = (string)indexes[0];
					int indexToChange = _HeaderColumns.IndexOf(attribute);
					if (indexToChange <= _RowColumns.Count)
					{
						_RowColumns[indexToChange] = value.ToString();
					}
					return true;
				}
			}
			return false;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			result = null;
			if (binder.Type == typeof(List<string>))
			{
				result = _RowColumns;
			}
			else if (binder.Type == typeof(string))
			{
				result = string.Join(",", _RowColumns);
			}
			else
			{
				return false;
			}
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			Type stringListType = typeof(List<string>);
			try
			{
				result = stringListType.InvokeMember(
					binder.Name,
					BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
					null, _RowColumns, args
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

		#region IEnumerable<DynamicCsvCell>

		public IEnumerator<string> GetEnumerator()
		{
			foreach (string cellValue in _RowColumns)
			{
				yield return cellValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (string cellValue in _RowColumns)
			{
				yield return cellValue;
			}
		}

		#endregion

		#region Public Methods

		public bool IsEmptyRow()
		{
			return (null != _RowColumns || 0 == _RowColumns.Count);
		}

		public IEnumerable<dynamic> AsDynamicEnumerable()
		{
			return (this as IEnumerable<string>).Cast<dynamic>();
		}

		#endregion

		public override string ToString()
		{
			return (_RowColumns != null && _RowColumns.Count > 0)
				? string.Join(",", _RowColumns)
				: string.Empty;
		}
	}
}
