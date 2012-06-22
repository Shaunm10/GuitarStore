




namespace NHibernate.GuitarStore.Console
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
using NHibernate.GuitarStore.Common;
using NHibernate.GuitarStore.DataAccess;
using NHibernate.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            //InitlizationTest();
            QueriesToBeCalled();
        }

        static void QueriesToBeCalled()
        {

            NHibernateBase nHibernateBase = new NHibernateBase();
            nHibernateBase.Initilize("NHibernate.GuitarStore");
            System.Console.WriteLine("NHibernate.GuitarStore assembly Initialized.");

            IList<Inventory> list1 = NHibernateBase.StatelessSession.CreateCriteria("Inventory").List<Inventory>();

            IList<Inventory> list2 = NHibernateBase.Session.CreateCriteria(typeof (Inventory)).List<Inventory>();

            IQueryable<Inventory> linq =
                (from l in NHibernateBase.Session.Query<Inventory>() select l);

        }

        static void InitlizationTest()
        {
            try
            {
                NHibernateBase nHibernateBase = new NHibernateBase();
                nHibernateBase.Initilize("NHibernate.GuitarStore");
                System.Console.WriteLine("NHibernate.GuitarStore assembly Initialized.");

                System.Console.ReadLine();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                while (ex.InnerException != null)
                {
                    message += "\r - InnerException: " + ex.InnerException.Message;
                    ex = ex.InnerException;
                }

                System.Console.WriteLine();
                System.Console.WriteLine("***** ERROR *****");
                System.Console.WriteLine(message);
                System.Console.WriteLine();
                System.Console.ReadLine();
            }
        }
    }
}
