using Taijj.SampleCode;
using UnityEditor;
using UnityEngine;

namespace Taijj.SampleCode
{
    [CustomPropertyDrawer(typeof(OreDropper.Entry))]
    public class DropEntryDrawer : PropertyDuoDrawer
    {
    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
			Draw(new PropertyDuoData
			{
				mainProperty = property,
				position = position,

				propertyName1 = "prefab",
				propertyName2 = "amount",

				label1 = label.text,
				label2 = "#",

				fieldWidth1 = 0.75f,
				labelWidth2 = 0.15f,
				fieldWidth2 = 0.1f
			});
    	}
    }
}