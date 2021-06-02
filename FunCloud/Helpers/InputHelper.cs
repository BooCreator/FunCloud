using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using DataBaseConnector.Ext;
using System.Collections.Generic;

namespace FunCloud.Helpers
{
    public struct Buttons
    {
        public const String Default = "btn bg-darkbrown bg-red-hover btn-button";
        public const String Darkrown = "btn bg-red bg-darkbrown-hover btn-button";
        public const String Red = "btn bg-red bg-red-hover btn-button";
        public const String Brown = "btn bg-brown bg-brown-hover btn-button";
        public const String Primary = "btn btn-primary btn-button";
        public const String Secondary = "btn btn-secondary btn-button";

    }

    public static class InputHelper
    {
        public static MvcHtmlString DropDownList(this HtmlHelper html, String ID, List<Typle> Items, Int32 Checked)
        {
            var input = new TagBuilder("select");
            input.MergeAttribute("class", "form-control");
            input.MergeAttribute("id", ID);
            input.MergeAttribute("name", ID);
            foreach(Typle Item in Items)
            {
                var option = new TagBuilder("option");
                option.MergeAttribute("value", Item.Name);
                option.InnerHtml = Item.Value;
                if (To.Int(Item.Name) == Checked)
                    option.MergeAttribute("selected", "selected");
                input.InnerHtml += option.ToString();
            }

            return new MvcHtmlString(input.ToString());
        }

        public static MvcHtmlString Hidden(this HtmlHelper html, String Name, Int32 Value)
        {
            var input = new TagBuilder("input");
            input.MergeAttribute("type", "hidden");
            input.MergeAttribute("name", Name);
            input.MergeAttribute("value", Value.ToString());
            return new MvcHtmlString(input.ToString());
        }

        public static MvcHtmlString CreateButtonFromLink(this HtmlHelper html, String Text, String Action)
        {
            var input = new TagBuilder("a");
            input.MergeAttribute("role", "button");
            input.MergeAttribute("href", Action);
            input.AddCssClass("btn btn-primary");
            input.InnerHtml = Text;
            return new MvcHtmlString(input.ToString());
        }

        public static MvcHtmlString CreateButton(this HtmlHelper html, String Text, String Action, String ButtonType = Buttons.Default)
        {
            var input = new TagBuilder("a");
            input.MergeAttribute("role", "button");
            input.MergeAttribute("href", Action);
            input.AddCssClass(ButtonType);
            input.InnerHtml = Text;
            return new MvcHtmlString(input.ToString());
        }

        public static MvcHtmlString CreateButton(this HtmlHelper html, String Text, String Action, String Method, Typle<String>[] attr = null)
        {
            var input = new TagBuilder("input");
            input.MergeAttribute("type", "button");
            input.MergeAttribute("value", Text);
            input.MergeAttribute("href", Action);
            input.MergeAttribute("onclick", Method);

            if(attr != null)
                foreach(Typle<string> attribute in attr)
                    input.MergeAttribute(attribute.Name, attribute.Value);

            return new MvcHtmlString(input.ToString());
        }
    }
}