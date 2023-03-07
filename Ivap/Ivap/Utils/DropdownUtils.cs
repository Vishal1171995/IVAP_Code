using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivap.Utils
{
    public class DropdownUtils
    {

        public static  SelectList ToSelectList(DataTable table, string valueField, string textField, string value = "",string ddlType="Normal")
        {
            List<SelectListItem> list = new List<SelectListItem>();

            if (ddlType.Trim().ToUpper() == "NORMAL")
            {
                string DataType = table.Columns[valueField].DataType.Name.ToString();
                if (DataType == "Int32")
                {
                    list.Add(new SelectListItem()
                    {
                        Text = "--Select--",
                        Value = "0",
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = "--Select--",
                        Value = "",
                    });
                }
            }

            foreach (DataRow row in table.Rows)
            {
                if (row[valueField].ToString() == value)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = row[textField].ToString(),
                        Value = row[valueField].ToString(),
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = row[textField].ToString(),
                        Value = row[valueField].ToString()
                    });
                }

            }
            if (string.IsNullOrEmpty(value))
            {
                return new SelectList(list, "Value", "Text");
            }
            else
            {
                return new SelectList(list, "Value", "Text", value);
            }

        }



        public static SelectList ToMultiSelectList(DataTable table, string valueField, string textField, string value = "", bool all=false)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (DataRow row in table.Rows)
            {
                if (row[valueField].ToString() == value)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = row[textField].ToString(),
                        Value = row[valueField].ToString(),
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Text = row[textField].ToString(),
                        Value = row[valueField].ToString()
                    });
                }

            }
            if (all)
            {
                list.Add(new SelectListItem()
                {
                    Text = "All",
                    Value = "-1",
                });
            }
            if (string.IsNullOrEmpty(value))
            {
                return new SelectList(list, "Value", "Text");
            }
            else
            {
                return new SelectList(list, "Value", "Text", value);
            }

        }

    }
}