using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace csVazEdit
{
    class Program
    {

        /// <sumary>
        /// Classe que representa um histórico de vazões.        
        /// </sumary>
        public class historicoVazoes
        {
            public int anoInicial = 0;
            public int anoFinal = 0;
            public Dictionary<int,List<int>> valores = new Dictionary<int, List<int>>();            
        }

        /// <sumary>
        /// Método de entrada. 
        /// </sumary>
        static void Main(string[] args)
        {            
            // Lê os dados de vazão de uma arquivo binário para um objeto 'histocoVazoes'.
            var watch = System.Diagnostics.Stopwatch.StartNew();
            historicoVazoes meuHistorico = leVazoes("tests/vazoes_original_ONS.dat");
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedTime);

            // Salva um histórico de vazões em um arquivo de tipo especificado.
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.dat", "binario");     // formato binário
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.csv", "csv");         // formato texto csv
            salvaVazoes(meuHistorico, "tests/vazoes_ons_2.txt", "vazEdit");     // formato texto VazEdit      

            mudaVazao(meuHistorico,1,1,2022,400);
            mudaVazao(meuHistorico,1,2,1931,999);
            salvaVazoes(meuHistorico, "tests/vazoes_ons_21.txt", "vazEdit");     // formato texto VazEdit      


        }

        /// <sumary>
        /// Método para ler as vazões de um arquivo binário para um objeto 'historicoVazoes'.
        /// Argumentos:
        ///     nomeArquivo - caminho completo do arquivo de vazões binários no formato ONS;
        ///     anoInicial  - (Opcional) ano inicial do histórico de vazões. Valor padrão 1931;
        ///     numPostos   - (Opcional) número de postos do arquivo de vazões. 
        ///         O ONS utiliza 320 postos para o modo "operação" e 600 postos para o modo "planejamento".
        /// Retorno:
        ///     Objeto tipo 'historicoVazoes' com os dados lidos do arquivo.
        /// </sumary>        
        public static historicoVazoes leVazoes(string nomeArquivo, int anoInicial=1931, int numPostos = 320)
        {
            // Cria uma instância de um objeto 'historicoVazoes' para conter os valores de vazões lidas.
            historicoVazoes histLocal = new historicoVazoes();            
            
            // Inicia o objeto com o ano inicial e listas de vazões vazias.
            histLocal.anoInicial = anoInicial;
            for(int p = 1; p < numPostos+1; p++) histLocal.valores[p] = new List<int>();

            // Lê todos os bytes do arquivo.
            byte [] data = File.ReadAllBytes(nomeArquivo);
            
            // Aloca os bytes lidos no objeto.
            for(int r = 0; r < (data.Length-numPostos); r=r+4*(numPostos))
            {
                for(int p=0;p<numPostos;p++)
                {                       
                    histLocal.valores[p+1].Add(BitConverter.ToInt32(new ArraySegment<byte>(data,(p*4)+r, 4)));
                }
            }

            // Calcula e aloca o ano final no objeto 'historicoVazoes'.
            histLocal.anoFinal = histLocal.anoInicial + (data.Length/(48*numPostos))-1;                       
            return histLocal;
        }    
                
        /// <sumary>
        /// Salva um histórico de vazões para um arquivo de tipo especificado.
        /// Os tipo de arquivo permitidos são:
        ///     "binário"   - arquivo binário no formato ONS;
        ///     "vazEdit"   - arquivo texto no formato do aplicativo 'VazEdit' do ONS;
        ///     "csv"       - arquivo texto no formato separado por vírgulas.
        /// Argumentos:
        ///     vazoesHist  - objeto do tipo 'historicoVazoes' com dados a serem salvos;
        ///     nomeArquivo - caminho completo do arquivo de vazões a ser salvo; 
        ///     tipoArquivo - (Opcional) tipo de arquivo (ver acima).
        /// Retorno:
        ///     Nenhum.       
        /// </sumary>
        public static void salvaVazoes(historicoVazoes vazoesHist, string nomeArquivo, string tipoArquivo="binario")
        {
            // Armazena o número de registros para o primeiro posto. 
            // Para arquivos válidos, o número de registros é igual para todos os postos.
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

        /// <sumary>
        /// Método para alterar/inserir vazões de um histórico de vazões dado.
        /// Esse método é particularmente útil para adequação do histórico quando novos meses/anos precisam ser inseridos.
        /// Argumentos:
        ///     vazoesHist  - objeto histórico de vazões cuja vazão se deseja alterar/incluir;
        ///     posto       - número do posto de vazão a ter sua vazão alterada/incluída;
        ///     mês, ano    - mês e ano ter o valor alterado/incluído;
        ///     novaVazao   - vazão a ser alterada/incluída.
        /// Retorno:
        ///     Nenhum.
        /// </sumary>
        public static void mudaVazao(historicoVazoes vazoesHist, int posto, int mes, int ano, int novaVazao)
        {
            // Checa se o mês está dentro do esperado.
            if (mes<1 || mes >12)
            {
                throw new Exception("Erro: O mês a alterar deve ser um valor numérico entre 1 e 12.");
            }

            // Se o ano especificado for inferior ao primeiro ano do histórico, lança um erro.            
            if (ano < vazoesHist.anoInicial) 
            {
                throw new Exception("Erro: Você não pode alterar vazões de anos anteriores a " + vazoesHist.anoInicial + ".");
            }
            // Se o ano estiver contido no histórico.
            else if (ano>=vazoesHist.anoInicial && ano<=vazoesHist.anoInicial)
            {
                vazoesHist.valores[posto][mes-1] = novaVazao;
            }
            // Se o ano for superior ao ano final do histórico, adiciona vetores nulos se necessário.
            else 
            {
                // Número de anos a inserir no historico;
                int anosInserir = ano - vazoesHist.anoFinal;

                // Lista de valores nulos a inserir no(s) novo(s) ano(s).
                List<int> valoresAno = new List<int>() { 0,0,0,0,0,0,0,0,0,0,0,0 };                
                
                // Atualização do valor final do histórico.
                vazoesHist.anoFinal = vazoesHist.anoFinal + anosInserir;
                
                // Cálculo da posição do valor a ser alterado.
                int pos = (mes-1)+(ano-vazoesHist.anoInicial)*12;

                // Adição das listas de valores nulos.
                for(int p =1; p<=vazoesHist.valores.Count(); p++)
                {
                    vazoesHist.valores[p].AddRange(valoresAno);
                }

                // Alteração do valor especificado.
                vazoesHist.valores[posto][pos] = novaVazao;
            }

        }

    }
}