using System;
using System.Data.Linq.Mapping;


namespace LNixTask_db.Table
{
    [Table(Name = "Auto")]
    [Serializable]
    public class Auto
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(Name = "Brand")]
        public string Brand { get; set; }
        [Column(Name = "Model")]
        public string Model { get; set; }
        [Column(Name = "Price")]
        public int Price { get; set; }

        public Auto() { }

        public Auto(string brand, string model, int price)
        {
            Brand = brand;
            Model = model;
            Price = price;
        }
    }
}
