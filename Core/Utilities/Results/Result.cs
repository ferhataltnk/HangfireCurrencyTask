using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public class Result<T> : IResult<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }


        public Result(T data,bool success, string message)
        {
            Data = data;
            Message = message;
            Success = success;

        }
    }
}
