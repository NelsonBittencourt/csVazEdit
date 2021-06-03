        
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;                        // Pacote EPPlus

namespace csVazEdit
{
    class excelOps
    {
        /// <sumary>
        ///
        /// Método para ler os dados de uma planilha do Excel, escolhi o pacote EPPlus.
        /// Essa escolha foi baseada no critério da facilidade e simplicidade de uso.
        /// Como a licença do EPPlus mudou, recomendo que se utilize outro pacote na sua versão
        /// final do aplicativo. 
        /// Só testei o código do métodos 'leVazoesExcel' no Windows e no VS Code.
        /// Para instalar o pacote, usei a linha de comando abaixo, no terminal:
        ///     dotnet add <nome do projeto> package EPPlus --version 5.6.4
        /// Onde <nome do projeto> é o nome do arquivo sem a extensão 'csproj'. No meu caso,
        /// <nome do projeto> é igual a 'csVazEdit'. Pode ser necessário considerar o nome completo do
        /// arquivo.
        ///
        /// A plalinha deverá conter uma aba chamada 'Dados' com dados no seguinte formato:
        ///     Primeira linha (linIni) - meses/anos das vazões a serem atualizadas/inseridas;
        ///     Primeira coluna (colIni) - os números dos postos a terem valores atualizados/inseridos;
        ///     Demais intervalos: dados das vazões a alterar.
        ///
        ///     Exemplo:
        ///
        ///     Posto   Jan/2020    Fev/2020    Mar/2020
        ///         1        100         200         300
        ///       320        500         600         700
        ///
        /// A primeira célula (linIni, colIni) será ignorada.
        /// 
        /// Argumentos:
        ///     nomeArquivo     - nome do arquivo Excel com as características acima;
        ///     linIni, colIni  - linha e coluna da primeira célula (canto superior esquerdo) da tabela de dados a serem lidos;
        ///     linFim, colFim  - linha e coluna da última célula (canto inferior direito) da tabela de dados a serem lidos.
        /// 
        /// Retorno:
        ///     Dicionário com número do posto e listas de dados a alterar.
        ///
        /// </sumary>
        public static Dictionary<int,List<List<int>>> leVazoesExcel(string nomeArquivo, int linIni, int colIni, int linFim, int colFim)
        {
            string worksheetName = "Dados";             // Nome da aba de dados
            int numCols = colFim-colIni;                // Número de colunas do intervalo
            FileInfo fi = new FileInfo(nomeArquivo);    // Obtêm as informações do arquivo

            List<int> meses = new List<int>();          // Cria lista de meses. Lidos da linha de cabeçalho do Excel
            List<int> anos = new List<int>();           // Cria lista de meses. Lidos da linha de cabeçalho do Excel

            // Dicionário para os dados de saída.
            Dictionary<int, List<List<int>>> outPut = new Dictionary<int, List<List<int>>>();
            
            // Abre o Excel e obtêm os dados.
            using (ExcelPackage package = new ExcelPackage(fi))
			{
                // Necessário para indicar que o aplicativo não é para uso comercial.
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                // Referência para a aba.
                ExcelWorksheet ws = package.Workbook.Worksheets[worksheetName];
                
                // Obtêm as listas de meses e anos para cada coluna do intervalo.
                for (int coluna = colIni+1; coluna < colFim+1; coluna++)
                {
                    DateTime data = Convert.ToDateTime(ws.Cells[linIni, coluna].Value);
                    meses.Add(data.Month);
                    anos.Add(data.Year);             
                }

                // Variável que conterá os arquivos lidos do Excel.
                int valor;
                
                // Loop para as linhas da planilha.
                for (int lin=linIni+1; lin <= linFim; lin++)
                {   
                    // Lista que conterá uma lista de listas de números inteiros.
                    List<List<int>> listaDados = new List<List<int>>();

                    // Obtêm o o número do posto.
                    int posto = Convert.ToInt32(ws.Cells[lin,colIni].Value);                    

                    // Loop para as colunas da planilha.                
                    for (int col=0; col <numCols; col++)
                    {                        
                        // Obtem o valor e converte em número inteiro.
                        valor = Convert.ToInt32(ws.Cells[lin,col+colIni+1].Value);                

                        // Cria a sub-lista com mês, ano e vazão a atualizar.
                        listaDados.Add(new List<int> {meses[col], anos[col], valor});                        
                    }

                    // Atualiza o dicionário de saída.
                    outPut[posto] = listaDados; 
                }
            }
            return outPut;           
        }
    }
}