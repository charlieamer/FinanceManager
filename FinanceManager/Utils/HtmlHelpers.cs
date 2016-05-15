using System;
using System.Web.Mvc;
using System.Reflection;

namespace FinanceManager
{
	public class HtmlHelpers
	{
		public static string TextInput (string name, ModelState state, string type, object value)
		{
			string ret = "";
			ret += "<!-- " + name + "-->\n";
			if (state != null && state.Errors.Count > 0) {
				ret += "<div class='form-group'>";
				ret += "<div class='col-md-4'></div>";
				ret += "<div class='col-md-4 text-danger'>";
				ret += state.Errors [0].ErrorMessage;
				ret += "</div>";
				ret += "</div>";
			}
			ret += "<div class='form-group'>";
			ret += "<label class='col-md-4 control-label' for='id_" + name + "'>";
			ret += name;
			ret += "</label>";
			ret += "<div class='col-md-4'>";
			string val = "";
			if (value != null) {
				try {
					val = value.GetType ().GetProperty (name).GetValue (value).ToString ();
				} catch (NullReferenceException ex) {
				}
			}
			ret += "<input id='id_" + name + "' name='" + name + "' type='" + type + "' " +
			"placeholder='" + name + "' class='form-control input-md' value='" + val + "'>";
			ret += "</div>";
			ret += "</div>";
			return ret;
		}

		public static string TextInput (string name, ModelState state, object value)
		{
			return TextInput (name, state, "text", value);
		}
	}
}

