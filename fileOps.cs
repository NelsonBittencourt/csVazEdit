using System;
using System.Collections.Generic;           
using System.Linq;                          
using System.IO;                            
using System.Text;

namespace csVazEdit
{
    class fileOps
    {
 
        // ***************************************************************
        // ********** Métodos para manipular arquivos de MLT *************
        // ***************************************************************

        /// <sumary>
        ///
        /// Lê arquivo binário de MLTs.
        ///
        /// Argumento:
        ///     nomeArquivo - caminho completo para o arquivo binário de postos;
        ///     numPostos   - (opcional) número de postos. Valor padrão = 320.
        ///
        /// Retorno:
        ///     Dicionário com número do posto e lista com 12 valores das MLTs mensais.
        ///
        /// </sumary>
        public static Dictionary<int, List<int>> leMLTs(string nomeArquivo, int numPostos=320)
        {
            // Declara e inicializa variável de saída.
            Dictionary<int,List<int>> tmpDict = new Dictionary<int, List<int>>();
            for(int i=0; i < numPostos;i++) tmpDict[i+1] = new List<int>();            

            // Lê os dados do arquivo.
            byte [] data = File.ReadAllBytes(nomeArquivo);

            // Loop para o número de registros do arquivo.
            for(int s = 0; s < (data.Length-numPostos); s=s+4*(numPostos))
            {
                //  Loop para o número de postos.
                for (int p = 0; p < numPostos; p++)
                {
                    // Converte e aloca o valor lido na variável de saída.
                    tmpDict[p+1].Add(BitConverter.ToInt32(new ArraySegment<byte>(data,(p*4)+s, 4)));
                }
            }
            return tmpDict;
        }

