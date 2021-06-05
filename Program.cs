using System;
using System.Collections.Generic;           // Listas e dicionários
using System.Linq;                          // Métodos para listas

namespace csVazEdit
{
    /* TODO:
        1) Refinar o código da rotina 'main' para tratamento da linha de comando;        
        2) Adicionar mais comentários.
    */
    class Program
    {        
        public class postoVazao
        {
            public string nome = "";
            public Int32 anoInicial = 0;
            public Int32 anoFinal =0;
        }
        
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

            // Testes para os métodos de manipulação de arquivos de MLT.
            // Dictionary<int, List<int>> minhasMLTs = fileOps.leMLTs("tests/MLT.dat", numPostos:320);
            // fileOps.salvaMLTs("tests/Novas_MLTs.dat", minhasMLTs, "binario");
            // fileOps.salvaMLTs("tests/Novas_MLTs.txt", minhasMLTs, "vazEdit");
            // return;

            // Testes para os métodos de manipulação de arquivos de postos.
            // Dictionary<int,postoVazao> meusPostos = fileOps.lePostos("tests/POSTOS.DAT");
            // fileOps.salvaPostos("tests/Novos_Postos.dat", meusPostos, "binario");
            // fileOps.salvaPostos("tests/Novos_Postos.txt", meusPostos, "vazEdit");            
            // return;
            
            // Para executar o método que contem os exemplos, retire os comentários das duas linhas abaixo.
            // exemplos();
            // return;

            // A partir deste ponto, serão executados os métodos considerando os argumentos da linha de comandos.

            // Variáveis iniciais.
            int argsLen = args.Length;
            string arquivoBin;
            string arquivoExcel;
            string arquivoTxt;
            int anoInicial = 1931;
            int numPostos = 320;

            // Mensagem com a sintaxe da linha de comando.
            string sintaxe = @"
            csVazEdit <operação> <arquivo 1> <arquivo 2> [ano incial] [número de postos] [intervalo Excel]
            
            <operação> = -b
            Converte o arquivo de entrada <arquivo 1> de binário para o arquivo de saída <arquivo 2> no formato
            texto VazEdit. Caso o ano inicial seja diferente de 1931 ou o número de postos seja diferente de 320, 
            deve-se especificar nos argumentos opcionais [ano inicial] e [número de postos].

            Importante: se for necessário alterar somente [número de postos], é obrigatório
            especificar [ano inicial].
            
            <operação> = -t
            Converte o arquivo de entrada <arquivo 1> do formato texto VazEdit no arquivo de saída
            <arquivo 2> no formato binário.
            
            <operação> = -e
            Lê dados de um arquivo Excel <arquivo 1> e atualiza um arquivo binário <arquivo 2>. Para esta opção
            é obrigatório definir [intervalo Excel] como uma linha inicial, coluna inicial, linha final, coluna final.
            Por exemplo:
                csVazEdit -e excel.xlsx vazoes.bin 3,2,13,14
                
            Neste exemplo a rotina varrerá o intervalo B3:N13 do Excel.
            Importante: Neste versão inicial, a aba precisa ter o nome 'Dados'.";
            
            // Verifica se existe o número mínimo de argumentos.
            if (argsLen < 3)
            {
                Console.WriteLine(sintaxe);
                return;
            }

            // Verifica se a operação desejava é válida.
            string arg0 = args[0];            
            if (arg0!="-b" && arg0!="-t" && arg0!="-e")
            {
                Console.WriteLine(sintaxe);
                return;
            }
            
