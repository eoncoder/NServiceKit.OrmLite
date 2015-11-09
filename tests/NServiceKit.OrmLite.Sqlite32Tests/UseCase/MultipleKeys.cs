using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.OrmLite.Sqlite;
using NServiceKit.DataAnnotations;

namespace NServiceKit.OrmLite.Tests.UseCase
{
    /// <summary>A simple use case.</summary>
	[TestFixture]
	public class MultipleKeys
	{
        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			//Inject your database provider here
			OrmLiteConfig.DialectProvider = new SqliteOrmLiteDialectProvider();
		}


        [Alias("UsersMultipleKey")]
        public class UsersMultipleKey
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            [PrimaryKey]
            public long Id { get; set; }
            [PrimaryKey]
            public long Id2 { get; set; }

            /// <summary>Gets or sets the value.</summary>
            /// <value>The nAME.</value>
            public string Name { get; set; }

            /// <summary>Gets or sets the created date.</summary>
            /// <value>The created date.</value>
            public DateTime CreatedDate { get; set; }
        }


        [Test]
        public void Simple_CreateTable_UsersMultipleKey_example()
        {
            var path = Config.SqliteFileDb;
            if (File.Exists(path))
                File.Delete(path);
            //using (IDbConnection db = ":memory:".OpenDbConnection())
            using (IDbConnection db = path.OpenDbConnection())
            {
                var dialectProvider = OrmLiteConfig.DialectProvider;

                string v_strQuery = dialectProvider.ToCreateTableStatement(typeof(UsersMultipleKey));

                Assert.IsNotNullOrEmpty(v_strQuery);

                db.CreateTable<UsersMultipleKey>(true);

            }

            File.Delete(path);
        }

        [Test]
        public void Simple_CRUD_UsersMultipleKey_example()
        {
            var path = Config.SqliteFileDb;
            if (File.Exists(path))
                File.Delete(path);
            //using (IDbConnection db = ":memory:".OpenDbConnection())
            using (IDbConnection db = path.OpenDbConnection())
            {
                var dialectProvider = OrmLiteConfig.DialectProvider;

                string v_strQuery = dialectProvider.ToCreateTableStatement(typeof(UsersMultipleKey));

                Assert.IsNotNullOrEmpty(v_strQuery);

                db.CreateTable<UsersMultipleKey>(true);

                db.Insert(new UsersMultipleKey { Id = 1, Id2 = 1, Name = "A", CreatedDate = DateTime.Now });
                db.Insert(new UsersMultipleKey { Id = 2, Id2 = 2, Name = "B", CreatedDate = DateTime.Now });
                db.Insert(new UsersMultipleKey { Id = 3, Id2 = 3, Name = "B", CreatedDate = DateTime.Now });

                var rowsB = db.Select<UsersMultipleKey>("Name = {0}", "B");
                var rowsB1 = db.Select<UsersMultipleKey>(user => user.Name == "B");

                Assert.That(rowsB, Has.Count.EqualTo(2));
                Assert.That(rowsB1, Has.Count.EqualTo(2));

                var rowIds = rowsB.ConvertAll(x => new { x.Id, x.Id2 });
                //Assert.That(rowIds, Is.EquivalentTo(new List<dynamic> { new { Id = 2, Id2 = 2 }, new { Id = 3, Id2 = 3 } }));

                rowsB.ForEach(x => db.Delete(x));

                rowsB = db.Select<UsersMultipleKey>("Name = {0}", "B");
                Assert.That(rowsB, Has.Count.EqualTo(0));

                var rowsLeft = db.Select<UsersMultipleKey>();
                Assert.That(rowsLeft, Has.Count.EqualTo(1));

                Assert.That(rowsLeft[0].Name, Is.EqualTo("A"));

                rowsLeft[0].Name = "C";
                db.Update<UsersMultipleKey>(rowsLeft[0]);

                rowsLeft = db.Select<UsersMultipleKey>();
                Assert.That(rowsLeft, Has.Count.EqualTo(1));

                Assert.That(rowsLeft[0].Name, Is.EqualTo("C"));

            }

            File.Delete(path);
        }

	}

}