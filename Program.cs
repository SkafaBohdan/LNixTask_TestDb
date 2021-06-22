using LNixTask_db.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.Linq;
using System.Linq;
using Microsoft.Data.SqlClient;


namespace LNixTask_db
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Task_Bd;Integrated Security=True";
            //Создание подключения
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение открыто");
                // Вывод информации о подключении
                Console.WriteLine("Свойства подключения:");
                Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
                Console.WriteLine("\tБаза данных: {0}", connection.Database);
                Console.WriteLine("\tСервер: {0}", connection.DataSource);
                Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
                Console.WriteLine("\tСостояние: {0}", connection.State);
                Console.WriteLine("\tWorkstationld: {0}", connection.WorkstationId);
            }
            Console.WriteLine("Подключение закрыто...");


            List<Auto> autos = new List<Auto>
            {
                new Auto("Opel", "Turbo", 5000),
                new Auto("Nissan", "Shyk", 5500),
                new Auto("Lada", "2111", 4000)
            };

            try
            {
                SaveDataAsync(autos);
                List<Auto> autos1 = await GetDataAsync();

                foreach (var obj in autos1)
                {
                    Console.WriteLine(obj.Brand.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("The END");
            }

            try
            {
                DataContext db = new DataContext(connectionString);

                PrintAutos(db);

                AddAuto(db, "Hynday", "Tucson", 7000);
                AddAuto(db, "Opel", "Turbo", 5000);
                AddAuto(db, "LADA", "21111", 6500);

                PrintAutos(db);
                EditPrice(db, 2, 6500);
                PrintAutos(db);
                DeleteAuto(db, 9);
                PrintAutos(db);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public async static void SaveDataAsync(List<Auto> autos)
        {
            Console.WriteLine("Save async");
            await Task.Run(() => SaveDataAs(autos));
            Console.WriteLine("End save async");
        }

        public static async Task<List<Auto>> GetDataAsync()
        {
            return await Task.Run(() => GetData());
        }

        public static void SaveDataAs(List<Auto> autos)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Auto>));
            using (FileStream stream = new FileStream("autos.xml", FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, autos);
            }
        }

        public static List<Auto> GetData()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Auto>));
            Thread.Sleep(1500);
            using (FileStream stream = new FileStream("autos.xml", FileMode.OpenOrCreate))
            {
                List<Auto> autos = (List<Auto>)serializer.Deserialize(stream);
                return autos;
            }
        }



        public static void AddAuto(DataContext db, string brand, string model, int price)
        {
            Auto autos = new Auto() { Brand = brand, Model = model, Price = price};
            db.GetTable<Auto>().InsertOnSubmit(autos);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("___________________");
            Console.WriteLine("Добалено новое авто");
        }

        public static void PrintAutos(DataContext db)
        {
            var autos = db.GetTable<Auto>();
            Console.WriteLine("Auto:");
            foreach (var a in autos)
            {
                Console.WriteLine($"{ a.Id} \t{ a.Brand} \t{ a.Model} \t{ a.Price}");
            }
        }

        public static void EditPrice(DataContext db, int id, int newPrice)
        {
            Auto autos = (from a in db.GetTable<Auto>() where a.Id == id select a).FirstOrDefault();
            if (autos != null)
            {
                autos.Price = newPrice;
                db.SubmitChanges();
                Console.WriteLine("___________________");
                Console.WriteLine($"Изменили цену авто на: { autos.Model} \t { autos.Price}");
            }
            else
                Console.WriteLine("Error 404");
        }

        public static void DeleteAuto(DataContext db, int id)
        {
            Auto autos = (from a in db.GetTable<Auto>() where a.Id == id select a).FirstOrDefault();
            if (autos != null)
            {
                db.GetTable<Auto>().DeleteOnSubmit(autos);
                db.SubmitChanges();
                Console.WriteLine("___________________");
                Console.WriteLine($"Авто с id: { id} удалено");
            }
            else
                Console.WriteLine("Error 404");
        }

    }
}
