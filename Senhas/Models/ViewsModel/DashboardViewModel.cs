using System;
using System.Collections.Generic;
using Senhas.Models.ViewsModel;

namespace Senhas.Models.ViewModels
{
    public class DashboardViewModel
    {
        public double TempoMedioEsperaSenhas { get; set; } // em minutos
        public List<AtendenteResumoViewModel> Atendentes { get; set; }
    }

    
}