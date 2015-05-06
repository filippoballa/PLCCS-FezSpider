using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAppBackgroundServer
{
    public class DatabaseException : Exception
    {
        private string mex;
  
        public DatabaseException(string mex) 
        {
            this.mex = mex;
        }

        public string Mex 
        {
            get { return this.mex; }
        }
    }
}
