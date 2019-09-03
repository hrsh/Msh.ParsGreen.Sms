namespace ParsGreen.HttpService.Core.Api.Models
{
    public class ParsGreenConfiguration
    {
        public string Token { get; set; }

        public int MaxLenght { get; set; }

        public double MciCoefficent { get; set; }

        public double IrancellCoefficent { get; set; }

        public double RightelCoefficent { get; set; }

        public string DefaultNumber { get; set; }
    }
}