using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace csVazEdit
{
    class Program
    {

        // Classe que representa um histórico de vazões.        
        public class historicoVazoes
        {
            public int anoInicial = 0;
            public int anoFinal = 0;
            public Dictionary<int,List<int>> valores = new Dictionary<int, List<int>>();            
        }

        static void Main(string[] args)
        {            
            // Lê os dados de vazão de uma arquivo binário para um objeto 'histocoVazoes'.
            
            
            
            historicoVazoes meuHistorico = leVazoes("tests/vazoes_original_ONS.dat");

           

            // Salva um histórico de vazões em um arquivo de tipo especificado.
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.dat", "binario");     // formato binário
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedTime);
            
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.csv", "csv");         // formato texto csv
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.txt", "vazEdit");     // formato texto VazEdit                 
        }

        /// <sumary>
        /// Lê as vazões de um arquivo binário para um objeto 'historicoVazoes'.
        /// </sumary>
        public static historicoVazoes leVazoes(string nomeArquivo, int anoInicial=1931, int numPostos = 320)
        {
            historicoVazoes histLocal = new historicoVazoes();            
            histLocal.anoInicial = anoInicial;

            for(int p = 1; p < numPostos+1; p++) histLocal.valores[p] = new List<int>();

            // Antigo e ineficiente...            
            // using (BinaryReader reader = new BinaryReader(File.Open(nomeArquivo, FileMode.Open)))
            // {
            //     while (reader.BaseStream.Position != reader.BaseStream.Length)
            //     {
            //         registros++;                    
            //         histLocal.valores[posto].Add(reader.ReadInt32());
            //         if (posto == numPostos) posto = 1; else posto++;                    
            //     }           

            // }

            byte [] data = File.ReadAllBytes(nomeArquivo);
            for(int r = 0; r < (data.Length-numPostos); r=r+4*(numPostos))
            {
                for(int p=0;p<numPostos;p++)
                {                       
                    histLocal.valores[p+1].Add(BitConverter.ToInt32(new ArraySegment<byte>(data,(p*4)+r, 4)));
                }
            }           

            // histLocal.anoFinal = histLocal.anoInicial + (registros/(12*numPostos))-1;                       
            return histLocal;
        }    

                
        /// <sumary>
        /// Salva um histórico de vazões para um arquivo binário, texto no formato 'VazEdit' e texto
        /// no formato 'csv'.
        /// </sumary>
        public static void salvaVazoes(historicoVazoes vazoesHist, string nomeArquivo, string tipoArquivo)
        {
            // Armazena o número de registros para o primeiro posto. 
            // Para arquivos válidos, o número de registro é igual para todos os postos.
            int n = vazoesHist.valores[1].Count();

            // Loop para salvar os dados no formato binário do ONS.
            if (tipoArquivo=="binario")
            {               
                using (BinaryWriter writer = new BinaryWriter(File.Open(nomeArquivo,FileMode.Create)))
                {
                    for (int reg = 0; reg < n; reg++) 
                    {
                        for (int posto = 1; posto < vazoesHist.valores.Count()+1;posto++)
                        {                            
                            writer.Write(vazoesHist.valores[posto][reg]);
                        }
                    }
                }                   
            }            

            // Loop para salvar os dados em formato texto do ONS (software VazEdit) ou separado por vírgulas ("csv").
            else if (tipoArquivo=="csv" || tipoArquivo =="vazEdit")
            {
                string sep = "";           
                List<int> adj;
                
                if (tipoArquivo=="csv")
                {
                    sep = ",";
                    adj = new List<int> {0,0,0};
                }
                else
                {
                    adj = new List<int> {3,6,5};
                }
                
                using (StreamWriter writer = new StreamWriter(File.Open(nomeArquivo,FileMode.Create)))
                {
                    for (int posto = 1; posto < vazoesHist.valores.Count()+1; posto++)                    
                    {   
                        if (vazoesHist.valores[posto].Sum()>0)
                        {                        
                            string sPosto = posto.ToString().PadLeft(adj[0],' ');
                            int ano = vazoesHist.anoInicial;

                            for(int reg = 0; reg < n; reg = reg + 12)
                            {
                                string sVazoes = "";
                                for(int m = 0; m < 12; m++)
                                {
                                    sVazoes+= vazoesHist.valores[posto][reg+m].ToString("D2").PadLeft(adj[1],' ') + sep;                               
                                }
                                string sAno = ano.ToString().PadLeft(adj[2],' ');
                                string sSaida =  sPosto + sep +  sAno + sep + sVazoes;
                                writer.WriteLine(sSaida);
                                ano++;
                            }
                        }
                    }
                }
            }
            else 
            {
                Console.Write("Tipo de arquivo inválido!");
            }
        }

        public static void mudaVazao()
        {

        }

    }
}