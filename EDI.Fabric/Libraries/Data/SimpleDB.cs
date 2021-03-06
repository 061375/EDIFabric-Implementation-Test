﻿using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using EDI.Fabric.Interfaces;
using System;

namespace EDI.Fabric
{
    class SimpleDB : DbConnection
    {
        private static SqlConnection sqlConnection1 = null;

        public SimpleDB(string conn)
        {
            sqlConnection1 = new SqlConnection(conn);
        }
        public Dictionary<int, Dictionary<string, string>> Query(string sql)
        {
            if(Program.debug)
            {
                Console.WriteLine("Query:");
                Console.WriteLine(sql);
            }

            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();
            /**
             * something to hang our results on
             * I don't expect more than a few 100 records per run
             * A simple query and loop to sort the data should suffice
             *
             * data[0] = {{key, value},{key, value}, etc..},
             * data[1] = {{key, value},{key, value}, etc..}, 
             * etc...
             * */
            Dictionary<int, Dictionary<string, string>> dReturn = new Dictionary<int, Dictionary<string, string>>();

            var reader = cmd.ExecuteReader();
            int row = 0;
            
            do
            { 
                while (reader.Read())
                {
                    int count = reader.FieldCount;
                    Dictionary<string, string> col = new Dictionary<string, string>();
                    for (int i = 0; i < count; i++)
                    {
                        if(false == col.ContainsKey(reader.GetName(i).ToString()))
                            col.Add(reader.GetName(i).ToString(), reader.GetValue(i).ToString());
                    }
                    dReturn.Add(row, col);
                    row++;
                }
            } while (reader.NextResult());

            return dReturn;
        }
        public Dictionary<string, string> GetFirst(string sql)
        {
            Dictionary<int, Dictionary<string, string>> firstResult = Query(sql);
            foreach(var r in firstResult)
            {
                return new Dictionary<string, string>(r.Value);
            }
            return new Dictionary<string, string>();
        }
        public void Close()
        {
            sqlConnection1.Close();
        }
    }

    internal class Return
    {

    }
}
