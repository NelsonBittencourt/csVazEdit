        
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;                        // Pacote EPPlus

namespace csVazEdit
{
    class ExcelOps
    {
        /// <sumary>
        /// Para ler os dados de uma planilha do Excel, escolhi o pacote EPPlus.
        /// Essa escolha foi baseada no critério da facilidade e simplicidade de uso.
        /// Como a licença do EPPlus mudou, recomendo que se utilize outro pacote na sua versão
        /// final do aplicativo. 
        /// Só testei o código do métodos 'leVazoesExcel' no Windows e no VS Code.
        /// Para instalar o pacote, usei a linha de comando abaixo no terminal:
        /// dotnet add <nome do projeto> package EPPlus --version 5.6.4
        /// Onde <nome do projeto> é o nome do arquivo sem a extensão 'csproj'. No meu caso,
        /// <nome do projeto> é igual a 'csVazEdit'.
        ///
        /// <sumary>
        /// Lê valores de vazão de uma planilha Excel (xlsx) para atualizar um arquivo binário de vazões.
        ///
        /// A plalinha deverá conter:
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
        /// </sumary>
        public static Dictionary<int,List<List<int>>> leVazoesExcel(string nomeArquivo, int linIni, int colIni, int linFim, int colFim)
        {
            string worksheetName = "Dados";                 // Nome da aba de dados
            int numCols = colFim-colIni;                    // Número de colunas do intervalo
            FileInfo fi = new FileInfo(nomeArquivo);        // Obtêm as informações do arquivo

            List<int> meses = new List<int>();              // Cria lista de meses. Lidos da linha de cabeçalho do Excel
            List<int> anos = new List<int>();               // Cria lista de meses. Lidos da linha de cabeçalho do Excel

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

                // Valor a ser lido.
                int valor;
                
                // Loop para as linhas e coluna de dados.
                for (int lin=linIni+1; lin <= linFim; lin++)
                {   
                    int posto = Convert.ToInt32(ws.Cells[lin,colIni].Value);                    
                    List<List<int>> listaDados = new List<List<int>>();
                    
                    for (int col=0; col <numCols; col++)
                    {                        
                        valor = Convert.ToInt32(ws.Cells[lin,col+colIni+1].Value);                
                        listaDados.Add(new List<int> {meses[col], anos[col], valor});                        
                    }                                   
                    outPut[posto] = listaDados; 
                }
            }
            return outPut;           
        }
    }
}