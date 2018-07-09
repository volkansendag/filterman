using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuteDev.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class rList<T>
    {
        #region Properties

        public bool Error { get; set; }

        public string MessageCode { get; set; }

        public string Message { get; set; }

        public int Count { get; set; }

        public List<T> Values { get; set; }

        #endregion

        #region Constructor

        public rList()
        {
            this.Values = new List<T>();
            this.Count = 0;
        }

        public rList(bool error, string messageCode, string message)
        {
            this.Error = error;
            this.MessageCode = messageCode;
            this.Message = message;
        }

        public rList(Exception ex) : this(true, "E1", ex.Message)
        { }

        public rList(List<T> values, int count)
        {
            this.Values = values;
            this.Count = count;
        }

        public rList(List<T> values)
            : this(values.ToList(), values == null ? 0 : values.Count)
        { }


        public rList(IQueryable<T> query)
            : this(query.ToList())
        { }


        public rList(IQueryable<T> query, pList prms)
        {
            if (prms.take > 0)
            {
                this.Values = query.Skip(prms.skip).Take(prms.take).ToList();
                this.Count = query.Count();
            }
            else
            {
                this.Values = query.ToList();
                this.Count = this.Values.Count;
            }
        }

        #endregion

        #region Functions

        public void Add(T val)
        {
            this.Values.Add(val);
            this.Count++;
        }

        #endregion
    }
}