        /// <sumary>
        ///
        /// Salva arquivo com dados de MLTs.
        ///
        /// Os tipos de arquivos permitidos são:
        ///     "binário"   - arquivo binário no formato ONS;
        ///     "vazEdit"   - arquivo texto no formato do aplicativo 'VazEdit' do ONS.
        ///
        /// Argumento:
        ///     nomeArquivo - caminho completo para o arquivo de saída de MLTs;
        ///     dictMLTs    - dicionário no mesmo formato utilizado no método 'leMLTs';
        ///     tipoArquivo - ver acima.
        ///
        /// Retorno:
        ///     Nenhum.
        ///
        /// </sumary>
        public static void salvaMLTs(string nomeArquivo, Dictionary<int, List<int>> dictMLTs, string tipoArquivo)
        {   
            // Para arquivo binário. 
            if (tipoArquivo=="binario")
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(nomeArquivo,FileMode.Create)))
                {   
                    for(int m = 0; m < 12; m++)
                    {  
                        foreach(int posto in dictMLTs.Keys) writer.Write(dictMLTs[posto][m]);
                    }                
                }
            }
            // Para arquivo texto formato 'vazEdit'.
            else if (tipoArquivo=="vazEdit")
            {
                using (StreamWriter writer = new StreamWriter(File.Open(nomeArquivo,FileMode.Create)))
                {
                    foreach(int posto in dictMLTs.Keys)
                    {
                        string saida = posto.ToString().PadLeft(3) + " ";
                        foreach(int mlt in dictMLTs[posto]) saida+= mlt.ToString().PadLeft(6);
                        writer.WriteLine(saida);                        
                    }
                }
            }
            else
            {
                Console.WriteLine("Tipo de arquivo de saída de MLTs não reconhecido!");
                return;
            }
        }

        // ***************************************************************
        // ********** Métodos para manipular arquivos de postos **********
        // ***************************************************************
        
        /// <sumary>
        ///
        /// Lê um arquivo binário com a lista de postos utilizadas nos modelos computacionais do ONS.
        ///
        /// Argumento:
        ///     nomeArquivo - caminho completo para o arquivo binário de postos.
        ///
        /// Retorno:
        ///     Dicionário com número dos postos e objetos 'postoVazao'.
        ///
        /// </sumary>
        public static Dictionary<int, Program.postoVazao> lePostos(string nomeArquivo)
        {
            // Inicializa contador de postos.
            int posto = 1;

            // Cria variável de saída.
            Dictionary<int,Program.postoVazao> tmpDict = new Dictionary<int, Program.postoVazao>();
            
            // Abre arquivo e lê os dados.
            using (BinaryReader reader = new BinaryReader(File.Open(nomeArquivo,FileMode.Open)))
            {
                while(reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    // Cria uma instância temporária de um objeto 'postoVazao'.
                    Program.postoVazao tmpPostoVazao = new Program.postoVazao();  
                    
                    // Lê o nome do posto. Já considerando as conversões necessárias.                    
                    byte[] tmp = reader.ReadBytes(12);
                    tmpPostoVazao.nome = Encoding.Latin1.GetString(tmp);
                    
                    // Lê o ano incial e o ano final.
                    tmpPostoVazao.anoInicial = reader.ReadInt32();                    
                    tmpPostoVazao.anoFinal = reader.ReadInt32();

                    tmpDict[posto] = tmpPostoVazao;
                    posto++;
                }
            }
            return tmpDict;
        }

        /// <sumary>
        ///
        /// Salva um arquivo binário ou texto com a lista de postos utilizadas nos modelos computacionais do ONS.
        ///
        /// Os tipos de arquivos permitidos são:
        ///     "binário"   - arquivo binário no formato ONS;
        ///     "vazEdit"   - arquivo texto no formato do aplicativo 'VazEdit' do ONS.
        ///
        /// Argumentos:
        ///     nomeArquivo - caminho completo para o arquivo de saída com dados de postos;
        ///     dicPostos   - dicionário no mesmo formato do método 'lePostos';
        ///     tipoArquivo - ver acima.
        ///
        /// Retorno:
        ///     Nenhum.
        ///
        /// </sumary>
        public static void salvaPostos(string nomeArquivo, Dictionary<int, Program.postoVazao> dictPostos, string tipoArquivo)
        {    
            // Para arquivo tipo binário.
            if (tipoArquivo=="binario")
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(nomeArquivo,FileMode.Create)))
                {                   
                    foreach(int posto in dictPostos.Keys)
                    {   
                        string nome = dictPostos[posto].nome.Trim().PadRight(12,' ');
                        byte[] unicodeBytes = Encoding.Unicode.GetBytes(nome);
                        byte[] latin = Encoding.Convert(Encoding.Unicode, Encoding.Latin1, unicodeBytes);
                        writer.Write(latin);
                        writer.Write(dictPostos[posto].anoInicial);
                        writer.Write(dictPostos[posto].anoFinal);                       
                    }                
                }
            }
            // Para formato texto 'vazEdit'.
            else if (tipoArquivo=="vazEdit")
            {
                using (StreamWriter writer = new StreamWriter(File.Open(nomeArquivo,FileMode.Create)))
                {
                    foreach(int posto in dictPostos.Keys)                    
                    {
                        string nome = dictPostos[posto].nome.Trim();                        
                        if (nome.Length>0)
                        {   
                            // Cria string de saída.
                            string saida = posto.ToString().PadLeft(4) + "  " + nome.PadRight(12) +
                                           dictPostos[posto].anoInicial.ToString().PadLeft(6) +
                                           dictPostos[posto].anoFinal.ToString().PadLeft(6);                           
                            writer.WriteLine(saida);
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("Tipo de arquivo de saída de postos não reconhecido!");
                return;
            }
        }

        // ***************************************************************
        // ********** Métodos para manipular arquivos de vazões **********
        // ***************************************************************

        /// <sumary>
        ///
        /// Método para ler as vazões de um arquivo txt (no formato VazEdit) para um objeto 'historicoVazoes'.
        ///
        /// Argumentos:
        ///     nomeArquivo - caminho completo do arquivo de vazões binários no formato ONS.        
        ///
        /// Retorno:
        ///     Objeto tipo 'historicoVazoes' com os dados lidos do arquivo.
        ///
        /// </sumary>    
        public static Program.historicoVazoes leVazoesTxt(string nomeArquivo)
        {
            // Objeto que receberá os dados.
            Program.historicoVazoes localHist = new Program.historicoVazoes();
            
            // Lê todas as linhas do arquivo.
            string [] data = File.ReadAllLines(nomeArquivo);
            
            string sPosto = "";     // string com número do posto
            int iPosto = 0;         // número do posto no formato inteiro
            int numPostos = 0;      // contador de postos
            int tamLista = 0;       // número total de vazões para da posto
       
            // Aloca o ano incial e o ano final.
            localHist.anoInicial = Convert.ToInt32(data[0].Substring(4,4));
            localHist.anoFinal = Convert.ToInt32(data[data.Length-1].Substring(4,4));
            
            // Calcula o número total de vazões por posto.
            tamLista = (localHist.anoFinal - localHist.anoInicial + 1) * 12;

            // Loop para as linhas do arquivo.
            foreach(string s in data)
            {
                string posto = s.Substring(0,3);                // obtem uma string com número do posto      
                int iAno = Convert.ToInt32(s.Substring(4,4));   // obtem o ano 

                // Caso ocorra variação na string do posto, converte para inteiro e incrementa contador de postos.
                if (posto!=sPosto) 
                { 
                    sPosto = posto;
                    iPosto = Convert.ToInt32(posto);
                    numPostos++; 
                }

                // Cria lista para armazenar as vazões de um posto.
                List<int> listaValores = new List<int>();

                // Adiciona os doze valores do ano 'iAno" na lista de valores.
                for (int m = 0; m <12; m++)
                {
                    listaValores.Add(Convert.ToInt32(s.Substring(6*m + 8, 6)));
                } 

                // Caso ainda o posto ainda não tenha sido inserido no dicionário, cria lista vazia.
                if (!localHist.valores.ContainsKey(iPosto)) localHist.valores[iPosto]=new List<int>(); 
                
                // Adiciona lista ao dicinário.
                localHist.valores[iPosto].AddRange(listaValores);               
            }

            // Completa os demais postos com zero.
            int maxPostos = 320;
            if (numPostos > 320) maxPostos = 600; 
            
            List<int> tmpList = new List<int>(new int[tamLista]);
            for (int p = 1; p <= maxPostos; p++)
            {
                if (!localHist.valores.ContainsKey(p)) localHist.valores[p] = tmpList;                
            }            
            return localHist;
        }

        /// <sumary>
        ///
        /// Método para ler as vazões de um arquivo binário para um objeto 'historicoVazoes'.
        ///
        /// Argumentos:
        ///     nomeArquivo - caminho completo do arquivo de vazões binários no formato ONS;
        ///     anoInicial  - (Opcional) ano inicial do histórico de vazões. Valor padrão 1931;
        ///     numPostos   - (Opcional) número de postos do arquivo de vazões. 
        ///         O ONS utiliza 320 postos para o modo "operação" e 600 postos para o modo "planejamento".
        ///
        /// Retorno:
        ///     Objeto tipo 'historicoVazoes' com os dados lidos do arquivo.
        ///
        /// </sumary>        
        public static Program.historicoVazoes leVazoesBin(string nomeArquivo, int anoInicial=1931, int numPostos = 320)
        {
            // Cria uma instância de um objeto 'historicoVazoes' para conter os valores de vazões lidas.
            Program.historicoVazoes histLocal = new Program.historicoVazoes();            
            
            // Inicia o objeto com o ano inicial e listas de vazões vazias.
            histLocal.anoInicial = anoInicial;
            for(int p = 1; p < numPostos+1; p++) histLocal.valores[p] = new List<int>();

            // Lê todos os bytes do arquivo.
            byte [] data = File.ReadAllBytes(nomeArquivo);
            
            // Aloca os bytes lidos no objeto.
            for(int r = 0; r < (data.Length-numPostos); r=r+4*(numPostos))
            {
                for(int p = 0; p < numPostos;p++)
                {                       
                    histLocal.valores[p+1].Add(BitConverter.ToInt32(new ArraySegment<byte>(data,(p*4)+r, 4)));
                }
            }

            // Calcula e aloca o ano final no objeto 'historicoVazoes'.
            histLocal.anoFinal = histLocal.anoInicial + (data.Length/(48*numPostos))-1;                       
            return histLocal;
        }    
                
        /// <sumary>
        ///
        /// Salva um histórico de vazões para um arquivo de tipo especificado.
        /// Os tipos de arquivo permitidos são:
        ///     "binário"   - arquivo binário no formato ONS;
        ///     "vazEdit"   - arquivo texto no formato do aplicativo 'VazEdit' do ONS;
        ///     "csv"       - arquivo texto no formato separado por vírgulas.
        ///
        /// Argumentos:
        ///     vazoesHist  - objeto do tipo 'historicoVazoes' com dados a serem salvos;
        ///     nomeArquivo - caminho completo do arquivo de vazões a ser salvo; 
        ///     tipoArquivo - (Opcional) tipo de arquivo (ver acima).
        ///
        /// Retorno:
        ///     Nenhum.       
        ///
        /// </sumary>
        public static void salvaVazoes(Program.historicoVazoes vazoesHist, string nomeArquivo, string tipoArquivo="binario")
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
                return;
                
            }
        }

    }
}