using System;
using System.Collections;

namespace Cassowary.Utils
{
	/// <summary>
	/// A simple set class for use in ClTableau and ClSimplexSolver.
	/// </summary>
	/// <remarks>
	/// Uses a Hashtable internally for storage.
	/// </remarks>
	public class Set : ICloneable
	{
		public Set()
		{
			_hash = new Hashtable();
		}

		public Set(int i)
		{
			_hash = new Hashtable(i);
		}

		public Set(int i, float f)
		{
			_hash = new Hashtable(i, f);
		}

		public Set(Hashtable h)
		{
			_hash = h;
		}

		public bool ContainsKey(object o)
		{
			return _hash.ContainsKey(o);
		}

		public void Add(object o)
		{
			_hash.Add(o, o);
		}

		public void Remove(object o)
		{
			_hash.Remove(o);
		}

		public void Clear()
		{
			_hash.Clear();
		}

		public object Clone()
		{
			return new Set((Hashtable)_hash.Clone());
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return _hash.GetEnumerator();
		}

		public override string ToString()
		{
			// start with left brace
			string result = "{";
			string separator = ", ";
			
			// add each element

			foreach (object o in _hash)
			{
				result += o;
				result += separator;
			}

			// remove last separator
			result = result.Substring(0, result.Length - separator.Length);

			// add right brace
			result += "}";

			return result;
		}

		/// <summary>
		/// For casting between Set and Hashtable.
		/// </summary>
		public static explicit operator Set (Hashtable h)
		{
			Set res = new Set(h);

			return res;
		}

		public int Count
		{
			get { return _hash.Count; }
		}

		public bool Empty
		{
			get { return _hash.Count == 0; }
		}

		private Hashtable _hash;
	}
}
