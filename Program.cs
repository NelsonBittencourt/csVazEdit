using System;
using System.Collections.Generic;
using System.Linq;

namespace csVazEdit
{
    internal class Program
    {
        // Classe que representa um posto de vazão.
        public class postoVazao
        {
            public string nome = "";
            public int anoInicial = 0;
            public int anoFinal = 0;
        }

        // Classe que representa um histórico de vazões.
        public class historicoVazoes
        {
            public int anoInicial = 0;
            public int anoFinal = 0;
            public Dictionary<int, List<int>> valores = new Dictionary<int, List<int>>();
        }

        /// <sumary>
        ///
        /// Ponto de entrada do aplicativo.
        ///
        /// </sumary>
        private static void Main(string[] args)
        {   
            // Converte a lista de argumentos em uma lista de strings e armazena o número de argumentos.
            List<string> argsL = args.ToList();
            int argsLen = argsL.Count;

            // Valores padrão
            int numPostos = 320;        // número de postos padrão
            int anoInicial = 1931;      // ano inicial padrão
            string excelRange = "";     // intervalo Excel padrão

            // Verifica se a lista de argumentos contem as opções '-np', '-ai' ou '-ex'.
            // Caso uma ou mais opções for encontrada, aloca o valor e limpa a lista.

            int idx = 0;

            // TODO: verificar os argumentos fornecidos.
            // Opção '-np'.
            if ((idx = argsL.IndexOf("-np")) != -1)
            {
                numPostos = Convert.ToInt32(argsL[idx + 1]);
                argsL.RemoveRange(idx, 2);
            }

            // Opção '-ai'.
            if ((idx = argsL.IndexOf("-ai")) != -1)
            {
                anoInicial = Convert.ToInt32(argsL[idx + 1]);
                argsL.RemoveRange(idx, 2);
            }

            // Opção '-ex'.
            if ((idx = argsL.IndexOf("-ex")) != -1)
            {
                excelRange = argsL[idx + 1];
                argsL.RemoveRange(idx, 2);
            }            

            // Verifica o número mínimo de argumentos.
            if (argsLen < 4)
            {
                Console.WriteLine(utils.msgSintaxe);
                return;
            }

            // Variável de retorno para a conversão
            bool ret = false;

            // Seleciona a acão de acordo com a variável selecionada.
            switch (argsL[0])
            {
                case "vazoes":
                    ret = converteVazoes(argsL, numPostos, anoInicial, excelRange);
                    break;
                case "postos":
                    ret = convertePostos(argsL, numPostos);
                    break;
                case "mlts":
                    ret = converteMLTs(argsL, numPostos);
                    break;
                default:
                    throw new Exception("Variável selecionada inválida. Os valores válidos são vazoes, postos ou mlts.");
            }
        }


        /// <sumary>
        ///
        /// Exemplos.
        ///
        /// Argumento:
        ///     Nenhum.
        ///
        /// Retorno:
        ///     Nenhum.
        ///
        /// </sumary>
        public static void exemplos()
        {
            Program.historicoVazoes vazoesHist = fileOps.loadBinVazoes("tests/vazoes_original_ONS.dat");
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_1.dat");
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_2.dat");
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_2.csv", "csv");
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_2.txt", "vazEdit");
            Program.mudaVazao(vazoesHist, 1, 1, 2022, 400);
            Program.mudaVazao(vazoesHist, 1, 2, 1931, 999);
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_3.txt", "vazEdit");
            Dictionary<int, List<List<int>>> dictionary = excelOps.leVazoesExcel("tests/csVazEdit_Excel.xlsx", 3, 2, 13, 14);
            foreach (int key in dictionary.Keys)
            {
                foreach (List<int> it in dictionary[key]) Program.mudaVazao(vazoesHist, key, it[0], it[1], it[2]);
            }
            fileOps.saveVazoes(vazoesHist, "tests/vazoes_ons_4.txt", "vazEdit");
        }


