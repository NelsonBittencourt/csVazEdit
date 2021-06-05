# csVazEdit versão 0.002
Código em C# para ler e escrever arquivos binários de vazão utilizados nos modelos Newave, Decomp, Gevazp e Dessem.


<img src="pictures/csVazEdit_ScreenShot.png" width="700">

<img src="pictures/csVazEdit_WinConsole.png" width="700"> 

<img src="pictures/csVazEdit_txt.png" width="700"> 

Modelo de arquivo Excel que o pyVazEdit lê para atualizar um arquivo de vazões binários:

<img src="pictures/csVazEdit_Exemplo_Excel.png" width="700"> 


## Utilização:

Para utilizar o csVazEdit, existem duas formas básicas:

### A) Alterando o código-fonte e utlizando as funções básicas:

#### A.1) Importando os dados de vazão de um arquivo binário:
```C#

var meuHistorico = fileOps.leVazoesBin(<caminho completo do arquivo binário>); 

```

#### A.2) Importando os dados de vazão de um arquivo texto (formato VazEdit):
```C#

var meuHistorico = fileOps.leVazoesTxt(<caminho completo do arquivo binário>);

```

#### A.3) Alterando ou incluindo valores em um histórico de vazões lido:
```C#

mudaVazao(meuHistorico,1,2,1931,999);

```

#### A.4) Salvando um histórico de vazões lidos em um formato específico:
```C#

 fileOps.salvaVazoes(meuHistorico, <caminho completo para o arquivo de saída>, "binario");     // formato binário
 fileOps.salvaVazoes(meuHistorico, <caminho completo para o arquivo de saída>, "csv");         // formato texto csv
 fileOps.salvaVazoes(meuHistorico, <caminho completo para o arquivo de saída>, "vazEdit");     // formato texto VazEdit     

```
### A.5) Importanto dados de um arquivo binário de MLTs:
```C#

var minhasMLTs = fileOps.leMLTs("tests/MLT.dat", numPostos:320);

```

### A.6) Salvando dados de MLTs para os formatos binário e/ou 'VazEdit':
```C#

fileOps.salvaMLTs("tests/Novas_MLTs.dat", minhasMLTs, "binario");  // formato binário
fileOps.salvaMLTs("tests/Novas_MLTs.txt", minhasMLTs, "vazEdit");  // formato texto 'VazEdit'

```

### A.7) Importanto dados de um arquivo binário de postos:
```C#

var meusPostos = fileOps.lePostos("tests/POSTOS.DAT");

```

### A.8) Salvando dados de postos para os formatos binário e/ou 'VazEdit':
```C#

fileOps.salvaPostos("tests/Novos_Postos.dat", meusPostos, "binario");  // formato binário
fileOps.salvaPostos("tests/Novos_Postos.txt", meusPostos, "vazEdit");  // formato texto 'VazEdit'            

```



### B) Utilizando a linha de comando para invocar uma das funções já criadas:

### B.1) Dados de vazões:

##### B.1.1) Convertendo um arquivo binário de vazões para texto:
```C#

csVazEdit -b <caminho do arquivo binário de entrada> <caminho do arquivo texto de saída> [ano inicial] [número de postos] 

````
ano incial - argumento opcional para especificar o primeiro ano do histórico do arquivo binário. Utilize este parâmetro caso o arquivo binário tenha um ano inicial diferente de 1931;

número de postos - argumento opcional para especificar o número de postos do arquivo binário a ser convertido. O ONS utiliza 320 postos para o modo "operação" do sistema e 600 postos para o modo "planejamento".

##### B.1.2) Convertendo um arquivo texto de vazões para binario:
```C#

csVazEdit -t <caminho do arquivo texto de entrada> <caminho do arquivo binário de saída>

```

##### B.1.3) Atualizando um arquivo binário de vazões com dados lidos de um arquivo Excel (requer o pacote EEPlus):
```C#

csVazEdit -e <caminho do arquivo Excel de entrada> <caminhgo do arquivo binário de saída> <intervalo Excel>

```
<intervalo Excel> devem ser quatro valores separados por vírgulos. São eles: linIni, colIni, linFim e colFim. Onde:
  linIni, colIni  - linha e coluna da primeira célula (canto superior esquerdo) da tabela de dados a serem lidos;
  linFim, colFim  - linha e coluna da última célula (canto inferior direito) da tabela de dados a serem lidos.
 
Exemplo:
```C#

 csVazEdit -e excel.xlsx vazoes.bin 3,2,13,14

```

### B.2) Dados de MLTs:

Em elaboração.

### B.3) Dados de postos:

Em elaboração.

## Dependências:

[EEPlus](https://www.nuget.org/packages/EPPlus)

## Licença:

[Ver licença](LICENSE)

## Projeto relacionado:

[NVazEdit C#](http://nrbenergia.somee.com/SoftDev/NVazEdit/NVazEdit)

## Sobre o autor:

[Meu LinkedIn](http://www.linkedin.com/in/nelsonrossibittencourt)

[Minha página de projetos](http://www.nrbenergia.somee.com)
