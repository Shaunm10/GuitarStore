// -----------------------------------------------------------------------
// <copyright file="NHibernateBase.cs" company="The Advisory Board Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


using NHibernate.Event;

namespace NHibernate.GuitarStore.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using log4net.Config;
    using NHibernate.Cfg;
    using IFormatter = System.Runtime.Serialization.IFormatter;
    using NHibernate.GuitarStore.DataAccess.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NHibernateBase
    {
        private static Configuration Configuration { get; set; }
        protected static ISessionFactory SessionFactory { get; set; }
        private static ISession session = null;
        private static IStatelessSession statelessSession = null;
        private static string SerializedConfiguration = System.Configuration.ConfigurationManager.AppSettings["SerializedFilename"];

        public NHibernateBase()
        {
            XmlConfigurator.Configure();
        }

        public static Configuration ConfigureNHibernate(string assembly)
        {
            // get the configuration from the last serialized file.
            Configuration = LoadConfigurationFromFile();

            // if the configuration isn't loaded
            if (Configuration == null)
            {
                // reload it the normal way (validate all the .hbm's)
                Configuration = new Configuration();
                Configuration.SetInterceptor(new SQLInterceptor());
                Configuration.AddAssembly(assembly);
                Configuration.EventListeners.PostDeleteEventListeners = new IPostDeleteEventListener[]{ new AuditDeleteEvent() };

                // save the configuration via a serialized file.
               // SaveConfigurationToViaSerialization();
            }

            return Configuration;
        }

        private static void SaveConfigurationToViaSerialization()
        {
            var file = File.Open(SerializedConfiguration, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file,Configuration);
            file.Close();
        }

        /// <summary>
        /// Determines if the serialized configuration file is still valid.
        /// </summary>
        private static bool IsConfigurationFileValid
        {
            get
            {
                try
                {
                    // get info about this assembly.
                    Assembly assembly = Assembly.Load("Nhibernate.GuitarStore");
                    FileInfo asmInfo = new FileInfo(assembly.Location);
                    
                    // get info about the serializedConfiguration saved.
                    FileInfo configInfo = new FileInfo(SerializedConfiguration);
                    
                    // if the last write time of the serialized configuration file is later or the same as the
                    // current assembly return true.
                    return configInfo.LastWriteTime >= asmInfo.LastWriteTime;
                }
                catch (Exception ex)
                {
                    // if we can't find the file etc, just return false.
                    return false;
                }
            }
        }

        /// <summary>
        /// Loads the configuration from the file.
        /// </summary>
        /// <returns></returns>
        private static NHibernate.Cfg.Configuration LoadConfigurationFromFile()
        {
            // first check to see if the last file saved is valid.
            if (!IsConfigurationFileValid)
            { 
                return null;
            }

            try
            {
                using (var file = File.Open(SerializedConfiguration, FileMode.Open))
                {
                    var binaryFormatter = new BinaryFormatter();
                    return (Configuration)binaryFormatter.Deserialize(file);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void Initilize(string assembly)
        {
            Configuration = ConfigureNHibernate(assembly);
            SessionFactory = Configuration.BuildSessionFactory();
        }

        /// <summary>
        /// Get's a Nhibernate session
        /// </summary>
        public static ISession Session
        {
            get
            {
                if (session == null)
                {
                    session = SessionFactory.OpenSession();
                }
                return session;
            }
        }

        /// <summary>
        /// Get's a Nhibernate Stateless Session
        /// </summary>
        public static IStatelessSession StatelessSession
        {
            get
            {
                if (statelessSession == null)
                {
                    statelessSession = SessionFactory.OpenStatelessSession();
                }
                return statelessSession;
            }
        }

        public IList<T> ExecuteICriteria<T>()
        {
            using (ITransaction tran = Session.BeginTransaction())
            {
                try
                {
                    IList<T> result = Session.CreateCriteria(typeof(T)).List<T>();
                    tran.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }


}
