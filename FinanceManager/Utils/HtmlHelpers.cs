using System;
using System.Web.Mvc;
using System.Reflection;

namespace FinanceManager
{
	public class HtmlHelpers
	{
		public static string Error (ModelState state)
		{
			string ret = "";
			if (state != null && state.Errors.Count > 0) {
				ret += "<div class='form-group'>";
				ret += "<div class='col-md-4'></div>";
				ret += "<div class='col-md-4 text-danger'>";
				ret += state.Errors [0].ErrorMessage;
				ret += "</div>";
				ret += "</div>";
			}
			return ret;
		}

		public static string FormInput (string name, ModelState state, string type, string val)
		{
			string ret = Error (state);
			ret += "<!-- " + name + "-->\n";

			ret += "<div class='form-group'>";
			ret += "<label class='col-md-4 control-label' for='id_" + name + "'>";
			ret += name;
			ret += "</label>";
			ret += "<div class='col-md-4'>";
			ret += "<input id='id_" + name + "' name='" + name + "' type='" + type + "' " +
			"placeholder='" + name + "' class='form-control input-md' value='" + val + "'>";
			ret += "</div>";
			ret += "</div>";
			return ret;
		}

		public static string TextInput (string name, ModelState state, string type, object value)
		{
			string val = "";
			if (value != null) {
				try {
					val = value.GetType ().GetProperty (name).GetValue (value).ToString ();
				} catch (NullReferenceException) {
				}
			}
			return FormInput (name, state, "text", val);
		}

		public static string TextInput (string name, ModelState state, object value)
		{
			return TextInput (name, state, "text", value);
		}

		public static string DateInput (string name, ModelState state, object value)
		{
			DateTime? time = null;
			try {
				time = value.GetType ().GetProperty (name).GetValue (value) as DateTime?;
			} catch (NullReferenceException) {
			}

			if (time == null)
				time = DateTime.Now;

			string val = time.Value.ToString ("yyyy/MM/dd HH:mm");
			string ret = FormInput (name, state, "text", val);

			ret += "<script>$('#id_" + name + "').datetimepicker();</script>";
			return ret;
		}
	}
}