            // Operação binário para texto.
            if (arg0=="-b" && argsLen>=3 && argsLen<5)
            {
                arquivoBin = args[1];
                arquivoTxt = args[2];
                
                if (argsLen > 3) anoInicial = Convert.ToInt32(args[3]);
                if (argsLen > 4) numPostos = Convert.ToInt32(args[4]);
                bool ret = binToVazEdit(arquivoBin, arquivoTxt, anoInicial, numPostos);
                
                if (ret)
                {
                    Console.WriteLine("Conversão ok.");
                    return;
                }
                else
                {
                    Console.WriteLine(@"A conversão de binário para texto falhou!
                    Verifique a sintaxe:
                    "+sintaxe);
                    return;
                }
            }

            // Operação texto para binário.
            if (arg0=="-t" && argsLen==3)
            {
                arquivoTxt = args[1];
                arquivoBin = args[2];
                                
                bool ret = VazEditToBin(arquivoTxt, arquivoBin);
                
                if (ret)
                {
                    Console.WriteLine("Conversão ok.");
                    return;
                }
                else
                {
                    Console.WriteLine(@"A conversão de texto para binário falhou!
                    Verifique a sintaxe:
                    "+sintaxe);
                    return;
                }
            }
            
            // Operação atualiza binário com dados do Excel.
            if (arg0=="-e" && argsLen==4)
            {
                arquivoBin = args[2];
                arquivoExcel = args[1];
                string [] sRange = args[3].Split(',');
                int [] range = new int[4];
                for(int a = 0; a < 4; a++) range[a] = Convert.ToInt32(sRange[a]);
                
                bool ret = atualizaBinExcel(arquivoExcel,arquivoBin, range[0],range[1],range[2], range[3]);
                
                if (ret)
                {
                    Console.WriteLine("Conversão ok.");
                    return;
                }
                else
                {
                    Console.WriteLine(@"A atualização de binário com dados do Excel falhou!
                    Verifique a sintaxe:
                    "+sintaxe);
                    return;
                }
            }
        }              
        
        /// <sumary>
        /// Rotina para demonstrar as funcionalidades do csVazEdit.
        /// </sumary>
        public static void exemplos()
        {
            // Código para teste de desempenho simples.
            // var watch = System.Diagnostics.Stopwatch.StartNew();
            // watch.Stop();
            // var elapsedTime = watch.ElapsedMilliseconds;
            // Console.WriteLine(elapsedTime);

            // Exemplo 01 - Lendo um histórico de vazões e salvando com o mesmo formato.
            // Lê os dados de vazão de uma arquivo binário para um objeto 'histocoVazoes'.            
            historicoVazoes meuHistorico = fileOps.leVazoesBin("tests/vazoes_original_ONS.dat");            
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_1.dat", "binario"); 

            // Exemplo 02 - Salvando o histórico lido anteriormente em três formatos diferentes.
            // Salva um histórico de vazões em um arquivo de tipo especificado.
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_2.dat", "binario");     // formato binário
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_2.csv", "csv");         // formato texto csv
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_2.txt", "vazEdit");     // formato texto VazEdit      

            // Exemplo 03 - Alterando alguns valores do histórico e salvando.
            mudaVazao(meuHistorico,1,1,2022,400);
            mudaVazao(meuHistorico,1,2,1931,999);
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_3.txt", "vazEdit");     // formato texto VazEdit      

            // Exemplo 04 - Atualiza o histórico com vazões obtidas do Excel e salva no formato VazEdit.
            // Lê dados de um intervalo do Excel em um dicionário.
            Dictionary<int, List<List<int>>> DadosExcel = excelOps.leVazoesExcel("tests/csVazEdit_Excel.xlsx",3,2,13,14);

            // Usa a rotina 'mudaVazao' para alterar o histórico com os dados do Excel.
            foreach (var posto in DadosExcel.Keys)
            {
                foreach (var dadoVazao in DadosExcel[posto])
                {
                    mudaVazao(meuHistorico,posto, dadoVazao[0], dadoVazao[1], dadoVazao[2]);                    
                }
            }
            fileOps.salvaVazoes(meuHistorico, "tests/vazoes_ons_4.txt", "vazEdit");     // formato texto VazEdit
        }


        /// <sumary>
        ///
        /// Rotina para converter um arquivo binário de vazões para um arquivo texto no formato do aplicativo
        /// VazEdit do ONS.
        ///
        /// Argumentos:
        ///     arquivoBin  - arquivo binário no formato ONS a ter seus dados convertidos;
        ///     arquivoTxt  - nome do arquivo de saída no formato texto;
        ///     anoInicial  - (opcional) ano inicial do histórico. Padrão = 1931;
        ///     numPostos   - (opcional) número de postos do histórico. Padráo = 320.
        ///
        /// Retorno:
        ///     'True' se não houver erro e 'False' se ocorrer(em) erro(s).
        ///
        /// </sumary>
        public static bool binToVazEdit(string arquivoBin, string arquivoTxt, int anoInicial=1931, int numPostos=320)
        {
            bool ret = false;            
            try 
            {
                // Lê histórico de vazões do arquivo binário.
                historicoVazoes vazoesHistorico = fileOps.leVazoesBin(arquivoBin, anoInicial, numPostos);

                // Salva no formato VazEdit
                fileOps.salvaVazoes(vazoesHistorico, arquivoTxt, "vazEdit");
                ret = true;
            }
            catch
            {}
            return ret;
        }


        /// <sumary>
        ///
        /// Rotina para converter um arquivo texto no formato do aplicativo VazEdit em formato binário.
        ///
        /// Argumentos:
        ///     arquivoTxt  - nome do arquivo de entrada no formato texto;
        ///     arquivoBin  - nome do arquivo de saída no formato binário.        
        ///
        /// Retorno:
        ///     'True' se não houver erro e 'False' se ocorrer(em) erro(s).
        ///
        /// </sumary>       
        public static bool VazEditToBin(string arquivoTxt, string arquivoBin)
        {
            bool ret = false;
            try 
            {
                // Lê histórico de vazões do arquivo binário.
                historicoVazoes vazoesHistorico = fileOps.leVazoesTxt(arquivoTxt);

                // Salva no formato VazEdit
                fileOps.salvaVazoes(vazoesHistorico, arquivoBin, "binario");                
                ret = true;
            }
            catch 
            {}
            return ret;
        }

        /// <sumary>
        ///
        /// Atualiza um arquivo binário a partir de um arquivo Excel.
        ///
        /// Argumentos:
        ///     arquivoExcel - nome do arquivo de entrada no Excel;
        ///     arquivoBin   - nome do arquivo de saída no formato binário; 
        ///     li, c1       - linha e coluna da primeira célula (canto superior esquerdo) da tabela de dados a serem lidos;
        ///     l2, c2       - linha e coluna da última célula (canto inferior direito) da tabela de dados a serem lidos.       
        ///
        /// Retorno:
        ///     'True' se não houver erro e 'False' se ocorrer(em) erro(s).
        ///
        /// </sumary>           
        public static bool atualizaBinExcel(string arquivoExcel,string arquivoBin, int l1, int c1, int l2, int c2)
        {
            bool ret = false;
            try 
            {
                // Lê histórico de vazões do arquivo binário.
                historicoVazoes vazoesHistorico = fileOps.leVazoesBin(arquivoBin);

                // Lê arquivo Excel com vazões a atualizar.                
                Dictionary<int, List<List<int>>> DadosExcel = excelOps.leVazoesExcel(arquivoExcel,l1,c1,l2,c2);

                // Usa a rotina 'mudaVazao' para alterar o histórico com os dados do Excel.
                foreach (var posto in DadosExcel.Keys)
                {
                    foreach (var dadoVazao in DadosExcel[posto])
                    {
                        mudaVazao(vazoesHistorico,posto, dadoVazao[0], dadoVazao[1], dadoVazao[2]);                    
                    }
                }

                // Salva arquivo binário atualizado.
                fileOps.salvaVazoes(vazoesHistorico, arquivoBin, "binario");                
                ret = true;
            }
            catch
            {}
            return ret;
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

            // Cálculo da posição do valor a ser alterado.
            int pos = (mes-1)+(ano-vazoesHist.anoInicial)*12;
            
            // Se o ano especificado for inferior ao primeiro ano do histórico, lança um erro.            
            if (ano < vazoesHist.anoInicial) 
            {
                throw new Exception("Erro: Você não pode alterar vazões de anos anteriores a " + vazoesHist.anoInicial + ".");
            }
            // Se o ano estiver contido no histórico.
            else if (ano>=vazoesHist.anoInicial && ano<=vazoesHist.anoFinal)
            {
                vazoesHist.valores[posto][pos] = novaVazao;
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