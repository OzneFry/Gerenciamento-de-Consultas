using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GerenciamentoDeConsultas.GerenciamentoDeContultas.Core.Services
{
    public static class XmlStorageHelper
    {
        public static void SalvarLista<T>(List<T> lista, string caminhoArquivo)
        {
            var pasta = Path.GetDirectoryName(caminhoArquivo);
            if (!string.IsNullOrEmpty(pasta) && !Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                serializer.Serialize(stream, lista);
            }
        }

        public static List<T> CarregarLista<T>(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
                return new List<T>();
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var stream = new FileStream(caminhoArquivo, FileMode.Open))
            {
                return (List<T>)serializer.Deserialize(stream);
            }
        }
    }
}
