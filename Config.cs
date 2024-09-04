using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogWriterMeow
{
    internal class Config:IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public bool PrintLogOntoConsole { get; set; } = false;
        public string LogFilePath { get; set; } = Paths.Log;
    }
}
