using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Models;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Interfaces
{
    public interface IServicoConsulta
    {
        bool AgendarConsulta(Consulta consulta);
        bool CancelarConsulta(int consultaId);
        List<Consulta> ObterConsultasPorData(DateTime data);
        List<Consulta> ObterConsultasPorMedico(int medicoId, DateTime data);
        string[,] ObterMatrizConsultas();
    }
}