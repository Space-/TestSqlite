using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using Dapper;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace TestSqlite
{
    internal class Program
    {
        public static readonly int NumOfData = 5000;

        private static void Main(string[] args)
        {
            InitSQLiteDb();
            ShowNumOfData();
            TestInsert();
            TestSelect();
            Console.Read();
        }

        private static string dbPath = @".\Test.sqlite";
        private static string cnStr = "data source=" + dbPath;

        private static void ShowNumOfData()
        {
            Console.WriteLine("Numbers of data: {0}", NumOfData);
        }

        private static void InitSQLiteDb()
        {
            if (File.Exists(dbPath)) return;
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute(@"
                CREATE TABLE Player (
                    Id VARCHAR(2),
                    TraceId VARCHAR(50),
                    CreatedOn DATETIME
                )");
            }
        }

        private static void TestInsert()
        {
            Console.Write("Insert:");
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Open();
                Stopwatch sw = new Stopwatch();
                sw.Start();

                cn.Execute("DELETE FROM Player");
                //參數是用@paramName
                var insertScript =
                    "INSERT INTO Player VALUES (@Id, @TraceId, @CreatedOn)";

                using (SQLiteTransaction tran = cn.BeginTransaction())
                {
                    for (int i = 1; i <= NumOfData; i++)
                    {
                        cn.Execute(insertScript, new Player(i.ToString(), Guid.NewGuid().ToString(), DateTime.Now));
                    }
                    tran.Commit();
                }
                sw.Stop();
                Console.WriteLine($"Duration={sw.ElapsedMilliseconds:n0}ms");

                //測試Primary Key
                //                try
                //                {
                //                    //故意塞入錯誤資料
                //                    cn.Execute(insertScript, Player.TestData[0]);
                //                    throw new ApplicationException("失敗：未阻止資料重複");
                //                }
                //                catch (Exception ex)
                //                {
                //                    Console.WriteLine($"測試成功:{ex.Message}");
                //                }
            }
        }

        private static void TestSelect()
        {
            Console.Write("Select:");
            Stopwatch sw = new Stopwatch();

            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Open();
                sw.Start();
                var list = cn.Query("SELECT * FROM Player");
                //                Console.WriteLine(
                //                    JsonConvert.SerializeObject(list, (Newtonsoft.Json.Formatting)Formatting.Indented));
            }
            sw.Stop();
            Console.Write($"Duration={sw.ElapsedMilliseconds:n0}ms");
        }
    }
}