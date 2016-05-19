using System;
using System.Web.Mvc;
using System.Reflection;
using System.Web;

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
				ret += HttpUtility.HtmlEncode (state.Errors [0].ErrorMessage);
				ret += "</div>";
				ret += "</div>";
			}
			return ret;
		}

		public static string FormInput (string name, ModelState state, string type, string val)
		{
			string ret = Error (state);
			ret += "<div class='form-group'>";
			ret += "<label class='col-md-4 control-label' for='id_" + HttpUtility.HtmlEncode (name) + "'>";
			ret += HttpUtility.HtmlEncode (name);
			ret += "</label>";
			ret += "<div class='col-md-4'>";
			ret += "<input id='id_" + HttpUtility.HtmlEncode (name) + "' name='" + HttpUtility.HtmlEncode (name)
			+ "' type='" + HttpUtility.HtmlEncode (type) + "' " +
			"placeholder='" + HttpUtility.HtmlEncode (name) +
			"' class='form-control input-md' value='" + HttpUtility.HtmlEncode (val) + "'>";
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
			return FormInput (name, state, type, val);
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

			string val = time.Value.ToString (Strings.FORMAT_DATE);
			string ret = FormInput (name, state, "text", val);

			ret += "<script>$('#id_" + HttpUtility.HtmlEncode (name) + "').datetimepicker();</script>";
			return ret;
		}
	}
}

