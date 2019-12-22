using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography
{
    class User
    {
        protected string ad;
        protected string sifre;

        public string Ad
        {
            get { return ad; }
            set { value = ad; }
        }

        public string Sifre
        {
            get { return sifre; }
            set { value = sifre; }
        }

    }
}
