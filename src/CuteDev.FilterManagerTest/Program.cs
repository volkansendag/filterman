using CuteDev.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuteDev.FilterManagerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var mdl = new TestModel();

            //mdl.MyEntities.Add(new MyEntity()
            //{
            //    Id = 1,
            //    Name = "ahmet"
            //});
            //mdl.SaveChanges();

            var prms = new pList()
            {
                filter = new filter()
                {
                    filters = new List<filterItem>()
                    {
                        new filterItem()
                        {
                            field = "Name",
                            @operator = "eq",
                            value = "volkan"
                        }
                    }
                }
            };

            var list = mdl.MyEntities.Filter(prms);

            Console.WriteLine("test");
            Console.ReadKey();

        }
    }
}
