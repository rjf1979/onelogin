namespace OneLogin.Core
{
    public class ExecuteResult
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string ResultCode { get; set; }
        public object Data { get; set; }


        public static ExecuteResult Ok(string message = "", string resultCode = "")
        {
            return new ExecuteResult
            {
                IsError = false,
                Message = message,
                ResultCode = resultCode,
                Data = "SUCCESS"
            };
        }

        public static ExecuteResult Ok(object data,string message = "", string resultCode = "")
        {
            return new ExecuteResult
            {
                IsError = false,
                Message = message,
                ResultCode = resultCode,
                Data = data
            };
        }

        public static ExecuteResult Error(string message, string resultCode = "")
        {
            return new ExecuteResult
            {
                IsError = true,
                Message = message,
                ResultCode = resultCode
            };
        }

        public static implicit operator ExecuteResult(ExecuteResult<object> executeResult)
        {
            if (executeResult.IsError)
            {
                return Error(executeResult.Message, executeResult.ResultCode);
            }

            return Ok(executeResult.Message, executeResult.ResultCode);
        }
    }

    public class ExecuteResult<T>
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string ResultCode { get; set; }
        public T Data { get; set; }

        public static ExecuteResult<T> Ok(T data, string message = "操作成功", string resultCode = "")
        {
            return new ExecuteResult<T>
            {
                IsError = false,
                Data = data,
                Message = message,
                ResultCode = resultCode
            };
        }

        public static ExecuteResult<T> Error(string message, string resultCode = "")
        {
            return new ExecuteResult<T>
            {
                IsError = true,
                Message = message,
                ResultCode = resultCode
            };
        }
    }
}
