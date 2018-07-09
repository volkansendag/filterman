using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuteDev.FilterManagerTest.Kendo
{
    public class rValue<T>
    {
        public bool Error { get; set; }

        public string MessageCode { get; set; }

        public string Message { get; set; }

        public T Value { get; set; }

        public rValue()
        {
        }

        public rValue(T val)
        {
            this.Value = val;
        }

        public rValue(bool error, string messageCode, string message)
        {
            this.Error = error;
            this.MessageCode = messageCode;
            this.Message = message;
        }

        public rValue(Exception ex) : this(true, "E1", ex.Message)
        {

        }
    }
}