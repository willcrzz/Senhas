namespace Senhas.Models.ViewsModel
{
    public class AtendenteResumoViewModel
    {
        public string Nome { get; set; }
        public int SenhasAtendidas { get; set; }
        public double TempoMedioAtendimento { get; set; } // em minutos
    }
}
