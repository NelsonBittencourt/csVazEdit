using System.Collections.Generic;

namespace  csVazEdit
{
    class utils
    {
        static public string msgSintaxe = @"
csVazEdit <var> <oper> <arq. entrada> <arq. saída> <arq. Excel> [-ai <ano incial>] [-np <núm. postos>] [-ex <intervalo Excel>]

Onde:

<var>  - Variável a ter os arquivos convertidos. Os valores aceitos são:
    'vazoes': para realizar operações em arquivos binários, texto e Excel com vazões;
    'postos': para realizar operações em arquivos binários e texto com postos;
    'mlts'  : para realizar operações em arquivos binários e texto com MLTs.

<oper> - Operação desejada. Pode receber os seguintes valores:
    '-paraTexto'        : converte um arquivo binário em um arquivo texto tipo 'VazEdit';
    '-paraCSV'          : converte um arquivo binário em um arquivo texto tipo 'csv' (somente para variável 'vazoes');
    '-paraBinario'      : converte um arquivo texto em binário;
    '-atualizaBinario'  : atualiza um arquivo binário a partir de um arquivo Excel.

<arq. entrada>    - arquivo de entrada para a realização da operação. O arquivo deve existir;

<arq. saída>      - arquivo que será gerado como resultado da operação;

<arq. Excel>      - arquivo Excel que será utilizado para atualizar um arquivo binário (somente operação '-atualizaBinario');                

-ai [ano inicial] - (opcional) ano inicial do arquivo binário de vazões. Valor padrão 1931;                

-np [núm. postos] - (opcional) número de postos do arquivo binário de vazões ou do arquivo binário de mlts. Valor padrão 320;

-ex [range Excel] - (opcional) sequência de 4 números inteiros separados por vírgula representando as coordenadas das células 
inicial e final do intervalo da planilha Excel a ser lida.            


Exemplos:
---------

1) Convertendo arquivo de vazões de binário (vazoes.dat) para texto, usando valores padrão:                
    csVazEdit vazoes -paraTexto vazoes.dat vazoes_t1.txt

2) Convertendo arquivo de vazões de binário (vazoes.dat) para texto, usando 600 postos:                
    csVazEdit vazoes -paraTexto vazoes.dat vazoes_t1.txt -np 600                

3) Convertendo arquivo de vazões texto (vazoes_t1.txt) para binário:                
    csVazEdit vazoes -paraBinario vazoes_t1.txt vazoes_t2.dat

4) Convertendo arquivo de postos de binário para texto e vice-versa:                
    csVazEdit postos -paraTexto postos.dat postos_t1.txt                       
    csVazEdit postos -paraBinario postos_t1.txt postos_t2.dat

5) Convertendo arquivo de MLTs de binário para texto e vice-versa:
    csVazEdit mlts -paraTexto mlt.dat mlt_t1.txt -np 320
    csVazEdit mlts -paraBinario mlt_t1.txt mlt_t2.dat

6) Atualizando um arquivo binário de vazões com dados do Excel:        
    csVazEdit vazoes -atualizaBinario vazoes_t2.dat vazoes_t3.dat excel.xlsx -ex 3,2,13,14

Neste exemplo a rotina varrerá o intervalo B3:N13 do Excel.

Importante 1: se for necessário alterar somente [número postos], é obrigatório especificar [ano inicial].        
Importante 2: na versão atual, a aba do Excel deve ter o nome 'Dados'.";
    
    }
}