using Library.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DataBase.Data.IData
{
    public interface IReport_Data
    {
        Task<List<Venta>> GetVentaReporte();
    }
}
