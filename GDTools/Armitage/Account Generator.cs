using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Armitage {
    public class Account_Generator : Auto_Start {
        public async Task StartAsync() {

        }
    }
}

/* INVALID EMAIL
POST /database/accounts/registerGJAccount.php HTTP/1.1
Host: www.boomlings.com
Accept: 
Content-Length: 84
Content-Type: application/x-www-form-urlencoded

userName=geq2&password=787898Vvinci&email=mlgkulkid777@gmail..com&secret=Wmfv3899gc9

-6


email in use = -3
unkown, something went wrong = -1
unkown, something went wrong = -2
*/