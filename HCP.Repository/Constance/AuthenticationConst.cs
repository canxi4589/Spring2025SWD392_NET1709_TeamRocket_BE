using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class AuthenticationConst
    {
        //validate
        public const string EmailTaken = "This Email is already taken!!";
        public const string PhoneTaken = "This Number is already taken!!";

        //success
        public const string EmailNotTaken = "This Email is okay to use!!";
        public const string ChangPasswordSuccess = "Change Dafault Password Successfully!!";

        //create
        public const string StaffNotice = "About the Creation of your Staff Account";
    }
}
