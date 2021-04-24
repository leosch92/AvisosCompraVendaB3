# AvisosCompraVendaB3

Essa aplicação tem como objetivo o envio de avisos de e-mails para compra e venda de ativos da B3, de acordo com preços ideais estabelecidos pelo usuário.

São necessários os seguintes itens para a utilização da aplicação:
* Uma chave da API de cotações da [HG Brasil](https://hgbrasil.com/apis/cotacao-acao/b3-brasil-bolsa-balcao-b3sa3)
* Um servidor de email configurado, com credenciais de usuário e senha, para o envio dos e-mails de aviso

**Instruções de instalação:**
1. Faça o download da aplicação em formato .zip da última [release](https://github.com/leosch92/AvisosCompraVendaB3/releases)
2. Faça a extração dos arquivos para um diretório de saída
3. O arquivo de configurações "appsettings.json" deverá ser preenchido com os seguintes dados:
* **apiKey** - Chave da API da HG Brasil
* **targetEmail** - E-mail que receberá os avisos
* **smtpConfig** - configuração do servidor de e-mail
  * **server** - endereço do servidor smtp
  * **port** - porta do servidor smtp, em geral 587
  * **sender** - endereço de email do remetente
  * **username** - credencial de usuário para login no servidor smtp (em geral o mesmo endereço do sender)
  * **password** - credencial de password para login no servidor smtp
* **emailJobIntervalInSeconds** - intervalo em segundos para envio dos e-mails

4. No diretório de saída, a aplicação poderá ser rodada com três parâmetros, na ordem a seguir:
* Nome do Ativo na B3
* Preço de compra
* Preço de venda
```ps
 .\AvisosCompraVendaB3.exe PETR4 25.00 23.80
```
