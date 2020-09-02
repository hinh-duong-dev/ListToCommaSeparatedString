using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace ListToCommaSeparatedString
{
    public static class CsvHelper
    {
        private static DataTable ConvertToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private static string ConvertToCsv(this DataTable table, params string[] header)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Join(",", header));

            foreach (DataRow dr in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = string.Format("\"{0}\",", value);
                            builder.Append(value);
                        }
                        else
                        {
                            value = string.Format("{0},", value);
                            builder.Append(value);
                        }
                    }
                    else
                    {
                        string value = string.Format("{0},", string.Empty);
                        builder.Append(value);
                    }
                }

                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        /// <summary>This method to convert list object to string comma separated
        /// </summary>
        public static string ToCsv<T>(this IList<T> dataSource, params string[] header)
        {
            DataTable table = dataSource.ConvertToDataTable();

            string csv = table.ConvertToCsv(header);

            return csv;
        }
    }
}
