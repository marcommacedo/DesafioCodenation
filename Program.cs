using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace codenation
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "answer.json";
            var answer = File.ReadAllText(path);
            var desafio = JsonConvert.DeserializeObject<Desafio>(answer);

            var numeroCasas = desafio.numero_casas;
            var decifrado = string.Empty;

            // decifrado
            foreach (var letra in desafio.cifrado)
            {
                var char_index = (int)letra - 97;
                var char_decifrado = letra;

                if (char_index >= 0 && char_index <= 26)
                {
                    var char_index_decifrado = char_index + numeroCasas + 97;

                    if (char_index_decifrado > 122)
                        char_index_decifrado -= 26;

                    char_decifrado = (char)char_index_decifrado;
                }

                decifrado += char_decifrado;
            }

            desafio.decifrado = decifrado;

            // resumo_criptografico
            var sha1 = SHA1.Create();
            Byte[] hashData = sha1.ComputeHash(Encoding.ASCII.GetBytes(desafio.decifrado));
            var resumo_criptografico = string.Empty;

            foreach (var x in hashData)
            {
                resumo_criptografico += x.ToString("X2");
            }

            desafio.resumo_criptografico = resumo_criptografico.ToLower();

            // submit
            File.WriteAllText(path, JsonConvert.SerializeObject(desafio));

            var client = new RestClient("");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("answer", path);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }

    class Desafio
    {
        public int numero_casas { get; set; }
        public string token { get; set; }
        public string cifrado { get; set; }
        public string decifrado { get; set; }
        public string resumo_criptografico { get; set; }
    }
}
