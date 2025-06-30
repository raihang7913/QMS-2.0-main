using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Services {
    public class SystemIFunc : Interfaces.IFunc {
        private readonly IConfiguration _config;
        public SystemIFunc(IConfiguration configur) {
            _config = configur;
        }
        public string connStr {
            get {
                string cS = _config.GetConnectionString("DefaultConnection");
                return cS;
            }
        }
    }
}
