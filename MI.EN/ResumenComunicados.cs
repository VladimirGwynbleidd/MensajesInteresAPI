using System.Runtime.Serialization;

namespace MI.EN
{
    [DataContract]
    public class ResumenComunicados
    {
        private string _DescError;

        [DataMember(Order = 0)]
        public int c_id { get; set; }

        [DataMember(Order = 1)]
        public string Encabezado { get; set; }

        [DataMember(Order = 2)]
        public string Detalle { get; set; }

        [DataMember(Order = 3)]
        public string f_VigenciaIni { get; set; }
        [DataMember(Order = 4)]
        public string f_VigenciaFin { get; set; }

        [DataMember(Order = 5)]
        public string DescError
        {
            get
            {
                if (_DescError == null)
                    _DescError = string.Empty;
                return _DescError;
            }
            set => _DescError = value.ToString();
        }

        [DataMember(Order = 6)]
        public int visto { get; set; }

    }
}
