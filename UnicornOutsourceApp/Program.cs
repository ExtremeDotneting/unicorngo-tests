using RollingOutTools.CmdLine;
using RollingOutTools.Storage;
using RollingOutTools.Storage.JsonFileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOutsourceApp
{
    class Program
    {
       

        static void Main(string[] args)
        {   
            StorageHardDrive.InitDependencies(
                 new JsonLocalStorage(
                     "values_storage"
                     )
                 );
            CmdLineExtension.Init();
            var switcher = new DefaultConsoleSwitcher();
            switcher.RunDefault(new MainCmd(switcher));


        }
    }

}
