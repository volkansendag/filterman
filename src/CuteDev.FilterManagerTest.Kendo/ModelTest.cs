namespace CuteDev.FilterManagerTest.Kendo
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class ModelTest : DbContext
    {
        // Your context has been configured to use a 'ModelTest' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'CuteDev.FilterManagerTest.Kendo.ModelTest' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'ModelTest' 
        // connection string in the application configuration file.
        public ModelTest()
            : base("name=ModelTest")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}