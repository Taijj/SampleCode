
using UnityEngine;

namespace Taijj.SampleCode
{
    public class SpanAttribute : PropertyAttribute
    {
    	public SpanAttribute(string minName, string maxName)
    	{
			MinName = minName;
			MaxName = maxName;
    	}

		public string MinName { get; private set; }
		public string MaxName { get; private set; }
    }
}