using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Constance
{
    public static class CommonConst
    {
        //Error
        public const string DatabaseError = "Database error occurred.";
        public const string InternalError = "Internal Server Error.";
        public const string UnauthorizeError = "No permission!";
        public const string SomethingWrongMessage = "Something went wrong after functioning!";
        public const string FileHandleError = "File exceeds the size limit of 5 MB.";
        public const string NotFoundError = "Not Found";

        //Success
        public const string SuccessTaskMessage = "Task done successfully";
    }
}