        /// <sumary>
        ///
        /// Converte arquivo de postos de binário para texto ou vice-versa.
        ///
        /// Argumentos:
        ///     args        - lista de strings contendo a linha de comando;
        ///     numPostos   - número total de postos do arquivo.
        ///
        /// Retorno:
        ///     'True' para conversão ok e 'False' para erro.
        ///
        /// </sumary>
        public static bool convertePostos(List<string> args, int numPostos)
        {
            string tipoArqEntrada = "";         // tipo de arquivo de entrada
            bool ret = false;                   // retorno do método

            // Se o arquivo de entrada não existe, lança uma exceção.
            if (!fileOps.fileExist(args[2]))
            {
                throw new Exception(string.Format("Erro na conversão de postos! O arquivo de entrada {0} não existe!", args[2]));
            }

            // Se o número de argumentos for correto (4), tenta converter.
            if (args.Count == 4)
            {
                try
                {
                    // Conversão para texto.
                    if (args[1] == "-paraTexto")
                    {
                        tipoArqEntrada = "binário";
                        Dictionary<int, Program.postoVazao> dictPostos = fileOps.loadBinPostos(args[2], numPostos);
                        fileOps.savePostos(args[3], dictPostos, "vazEdit");
                    }
                    // Conversão para binário.
                    else if (args[1] == "-paraBinario")
                    {
                        tipoArqEntrada = "texto";
                        Dictionary<int, Program.postoVazao> dictPostos = fileOps.loadTxtPostos(args[2], numPostos);
                        fileOps.savePostos(args[3], dictPostos, "binario");
                    }
                    // Tipo de conversão não reconhecido.
                    else
                    {
                        throw new Exception(string.Format("A direção da conversão deve ser -paraTexto ou -paraBinario.", args[1]));
                    }

                    Console.WriteLine("Conversão do arquivo {0} no arquivo {1} bem sucedida!", args[2], args[3]);
                    ret = true;
                }
                catch
                {
                    Console.WriteLine(string.Format(@"Houve um erro na conversão do arquivo {0} de postos {1} para o arquivo {2}.\n
                    Verifique o caminho dos arquivos e se você tem permissão para criação/alteração.", tipoArqEntrada, args[2], args[3]));
                }
            }
            return ret;
        }


        /// <sumary>
        ///
        /// Converte arquivo de MLTs de binário para texto ou vice-versa.
        ///
        /// Argumentos:
        ///     args      - lista de strings com argumentos;
        ///     numPosto  - número total de de postos do arquivo.
        ///
        /// Retorno:
        ///     'True' para conversão ok e 'False' para erro.
        ///
        /// </sumary>
        public static bool converteMLTs(List<string> args, int numPostos)
        {
            string tipoArqEntrada = "";         // tipo de arquivo de entrada
            bool ret = false;                   // retorno do método
            
            // Se o arquivo de entrada não existe, lança uma exceção.
            if (!fileOps.fileExist(args[2]))
            {
                throw new Exception(string.Format("Erro na conversão de MLTs! O arquivo de entrada {0} não existe!", args[2]));
            }

            // Se o número de argumentos for correto (4), tenta converter.
            if (args.Count == 4)
            {
                try
                {
                    // Conversão para texto.
                    if (args[1] == "-paraTexto")
                    {
                        tipoArqEntrada = "binário";
                        Dictionary<int, List<int>> dictMLTs = fileOps.loadBinMLTs(args[2], numPostos);
                        fileOps.saveMLTs(args[3], dictMLTs, "vazEdit");
                    }
                    // Conversão para binário.
                    else if (args[1] == "-paraBinario")
                    {
                        tipoArqEntrada = "texto";
                        Dictionary<int, List<int>> dictMLTs = fileOps.loadTxtMLTs(args[2]);
                        fileOps.saveMLTs(args[3], dictMLTs, "binario");
                    }
                    // Tipo de conversão não reconhecido.
                    else
                    {
                        throw new Exception(string.Format("A direção da conversão de ser -paraTexto ou -paraBinario!", args[1]));
                    }
                    Console.WriteLine("Conversão do arquivo {0} no arquivo {1} bem sucedida!", args[2], args[3]);
                    ret = true;
                }
                catch
                {
                    Console.WriteLine(string.Format("Houve um erro na conversão do arquivo {0} de MLTs {1} para o arquivo {2}.\nVerifique o caminho dos arquivos e se você tem permissão para criação/alteração.", tipoArqEntrada, args[2], args[3]));
                }
            }
            return ret;
        }

        /// <sumary>
        ///
        /// Converte arquivo de vazões de binário para texto ou vice-versa ou atualiza vazões com Excel.
        ///
        /// Argumentos:
        ///     args        - lista de strings com argumentos;
        ///     numPosto    - número total de postos do arquivo;
        ///     anoInicial  - ano inicial do arquivo de vazões;
        ///     excelRAnge  - string com quatro valores separados por vírgulas que representa as coordenadas
        ///     da folha de dados 'Dados' do arquivo Excel.
        ///
        /// Retorno:
        ///     'True' para conversão ok e 'False' para erro.
        ///
        /// </sumary>
        public static bool converteVazoes(List<string> args, int numPostos, int anoInicial, string excelRange)
        {
            string tipoArqEntrada = "";         // tipo de arquivo de entrada
            bool ret = false;                   // retorno do método           
            int argsLen = args.Count;           // número de argumentos passados

            // Caso o arquivo de entrada não exista, lança uma exceção.
            if (!fileOps.fileExist(args[2]))
            {
                throw new Exception(string.Format("Erro na conversão de vazões! O arquivo de entrada {0} não existe!", args[2]));
            }

            // Caso sejam passados 5 argumentos e o intervalo Excel seja válido, verifica a existência
            // do arquivo Excel.
            if (argsLen == 5 && excelRange != "")
            {
                if (!fileOps.fileExist(args[4])) throw new Exception(string.Format("Erro na conversão de vazões! O arquivo Excel {0} não existe!", args[4]));
            }

            // Variável para conter o intervalo Excel na forma de uma lista de inteiros.
            List<int> range = new List<int>();

            // Converte a string 'excelRange' em lista.
            if (excelRange.Length > 0)
            {
                string[] tmp = excelRange.Split(',');
                for (int a = 0; a < 4; a++) range.Add(Convert.ToInt32(tmp[a]));
            }

            // Caso o número de argumentos seja 4 (-paraTexto e -paraBinario) ou 5 (-atualizaBinario),
            // tenta a conversão.
            //
            if (argsLen == 4 || argsLen == 5)
            {
                try
                {   
                    // Conversão para texto.
                    if (args[1] == "-paraTexto")
                    {
                        tipoArqEntrada = "binário";
                        historicoVazoes histVazoes = fileOps.loadBinVazoes(args[2], anoInicial, numPostos);
                        fileOps.saveVazoes(histVazoes, args[3], "vazEdit");
                    }
                    // Conversão para csv.
                    else if (args[1] == "-paraCSV")
                    {
                        tipoArqEntrada = "binário";
                        historicoVazoes histVazoes = fileOps.loadBinVazoes(args[2], anoInicial, numPostos);
                        fileOps.saveVazoes(histVazoes, args[3], "csv");
                    }
                    // Conversão para binário.
                    else if (args[1] == "-paraBinario")
                    {
                        tipoArqEntrada = "texto";
                        historicoVazoes histVazoes = fileOps.loadTxtVazoes(args[2]);
                        fileOps.saveVazoes(histVazoes, args[3], "binario");
                    }
                    // Atualiza binário com Excel.
                    else if (args[1] == "-atualizaBinario")
                    {
                        // Lê arquivo binário de vazões.
                        Program.historicoVazoes vazoesHist = fileOps.loadBinVazoes(args[2], anoInicial, numPostos);

                        // Lê dados do Excel.
                        Dictionary<int, List<List<int>>> dadosExcel = excelOps.leVazoesExcel(args[4], range[0], range[1], range[2], range[3]);

                        // Altera vazões do histórico lido.
                        foreach (int posto in dadosExcel.Keys)
                        {
                            foreach (List<int> dadoExcel in dadosExcel[posto]) mudaVazao(vazoesHist, posto, dadoExcel[0], dadoExcel[1], dadoExcel[2]);
                        }
                        // Salva o novo arquivo.
                        fileOps.saveVazoes(vazoesHist, args[3]);
                    }
                    // Tipo de conversão não reconhecido.
                    else
                    {
                        throw new Exception(string.Format("A direção da conversão de ser -paraTexto , -paraBinario ou -atualizaBinario!", args[1]));
                    }
                    Console.WriteLine("Conversão do arquivo {0} no arquivo {1} bem sucedida!", args[2], args[3]);
                    ret = true;
                }
                catch
                {
                    Console.WriteLine(string.Format("Houve um erro na conversão do arquivo {0} de vazões {1} para o arquivo {2}.\nVerifique o caminho dos arquivos e se você tem permissão para criação/alteração.", tipoArqEntrada, args[2], args[3]));
                }
            }
            return ret;
        }

        /// TODO: Melhorar a performance deste método através da passagem direta da lista de 
        ///  valores a atualizar.
        ///
        /// <sumary>
        ///
        /// Altera/inclui valores de um objeto 'historicoVazoes'.
        ///
        /// Argumentos:
        ///     vazoesHist  - objeto 'historicoVazoes' a ter valores alterados/incluidos;
        ///     posto       - número do posto a ter o valore alterado/incluido;
        ///      mes, ano    - mês e ano a ter o valore alterado/incluido;
        ///      novaVazao   - novo valor.
        ///
        /// Retorno:
        ///     Nenhum.
        ///
        /// </sumary>
        public static void mudaVazao(Program.historicoVazoes vazoesHist, int posto, int mes, int ano, int novaVazao)
        {
            // Testa a validade do mês fornecido.
            if (mes < 1 || mes > 12) throw new Exception("Erro: O mês a alterar deve ser um valor numérico entre 1 e 12.");
            
            // Testa a validade do ano fornecido.
            if (ano < vazoesHist.anoInicial) throw new Exception("Erro: Você não pode alterar vazões de anos anteriores a " + vazoesHist.anoInicial.ToString() + ".");

            // Calcula a posição do valor a ser alterado/incluido.
            int index = mes - 1 + (ano - vazoesHist.anoInicial) * 12;
                        
            //  Se o ano já estiver presente no histórico de vazões, atualiza diretamente.
            if (ano >= vazoesHist.anoInicial && ano <= vazoesHist.anoFinal)
            {
                vazoesHist.valores[posto][index] = novaVazao;
            }
            // Caso o ano não estiver presente, insere o ano.
            // Repare que, para manter a consistência, ao inserir anos devemos inserir em todos os postos.
            else
            {
                // Calcula o número de anos a adicionar no histórico.
                int anosAdic = ano - vazoesHist.anoFinal;
                
                // Cria lista com doze meses para inserir no histórico.
                List<int> tmpList = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                
                // Atualiza o ano final do histórico.
                vazoesHist.anoFinal += anosAdic;

                // Loop para inserir os valores nulos.                
                for(int p =1; p<=vazoesHist.valores.Count(); p++) vazoesHist.valores[p].AddRange(tmpList);

                // Atualiza o valor.
                vazoesHist.valores[posto][index] = novaVazao;
            }
        }
    }
}
